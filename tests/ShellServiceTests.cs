using System;
using Autodesk.AutoCAD.Geometry;

using shell.libacadtest;
using shell.libacadtest.assertions;

namespace shell.tests
{
    [AcadTest("ShellServiceBasePointsTest")]
    public class ShellServiceBasePointsTest : TestCase
    {
        protected override bool Test()
        {
            Point3dCollection points = ShellService.CalculateBasePoints(new ShellSettings());
            return Assert.AreEqual(points.Count, 14);
        }
    }

    [AcadTest("ShelleServiceSplinePointsTest")]
    public class ShelleServiceSplinePointsTest : TestCase
    {
        protected override bool Test()
        {
            Point3dCollection points = ShellService.CalculateBasePoints(new ShellSettings());
            Point3dCollection spline = ShellService.CalculateSplinePoints(points, new ShellSettings());

            return Assert.AreEqual(spline.Count, 3);
        }
    }
}
