namespace Nube.Errors
{
    public static class SyntaxError
    {
        public static int Line { get; set; }
        public static int Column { get; set; }
        public static string Message { get; set; }

        public static void ReportError(int line, int column, string message)
        {
            Console.WriteLine($"ERROR: {message} on line: {line}, column: {column}");
        }
    }
}
