using Nube.LexicalAnalysis;

namespace Nube
{
    public class Program
    {
        static void Main()
        {
            string content = "12e15";
            Lexer l = new Lexer(content);
            //l.Analyze(content);
        }
    }
}
