using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using static Autodesk.AutoCAD.DatabaseServices.Surface;

namespace shell
{
    /// <summary>
    /// Класс с логикой генерации контуров вращения гильзы
    /// </summary>
    public static class ShellService
    {
        /// <summary>
        /// Создает коллекцию точек первого контура вращения
        /// </summary>
        public static Point3dCollection CalculateBasePoints(ShellSettings shellSettings)
        {
            // и сохраним их в переменных
            //
            double shellSize = shellSettings.ShellSize, shellRadius = shellSettings.ShellRadius, bulletRadius = shellSettings.BulletRadius;
            double upperCapsuleRadius = shellSettings.UpperCapsuleRadius, lowerCapsuleRadius = shellSettings.LowerCapsuleRadius;
            double flangeSize = shellSettings.FlangeSize, flangeEdge = shellSettings.FlangeEdge;

            // рассчитаем направляющие векторы гильзы, radius-векторы направлены вдоль оси Ох, size-векторы направлены вдоль Оz
            //
            Vector3d shellRadiusVector = Vector3d.XAxis * shellRadius;
            Vector3d bulletRadiusVector = Vector3d.XAxis * bulletRadius;
            Vector3d upperCapsuleRadiusVector = Vector3d.XAxis * upperCapsuleRadius;
            Vector3d lowerCapsuleRadiusVector = Vector3d.XAxis * lowerCapsuleRadius;

            Vector3d shellSizeVector = Vector3d.ZAxis * shellSize;
            Vector3d flangeSizeVector = Vector3d.ZAxis * flangeSize;
            Vector3d flangeEdgeVector = Vector3d.ZAxis * flangeEdge;

            double bodyBaseSize = flangeSize + flangeEdge;
            Vector3d bodyBaseSizeVector = Vector3d.ZAxis * bodyBaseSize;

            double midShellBodySize = (shellSize - flangeSize) / 2.0 + flangeSize;
            Vector3d midShellBodySizeVector = Vector3d.ZAxis * midShellBodySize;

            double flangeLowerRadius = shellRadius * 0.85;
            Vector3d flangeLowerRadiusVector = Vector3d.XAxis * flangeLowerRadius;

            double midFlangeSize = (flangeSize - flangeEdge) / 2.0 + flangeEdge;
            Vector3d midFlangeSizeVector = Vector3d.ZAxis * midFlangeSize;

            double outerBevelRadius = shellRadius - flangeEdge / 2.0;
            double innerBevelRadius = lowerCapsuleRadius + flangeEdge / 4.0;

            Vector3d outerBevelRadiusVector = Vector3d.XAxis * outerBevelRadius;
            Vector3d innerBevelRadiusVector = Vector3d.XAxis * innerBevelRadius;

            // наружный контур гильзы
            Point3dCollection contour = new Point3dCollection();

            // добавляем точки первого контура - простое сложение векторов
            //
            contour.Add(Point3d.Origin + bodyBaseSizeVector + upperCapsuleRadiusVector);
            contour.Add(Point3d.Origin + flangeSizeVector + upperCapsuleRadiusVector);
            contour.Add(Point3d.Origin + flangeSizeVector + lowerCapsuleRadiusVector);

            contour.Add(Point3d.Origin + flangeEdgeVector / 4.0 + lowerCapsuleRadiusVector);
            contour.Add(Point3d.Origin + innerBevelRadiusVector);
            contour.Add(Point3d.Origin + outerBevelRadiusVector);
            contour.Add(Point3d.Origin + flangeEdgeVector / 2.0 + shellRadiusVector);

            contour.Add(Point3d.Origin + flangeEdgeVector + shellRadiusVector);
            contour.Add(Point3d.Origin + flangeEdgeVector + flangeLowerRadiusVector);
            contour.Add(Point3d.Origin + midFlangeSizeVector + flangeLowerRadiusVector);
            contour.Add(Point3d.Origin + flangeSizeVector + shellRadiusVector);
            contour.Add(Point3d.Origin + shellSizeVector + shellRadiusVector);
            contour.Add(Point3d.Origin + shellSizeVector + bulletRadiusVector);
            contour.Add(Point3d.Origin + midShellBodySizeVector + bulletRadiusVector);

            return contour;
        }

        /// <summary>
        /// Создает коллекцию точек второго контура вращения (сглаженная часть)
        /// </summary>
        public static Point3dCollection CalculateSplinePoints(Point3dCollection contour, ShellSettings shellSettings)
        {
            Point3dCollection splineContour = new Point3dCollection();

            double bodyBaseSize = shellSettings.FlangeSize + shellSettings.FlangeEdge;

            Vector3d bodyBaseSizeVector = Vector3d.ZAxis * bodyBaseSize;
            Vector3d bulletRadiusVector = Vector3d.XAxis * shellSettings.BulletRadius;

            splineContour.Add(contour[contour.Count - 1]);
            splineContour.Add(Point3d.Origin + bodyBaseSizeVector + bulletRadiusVector);
            splineContour.Add(contour[0]);

            return splineContour;
        }
    }

    /// <summary>
    /// Класс с логикой отображения гильзы
    /// </summary>
    public static class ShellRenderer
    {
        public static void Render(Point3dCollection contour, Point3dCollection splineContour, Point3d basePoint)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Matrix3d translationMtr = Matrix3d.Displacement(Point3d.Origin.GetVectorTo(basePoint));

            // открываем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // получаем таблицу блоков
                BlockTableRecord ms = (BlockTableRecord) tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);

                // создаем полилинию по первому контуру и добавляем ее в чертеж
                Polyline3d contourPoly = new Polyline3d(Poly3dType.SimplePoly, contour, false);
                ms.AppendEntity(contourPoly);
                tr.AddNewlyCreatedDBObject(contourPoly, true);

                // второй контур
                Polyline3d contourSplinePoly = new Polyline3d(Poly3dType.QuadSplinePoly, splineContour, false);
                ms.AppendEntity(contourSplinePoly);
                tr.AddNewlyCreatedDBObject(contourSplinePoly, true);

                // поверхности вращения
                RevolvedSurface rev1 = CreateRevolvedSurface(new Profile3d(contourPoly), Point3d.Origin, Vector3d.ZAxis, 2 * Math.PI, 0.0, new RevolveOptions());
                RevolvedSurface rev2 = CreateRevolvedSurface(new Profile3d(contourSplinePoly), Point3d.Origin, Vector3d.ZAxis, 2 * Math.PI, 0.0, new RevolveOptions());

                // добавим в чертеж первую поверхность вращения
                ms.AppendEntity(rev1);
                tr.AddNewlyCreatedDBObject(rev1, true);

                // добавим в чертеж вторую поверхность вращения
                ms.AppendEntity(rev2);
                tr.AddNewlyCreatedDBObject(rev2, true);

                // создадим солид из двух поверхностей вращения
                Solid3d s = new Solid3d();
                s.CreateSculptedSolid(new Entity[] { rev1, rev2 }, new IntegerCollection());
                s.TransformBy(translationMtr);

                // добавить созданный солид в чертеж
                ms.AppendEntity(s);
                tr.AddNewlyCreatedDBObject(s, true);

                contourPoly.Erase();
                contourSplinePoly.Erase();
                rev1.Erase();
                rev2.Erase();

                // коммитим транзакцию - иначе у нас ничего в чертеже не нарисуется
                tr.Commit();

                contourPoly.Dispose();
                contourSplinePoly.Dispose();
                rev1.Dispose();
                rev2.Dispose();
                s.Dispose();
            }
        }
    }

    /// <summary>
    /// Класс с командами автокада
    /// </summary>
    public class Shell: IExtensionApplication
    {
        public void Initialize()
        {
        }

        public void Terminate()
        {
        }

        /// <summary>
        /// Команда для отрисовки гильзы
        /// </summary>
        [CommandMethod("makeshell")]
        public void MakeShell()
        {
            // создадим новый диалог с настройками гильзы
            ShellSettingsDialog settingsDialog = new ShellSettingsDialog();

            // если в диалоге нажали на "окей"
            if (settingsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Point3dCollection contour       = ShellService.CalculateBasePoints(settingsDialog.Settings);
                Point3dCollection splineContour = ShellService.CalculateSplinePoints(contour, settingsDialog.Settings);

                ShellRenderer.Render(contour, splineContour, Point3d.Origin);
            }
        }
    }
}
