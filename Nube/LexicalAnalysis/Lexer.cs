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
        // I want to step over \t, ' ', \n, \0 (EOF)
        private bool stepOver(char c)
        {
            if (c == '\n' || c == '\0' || c == '\t' || c == ' ' || c == '\r')
            {
                return true;
            }
            return false;
        }
        private string checkKeyword(string value)
        {
            foreach (string keyword in TokenType.keywords)
            {
                if (value == keyword)
                {
                    return keyword;
                }
            }
            return TokenType.IDENT;
        }
        private bool isLetter(char ch)
        {
            return (ch == '_') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }
        // Check if I can read the next character next to the lexer position
        private bool lastCharacter()
        {
            if (NextPosition == Content.Length)
            {
                return true;
            }
            return false;
        }
        // read next character and move the lexer with one position to the right
        private void readNextCharacter()
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
        private Token NextToken()
        {
            Token token = new Token();
            switch (Symbol)
            {
                #region Delimiters Verification
                case '.':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.DOT, ".", 1);
                    readNextCharacter();
                    break;
                case ',':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.COMMA, ",", 1);
                    readNextCharacter();
                    break;
                case '(':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LPAREN, "(", 1);
                    readNextCharacter();
                    break;
                case ')':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RPAREN, ")", 1);
                    readNextCharacter();
                    break;
                case ':':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.COLON, ":", 1);
                    readNextCharacter();
                    break;
                case '{':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LBRACE, "{", 1);
                    readNextCharacter();
                    break;
                case '}':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RBRACE, "}", 1);
                    readNextCharacter();
                    break;
                case ';':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.SEMICOLON, ";", 1);
                    readNextCharacter();
                    break;
                case '[':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.LBRACKET, "[", 1);
                    readNextCharacter();
                    break;
                case ']':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.RBRACKET, "]", 1);
                    readNextCharacter();
                    break;
                case '"':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.QUOTATION, "\"", 1);
                    readNextCharacter();
                    break;
                case '\'':
                    (token.Type, token.Value, token.Length) = (TokenType.Delimiters.APOSTROPHE, "'", 1);
                    readNextCharacter();
                    break;
                #endregion
                #region Operators Verification
                case '+':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.PLUS, "+", 1);
                    readNextCharacter();
                    break;
                case '-':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MINUS, "-", 1);
                    readNextCharacter();
                    break;
                case '=':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.EQUAL, "=", 1);
                    readNextCharacter();
                    break;
                case '>':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.GREATER, ">", 1);
                    readNextCharacter();
                    break;
                case '<':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.LESS, "<", 1);
                    readNextCharacter();
                    break;
                case '*':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MULTIPLY, "*", 1);
                    readNextCharacter();
                    break;
                case '/':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.DIVIDE, "/", 1);
                    readNextCharacter();
                    break;
                case '%':
                    (token.Type, token.Value, token.Length) = (TokenType.Operators.MOD, "%", 1);
                    readNextCharacter();
                    break;
                #endregion
                default:
                    if (isLetter(Symbol))
                    {
                        (token.Type, token.Value, token.Length, token.Position) = checkTokenType();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length, token.Position) = checkNumber();
                    }
                    break;
            }
            return token;
        }
        private Token checkTokenType()
        {
            Token token = new Token();
            string value = "";
            while (isLetter(Symbol))
            {
                value += Symbol;
                readNextCharacter();
            }
            token.Value = value;
            token.Length = value.Length;
            string tokenType = checkKeyword(value);
            return token;
        }
        private Token checkNumber()
        {
            Token token = new Token();
            string value = "";
            // ex: 2e-15
            bool scientificNumber = false;
            // ex: 2.37
            bool floatNumber = false;
            bool hasSign = true;
            while (Char.IsDigit(Symbol) || scientificNumber == false || floatNumber == false)
            {
                if (stepOver(Symbol) || Symbol == ';' || Symbol == ',')
                {
                    break;
                }
                if (scientificNumber && Symbol == 'e')
                {
                    token.Type = TokenType.ILLEGAL;
                    break;
                }
                if (floatNumber && Symbol == '.')
                {
                    token.Type = TokenType.ILLEGAL;
                    break;
                }
                if (Char.IsDigit(Symbol) == false && Symbol != 'e' && Symbol != '.')
                {
                    token.Type = TokenType.ILLEGAL;
                    break;
                }
                if (Symbol == 'e')
                {
                    scientificNumber = true;
                }
                else if (Symbol == '.')
                {
                    floatNumber = true;
                }
                value += Symbol;
                readNextCharacter();
            }
            if (token.Type == TokenType.ILLEGAL)
            {
                token.Value = "Programul nu recunoaste acest tip de input!";
                token.Length = 0;
            }
            else if (floatNumber)
            {
                token.Value = value;
                token.Length = value.Length;
                token.Type = TokenType.FLOAT_CONSTANT;
            }
            else
            {
                token.Value = value;
                token.Length = value.Length;
                token.Type = TokenType.INT_CONSTANT;
            }
            return token;
        }
        public void Analyze(string content)
        {
            readNextCharacter();
            if (Symbol != (char)0 && Symbol != ' ')
            {
                Token token = NextToken();
                Console.WriteLine($"{token.Value}, lungimea: {token.Length}");
            }
            while (Symbol != (char)0)
            {
                if (stepOver(Symbol))
                {
                    readNextCharacter();
                    continue;
                }
                Token token = NextToken();
                Console.WriteLine($"{token.Value}, lungimea: {token.Length}");
            }
        }
    }
}
