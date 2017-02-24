namespace shell.libacadtest.assertions
{
    public static class Assert
    {
        public static bool AreEqual<T>(T actual, T expected)
        {
            return actual.Equals(expected);
        }
    }
}
