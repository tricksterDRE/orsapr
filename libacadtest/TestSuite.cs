using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace shell.libacadtest
{
    public class TestSuite
    {
        [CommandMethod("libacadtestruntests")]
        public void RunTests()
        {
            Utils.WriteMessage("Starting test runner.\n");

            foreach (TestCase test in CollectTests())
                test.Run();

            Utils.WriteMessage("Tests execution completed.\n");
        }

        private List<TestCase> CollectTests()
        {
            List<TestCase> retval = new List<TestCase>();
            Type[] typesInAssembly = Assembly.GetAssembly(typeof(TestSuite)).GetTypes();

            foreach (Type type in typesInAssembly)
            {
                AcadTestAttribute testAttribute = type.GetCustomAttribute(typeof(AcadTestAttribute), true) as AcadTestAttribute;

                if (testAttribute != null)
                {
                    TestCase testCase = Activator.CreateInstance(type) as TestCase;

                    if (testCase != null)
                    {
                        testCase.TestCaseName = testAttribute.TestName;
                        retval.Add(testCase);
                    }
                }
            }

            return retval;
        }
    }
}
