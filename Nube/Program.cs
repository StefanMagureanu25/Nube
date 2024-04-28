using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis;
using Nube.Syntactic_Analysis.Printer;

namespace Nube
{
    public class Program
    {
        static void Main(string[] args)
        {
            double x = 3.2;
            int y = 3;
            Console.WriteLine(x - y);
            /* try
             {
                 using (var file = new StreamReader(@"D:\Nube\Nube\Documentation\input.txt"))
                 {
                     Lexer lexer = new Lexer();
                     while ((lexer.Content = file.ReadLine()) != null)
                     {
                         lexer.Analyze(lexer.Content);
                         lexer.NextLine();
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
             }*/
            Expression expression = new Expression.Binary(
            new Expression.Unary(
                    new Token(TokenType.MINUS, "-", 1, 1, 1),
                    new Expression.Literal(123)
                                ),
            new Token(TokenType.MULTIPLY, "*", 1, 1, 1),
            new Expression.Grouping(
                        new Expression.Literal(45.67))
                                );
            Console.WriteLine(new AstPrinter().Print(expression));


        }
    }
}