
namespace Nube.LexicalAnalysis
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Length { get; set; }
        public int Position { get; set; } = 0;

        public void Deconstruct(out string type, out string value, out int length, out int position)
        {
            type = Type;
            value = Value;
            length = Length;
            position = Position;
        }
    }
}
