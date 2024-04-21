
namespace Nube.LexicalAnalysis
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Length { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }

        public Token()
        {
            Length = 0;
            Position = 0;
            Line = 0;
            Value = "";
            Type = TokenType.INVALID;
        }
        public void Deconstruct(out TokenType type, out string value, out int length, out int line, out int position)
        {
            type = Type;
            value = Value;
            length = Length;
            line = Line;
            position = Position;
        }
        public override string ToString()
        {
            return $"{Type}:'{Value}' with length {Length} at line {Line}, position {Position}";
        }
    }
}
