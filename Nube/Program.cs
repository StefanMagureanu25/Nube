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
                using (var file = new StreamReader(@"D:\Nube\Nube\Documentation\test.txt"))
                {
                    while ((Lexer.Content = file.ReadLine()) != null)
                    {
                        Lexer.Analyze(Lexer.Content);
                        Lexer.Tokens.Add(new Token(TokenType.EOF, null, 0, Lexer.Line, Lexer.Position + 1));
                        foreach (Token token in Lexer.Tokens)
                        {
                            Console.WriteLine(token.ToString());
                        }
                        Parser parser = new Parser(Lexer.Tokens);
                        Expression expr = parser.parse();
                        Console.WriteLine($"\nRezultatul operatiei este:");
                        interpreter.interpret(expr);
                        Console.WriteLine();
                        Lexer.NextLine();
                        // doar pentru testarea operatiilor pe mai multe linii
                        Lexer.Tokens.Clear();
                    }
                    foreach (var token in Lexer.Tokens)
                    {
                        Console.WriteLine(token.ToString());
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fisierul introdus ca sursa nu a putut fi gasit!");
            }
            catch (Exception)
            {
                Console.WriteLine("A intervenit o alta problema in citirea fisierului!");
            }
        }
    }
}