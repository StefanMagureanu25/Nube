using Nube.LexicalAnalysis;

namespace Nube.Errors
{
    public class ParseError : Exception
    {
        public ParseError(Token token, string message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
