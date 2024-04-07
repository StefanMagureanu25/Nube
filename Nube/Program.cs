using Nube.LexicalAnalysis;

namespace Nube
{
    public class Program
    {
        static void Main(string[] args )
        {
            try
            {
                using (var file = new StreamReader(@"D:\Nube\Nube\input.txt"))
                {
                    var content = file.ReadToEnd();
                    Lexer l = new Lexer(content);
                    l.Analyze(content);
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
