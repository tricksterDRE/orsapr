using System;

namespace shell.libacadtest
{
    public abstract class TestCase
    {
        public string TestCaseName { get; set; }

        public void Run() => Utils.WriteMessage(Test() ? $"\tTest {TestCaseName} successful.\n" : $"\tTest {TestCaseName} failed.\n");
        protected abstract bool Test();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AcadTestAttribute: Attribute
    {
        public string TestName;

        public AcadTestAttribute(string TestName)
        {
            this.TestName = TestName;
        }
    }
}
