using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis;
using Nube.Syntactic_Analysis.Interpreter;

namespace Nube
{
    public class Program
    {
        private static Interpreter interpreter = new Interpreter();
        static void Main(string[] args)
        {
            try
            {
                using (var file = new StreamReader(@"C:\Nube\Nube\Documentation\input.txt"))
                {
                    Lexer.Content = file.ReadToEnd();
                    Lexer.Analyze(Lexer.Content);
                    Lexer.Tokens.Add(new Token(TokenType.EOF, null, 0, Lexer.Line, Lexer.Position + 1));
                    /*foreach (Token token in Lexer.Tokens)
                    {
                        Console.WriteLine(token.ToString());
                    }*/
                    Parser parser = new Parser(Lexer.Tokens);
                    List<Statement> statements = parser.parse();
                    interpreter.interpret(statements);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fisierul introdus ca sursa nu a putut fi gasit!");
            }
            catch (Exception)
            {

            }
        }
    }
}