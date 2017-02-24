using shell.libacadtest;
using System.Diagnostics;
using Autodesk.AutoCAD.Geometry;

namespace shell.tests
{
    [AcadTest("ShellRendererLoadTest")]
    public class ShellRendererLoadTest : TestCase
    {
        private const int loadIterationsCount = 20;

        private ShellSettings settings = new ShellSettings();
        private Point3d startPoint = Point3d.Origin;

        protected override bool Test()
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < loadIterationsCount; i++)
            {
                Point3dCollection contour       = ShellService.CalculateBasePoints(settings);
                Point3dCollection splineContour = ShellService.CalculateSplinePoints(contour, settings);

                ShellRenderer.Render(contour, splineContour, startPoint);
                watch.Stop();

                startPoint = startPoint + Vector3d.XAxis * 10.0;

                Utils.WriteMessage($"\t\tTest iteration {i}, execution time = {watch.ElapsedMilliseconds} ms.\n");
                watch.Reset();
                watch.Start();
            }

            return true;
        }
    }
}
