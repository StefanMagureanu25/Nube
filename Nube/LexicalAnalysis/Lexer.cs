using System.Text.RegularExpressions;

namespace Nube.LexicalAnalysis
{
    public class Lexer
    {
        #region Properties & Constructors
        private static readonly Dictionary<string, TokenType> _keywords = [];
        private bool isComment = false;
        public string? Content { get; set; }
        //Position where the finite automata(lexer) is situated
        public int Position { get; set; }
        //Actual position for the character we read
        public int NextPosition { get; set; }
        public char Symbol { get; set; }
        public int Line { get; set; } = 1;
        public Lexer(string content)
        {
            Content = content;
        }
        public Lexer()
        {
            Position = -1;
            NextPosition = 0;
            addKeywords();
        }
        #endregion

        #region Helpful methods for finite state automaton traversal
        public void ResetPosition()
        {
            Position = -1;
            NextPosition = 0;
        }
        public void AddLine()
        {
            Line++;
        }
        public void NextLine()
        {
            ResetPosition();
            AddLine();
        }
        private bool stepOver(char c)
        {
            if (c == '\n' || c == '\0' || c == '\t' || c == ' ' || c == '\r')
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Extra methods for defining my tokens' rules
        private void addKeywords()
        {
            _keywords.Add("const", TokenType.CONST);
            _keywords.Add("string", TokenType.STRING);
            _keywords.Add("boolean", TokenType.BOOLEAN);
            _keywords.Add("integer", TokenType.INTEGER);
            _keywords.Add("char", TokenType.CHAR);
            _keywords.Add("real", TokenType.REAL);
            _keywords.Add("natural", TokenType.NATURAL);
            _keywords.Add("nothing", TokenType.NOTHING);
            _keywords.Add("null", TokenType.NULL);

            _keywords.Add("main", TokenType.MAIN);
            _keywords.Add("fn", TokenType.FN);
            _keywords.Add("import", TokenType.IMPORT);

            _keywords.Add("if", TokenType.IF);
            _keywords.Add("for", TokenType.FOR);
            _keywords.Add("while", TokenType.WHILE);

            _keywords.Add("stop", TokenType.STOP);
            _keywords.Add("continue", TokenType.CONTINUE);

            _keywords.Add("true", TokenType.TRUE);
            _keywords.Add("false", TokenType.FALSE);

            _keywords.Add("to", TokenType.TO);
            _keywords.Add("step", TokenType.STEP);

            _keywords.Add("return", TokenType.RETURN);
        }
        private TokenType checkKeyword(string value)
        {
            TokenType tokenType;
            if (_keywords.TryGetValue(value, out tokenType))
                return tokenType;

            if (value == "or")
            {
                tokenType = TokenType.OR;
            }
            else if (value == "and")
            {
                tokenType = TokenType.AND;
            }
            else
            {
                tokenType = TokenType.IDENTIFIER;
            }
            return tokenType;
        }
        private bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }
        private bool isLetter(char ch)
        {
            return (ch == '_') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }
        private bool isAlphaNumeric(char ch)
        {
            return isDigit(ch) || isLetter(ch);
        }
        #endregion

        #region Token recognition
        private Token takeString()
        {
            Token token = new Token();
            bool acceptedString = false;
            // check until EOF
            while (Symbol != (char)0)
            {
                token.Value += Symbol;
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
                token.Length = token.Value.Length;
                token.Type = TokenType.STRING;
            }
            return token;
        }
        private Token takeChar()
        {
            Token token = new Token();
            bool acceptedChar = false;
            while (Symbol != (char)0)
            {
                token.Value += Symbol;
                if (token.Value.Length > 1)
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
                token.Length = token.Value.Length;
                token.Type = TokenType.CHAR;
            }
            return token;
        }
        private void consumeComment()
        {
            // single line comment
            if (Symbol == '/')
            {
                while (Symbol != '\0')
                {
                    readNextCharacter();
                }
                isComment = false;
            }
            // multi-line comment
            else if (Symbol == '*' || isComment == true)
            {
                readNextCharacter();
                // check until EOF
                while (Symbol != (char)0)
                {
                    if (Symbol == '/' && Content[Position - 1] == '*')
                    {
                        isComment = false;
                        // I have to jump over '/' and go to the next character
                        readNextCharacter();
                        break;
                    }
                    readNextCharacter();
                }
            }
        }
        private Token checkTokenType()
        {
            Token token = new Token();
            while (isAlphaNumeric(Symbol))
            {
                token.Value += Symbol;
                readNextCharacter();
            }
            token.Length = token.Value.Length;
            token.Type = checkKeyword(token.Value);
            return token;
        }
        private Token checkNumber()
        {
            Token token = new Token();
            string value = "";

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
                // Take the index of the next character after the number
                Position = Position - value.Length + match.Length;
                NextPosition = Position + 1;
                if (Position < Content.Length)
                {
                    Symbol = Content[Position];
                }
                if (token.Value.Contains(".") || token.Value.Contains("e") || token.Value.Contains("E"))
                {
                    token.Type = TokenType.REAL;
                }
                else if (token.Value.Contains("-") == false)
                {
                    token.Type = TokenType.NATURAL;
                }
                else
                {
                    token.Type = TokenType.INTEGER;
                }
            }
            else
            {
                token.Value = "Programul nu recunoaste acest tip de input!";
                token.Length = 0;
                token.Type = TokenType.INVALID;
            }
            return token;
        }
        #endregion

        #region Automaton traversal
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
        private Token? NextToken()
        {
            if (isComment == true)
            {
                consumeComment();
                return null;
            }
            Token token = new Token();
            switch (Symbol)
            {
                #region Delimiters Verification
                case '.':
                    if (Char.IsDigit(Content[NextPosition]))
                    {
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = checkNumber();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.DOT, ".", 1);
                        readNextCharacter();
                    }
                    break;
                case ',':
                    (token.Type, token.Value, token.Length) = (TokenType.COMMA, ",", 1);
                    readNextCharacter();
                    break;
                case '(':
                    (token.Type, token.Value, token.Length) = (TokenType.LPAREN, "(", 1);
                    readNextCharacter();
                    break;
                case ')':
                    (token.Type, token.Value, token.Length) = (TokenType.RPAREN, ")", 1);
                    readNextCharacter();
                    break;
                case ':':
                    readNextCharacter();
                    if (Symbol == ':')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.DECLARATIVE, "::", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.COLON, ":", 1);
                    }
                    break;
                case '{':
                    (token.Type, token.Value, token.Length) = (TokenType.LBRACE, "{", 1);
                    readNextCharacter();
                    break;
                case '}':
                    (token.Type, token.Value, token.Length) = (TokenType.RBRACE, "}", 1);
                    readNextCharacter();
                    break;
                case ';':
                    (token.Type, token.Value, token.Length) = (TokenType.SEMICOLON, ";", 1);
                    readNextCharacter();
                    break;
                case '[':
                    (token.Type, token.Value, token.Length) = (TokenType.LBRACKET, "[", 1);
                    readNextCharacter();
                    break;
                case ']':
                    (token.Type, token.Value, token.Length) = (TokenType.RBRACKET, "]", 1);
                    readNextCharacter();
                    break;
                case '"':
                    readNextCharacter();
                    if (Symbol != '"')
                    {
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = takeString();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.STRING, "", 0);
                        readNextCharacter();
                    }
                    break;
                case '\'':
                    readNextCharacter();
                    if (Symbol != '\'')
                    {
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = takeChar();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.CHAR, "", 0);
                        readNextCharacter();
                    }
                    break;
                #endregion

                #region Operators Verification
                case '!':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.NOT_EQUAL, "!=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.NOT, "!", 1);
                    }
                    break;
                case '&':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_AND_EQUAL, "&=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_AND, "&", 1);
                    }
                    break;
                case '|':
                    readNextCharacter();
                    if (Symbol == '|')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.OR, "||", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_OR_EQUAL, "|=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_OR, "|", 1);
                    }
                    break;
                case '^':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_XOR_EQUAL, "^=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.BITWISE_XOR, "^", 1);
                    }
                    break;
                case '+':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.PLUS_EQUAL, "+=", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '+')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.PLUS_PLUS, "++", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.PLUS, "+", 1);
                    }
                    break;
                case '-':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MINUS_EQUAL, "-=", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '-')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MINUS_MINUS, "--", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MINUS, "-", 1);
                    }
                    break;
                case '=':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.EQUAL_EQUAL, "==", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.EQUAL, "=", 1);
                    }
                    break;
                case '>':
                    readNextCharacter();
                    if (Symbol == '>')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.RIGHT_SHIFT, ">>", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.GREATER_EQUAL, ">=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.GREATER, ">", 1);
                    }
                    break;
                case '<':
                    readNextCharacter();
                    if (Symbol == '<')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.LEFT_SHIFT, "<<", 2);
                        readNextCharacter();
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.LESS_EQUAL, "<=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.LESS, "<", 1);
                    }
                    break;
                case '*':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MULTIPLY_EQUAL, "*=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MULTIPLY, "*", 1);
                    }
                    break;
                case '/':
                    readNextCharacter();
                    if (Symbol == '*' || Symbol == '/')
                    {
                        isComment = true;
                        consumeComment();
                        return null;
                    }
                    else if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.DIVIDE_EQUAL, "/=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.DIVIDE, "/", 1);
                    }
                    break;
                case '%':
                    readNextCharacter();
                    if (Symbol == '=')
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MOD_EQUAL, "/=", 2);
                        readNextCharacter();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length) = (TokenType.MOD, "%", 1);
                    }
                    break;
                #endregion

                #region Keywords and Identifiers
                default:
                    if (isLetter(Symbol))
                    {
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = checkTokenType();
                    }
                    else
                    {
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = checkNumber();
                    }
                    break;
                    #endregion
            }
            if (token.Value != null)
            {
                token.Position = Position - token.Value.Length + 1;
                token.Line = Line;
            }
            return token;
        }
        public void Analyze(string content)
        {
            readNextCharacter();
            while (Symbol != (char)0)
            {
                if (stepOver(Symbol))
                {
                    readNextCharacter();
                    continue;
                }
                Token token = NextToken();
                if (token != null)
                {
                    Console.WriteLine(token.ToString());
                }
            }
        }
        #endregion
    }
}