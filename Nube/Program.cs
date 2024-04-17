using Nube.LexicalAnalysis;

namespace Nube
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var file = new StreamReader(@"D:\Nube\Nube\input.txt"))
                {
                    Lexer l = new Lexer();
                    string contentLine;
                    while ((contentLine = file.ReadLine()) != null)
                    {
                        l.Content = contentLine;
                        l.Analyze(l.Content);
                        l.NextLine();
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