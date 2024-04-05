namespace Nube.LexicalAnalysis
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Length { get; set; }
        public int Position { get; set; } = 0;
    }
}
