
namespace Nube.LexicalAnalysis
{
    public class Token
    {
        public TokenType Type { get; set; }
        public Object Value { get; set; }
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
        public Token(TokenType type, Object value, int length, int line, int position)
        {
            Type = type;
            Value = value;
            Length = length;
            Position = position;
            Line = line;
        }

        public void Deconstruct(out TokenType type, out Object value, out int length, out int line, out int position)
        {
            type = Type;
            value = Value;
            length = Length;
            line = Line;
            position = Position;
        }
        public override string ToString()
        {
            if (Value == null)
            {
                return $"{Type}:'{Value}' with length {Length} at line {Line}, position {Position}";
            }
            return $"{Type}:'{Value}', {Value.GetType()} with length {Length} at line {Line}, position {Position}";
        }
    }
}
