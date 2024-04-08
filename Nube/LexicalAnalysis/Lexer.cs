using System.Text.RegularExpressions;

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
        // I want to step over \t,\r, ' ', \n, \0 (EOF)
        private bool stepOver(char c)
        {
            if (c == '\n' || c == '\0' || c == '\t' || c == ' ' || c == '\r')
            {
                return true;
            }
            return false;
        }
        private bool isPartOfNumber(char c)
        {
            return char.IsDigit(c) || c == '.' || c == 'e' || c == 'E' || c == '-' || c == '+';
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
                    if (Char.IsDigit(Content[NextPosition]))
                    {
                        (token.Type, token.Value, token.Length, token.Position) = checkNumber();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Delimiters.DOT, ".", 1);
                        readNextCharacter();
                    }
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
                    readNextCharacter();
                    if (Symbol != '"')
                    {
                        (token.Type, token.Value, token.Length, token.Position) = TakeString();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.STRING_CONSTANT, "", 0);
                        readNextCharacter();
                    }
                    break;
                case '\'':
                    readNextCharacter();
                    if (Symbol != '\'')
                    {
                        (token.Type, token.Value, token.Length, token.Position) = TakeChar();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.CHAR_CONSTANT, "", 0);
                        readNextCharacter();
                    }
                    break;
                #endregion
                #region Operators Verification
                case '!':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.NOT_EQUAL, "!=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.NOT, "!", 1);
                    }
                    break;
                case '&':
                    readNextCharacter();
                    if (Symbol == '&')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.AND, "&&", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_AND_EQUAL, "&=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_AND, "&", 1);
                    }
                    break;
                case '|':
                    readNextCharacter();
                    if (Symbol == '|')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.OR, "||", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_OR_EQUAL, "|=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_OR, "|", 1);
                    }
                    break;
                case '^':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_XOR_EQUAL, "^=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.BITWISE_XOR, "^", 1);
                    }
                    break;
                case '+':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.PLUS_EQUAL, "+=", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '+')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.PLUS_PLUS, "++", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.PLUS, "+", 1);
                    }
                    break;
                case '-':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MINUS_EQUAL, "-=", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '-')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MINUS_MINUS, "--", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MINUS, "-", 1);
                    }
                    break;
                case '=':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.EQUAL_EQUAL, "==", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.EQUAL, "=", 1);
                    }
                    break;
                case '>':
                    readNextCharacter();
                    if (Symbol == '>')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.RIGHT_SHIFT, ">>", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.GREATER_EQUAL, ">=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.GREATER, ">", 1);
                    }
                    break;
                case '<':
                    readNextCharacter();
                    if (Symbol == '<')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.LEFT_SHIFT, "<<", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.LESS_EQUAL, "<=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.LESS, "<", 1);
                    }
                    break;
                case '*':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MULTIPLY_EQUAL, "*=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MULTIPLY, "*", 1);
                    }
                    break;
                case '/':
                    readNextCharacter();
                    if (Symbol == '*' || Symbol == '/')
                    {
                        (token.Type, token.Value, token.Length, token.Position) = TakeComment();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.DIVIDE_EQUAL, "/=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.DIVIDE, "/", 1);
                    }
                    break;
                case '%':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MOD_EQUAL, "/=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.Operators.MOD, "%", 1);
                    }
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
        private Token TakeString()
        {
            Token token = new Token();
            string value = "";
            bool acceptedString = false;
            // check until EOF
            while (Symbol != (char)0)
            {
                value += Symbol;
                readNextCharacter();
                if (Symbol == '"')
                {
                    acceptedString = true;
                    // I have to jump over "
                    readNextCharacter();
                    break;
                }
            }
            if (acceptedString)
            {
                token.Value = value;
                token.Length = value.Length;
                token.Type = TokenType.STRING_CONSTANT;
            }
            else
            {
                token.Type = TokenType.ILLEGAL;
            }
            return token;
        }
        private Token TakeChar()
        {
            Token token = new Token();
            string value = "";
            bool acceptedChar = false;
            while (Symbol != (char)0)
            {
                value += Symbol;
                if (value.Length > 1)
                {
                    // Char can't have length bigger than 1
                    break;
                }
                readNextCharacter();
                if (Symbol == '\'')
                {
                    acceptedChar = true;
                    // I have to jump over '
                    readNextCharacter();
                    break;
                }
            }
            if (acceptedChar)
            {
                token.Value = value;
                token.Length = value.Length;
                token.Type = TokenType.CHAR_CONSTANT;
            }
            else
            {
                token.Type = TokenType.ILLEGAL;
            }
            return token;
        }
        private Token TakeComment()
        {
            Token token = new Token();
            string value = "";
            // single line comment
            if (Symbol == '/')
            {
                readNextCharacter();
                while (Symbol != '\n' && Symbol != '\r')
                {
                    value += Symbol;
                    readNextCharacter();
                }
                token.Value = value;
                token.Length = value.Length;
                token.Type = TokenType.COMMENT;
            }
            // multi-line comment
            else
            {
                bool acceptedComment = false;
                readNextCharacter();
                // check until EOF
                while (Symbol != (char)0)
                {
                    if (Symbol == '*' && Content[NextPosition] == '/')
                    {
                        acceptedComment = true;
                        // I have to jump over '/' and go to the next character
                        readNextCharacter();
                        readNextCharacter();
                        break;
                    }
                    value += Symbol;
                    readNextCharacter();
                }
                if (acceptedComment)
                {
                    token.Value = value;
                    token.Length = value.Length;
                    token.Type = TokenType.COMMENT;
                }
                else
                {
                    token.Type = TokenType.ILLEGAL;
                }
            }
            return token;
        }
        private Token checkTokenType()
        {
            Token token = new Token();
            string value = "";
            while (Char.IsLetterOrDigit(Symbol) || Symbol == '_')
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

            // Regex pattern to recognize numbers
            string numberPattern = @"[-+]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][-+]?\d+)?";

            while (Symbol != '\n' && Symbol != ' ' && Symbol != (char)0)
            {
                value += Symbol;
                readNextCharacter();
            }

            // Match the extracted value against the number pattern
            Match match = Regex.Match(value, numberPattern);
            if (match.Success)
            {
                token.Value = match.Value;
                token.Length = match.Length;
                int startIndex = match.Index;
                // Take the index of the next character after the number
                Position = Position - value.Length + match.Length;
                NextPosition = Position + 1;
                if (Position < Content.Length)
                {
                    Symbol = Content[Position];
                }
                if (token.Value.Contains(".") || token.Value.Contains("e") || token.Value.Contains("E"))
                {
                    token.Type = TokenType.FLOAT_CONSTANT;
                }
                else
                {
                    token.Type = TokenType.INT_CONSTANT;
                }
            }
            else
            {
                token.Value = "Programul nu recunoaste acest tip de input!";
                token.Length = 0;
                token.Type = TokenType.ILLEGAL;
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