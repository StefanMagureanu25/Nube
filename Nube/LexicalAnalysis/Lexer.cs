namespace Nube.LexicalAnalysis
{
    public class Lexer
    {
        public string Content { get; set; }
        //Position where the finite automata(lexer) is situated
        public int Position { get; set; }
        //Actual position for the character we read
        public int NextPosition { get; set; }
        public char Symbol { get; set; }
        public Lexer(string content)
        {
            Content = content;
            Position = -1;
            NextPosition = 0;
        }
        // Check if I can read the next character next to the lexer position
        public bool lastCharacter()
        {
            if (NextPosition == Content.Length)
            {
                return true;
            }
            return false;
        }
        // read next character and move the lexer with one position to the right
        public void readNextCharacter()
        {
            if (NextPosition >= Content.Length)
            {
                Symbol = (char)0;
            }
            else
            {
                Symbol = Content[NextPosition];
            }
            Position = NextPosition;
            NextPosition++;
        }
        public Token NextToken()
        {
            Token token = new Token();
            switch (Symbol)
            {
                #region Delimiters Verification
                case '.':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.DOT, ".", 1);
                    break;
                case ',':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.COMMA, ",", 1);
                    break;
                case '(':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LPAREN, "(", 1);
                    break;
                case ')':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RPAREN, ")", 1);
                    break;
                case ':':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.COLON, ":", 1);
                    break;
                case '{':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LBRACE, "{", 1);
                    break;
                case '}':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RBRACE, "}", 1);
                    break;
                case ';':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.SEMICOLON, ";", 1);
                    break;
                case '[':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LBRACKET, "[", 1);
                    break;
                case ']':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RBRACKET, "]", 1);
                    break;
                case '"':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.QUOTATION, "\"", 1);
                    break;
                case '\'':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.APOSTROPHE, "'", 1);
                    break;
                #endregion
                #region Operators Verification
                case '+':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.PLUS, "+", 1);
                    break;
                case '-':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MINUS, "-", 1);
                    break;
                case '=':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.EQUAL, "=", 1);
                    break;
                case '>':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.GREATER, ">", 1);
                    break;
                case '<':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.LESS, "<", 1);
                    break;
                case '*':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MULTIPLY, "*", 1);
                    break;
                case '/':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.DIVIDE, "/", 1);
                    break;
                case '%':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MOD, "%", 1);
                    break;
                #endregion
                default:

                    break;
            }
            return token;
        }
        public void analyze(string content)
        {

        }
    }
}
