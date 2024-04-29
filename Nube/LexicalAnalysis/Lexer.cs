using System.Text.RegularExpressions;

namespace Nube.LexicalAnalysis
{
    public static class Lexer
    {
        #region Properties & Constructors
        public static List<Token> Tokens = new List<Token>();
        private static readonly Dictionary<string, TokenType> _keywords = [];
        private static bool isComment = false;
        public static string? Content { get; set; }
        //Position where the finite automata(lexer) is situated
        public static int Position { get; set; } = -1;
        //Actual position for the character we read
        public static int NextPosition { get; set; } = 0;
        public static char Symbol { get; set; }
        public static int Line { get; set; } = 1;
        #endregion

        #region Helpful methods for finite state automaton traversal
        public static void ResetPosition()
        {
            Position = -1;
            NextPosition = 0;
        }
        public static void AddLine()
        {
            Line++;
        }
        public static void NextLine()
        {
            ResetPosition();
            AddLine();
        }
        private static bool stepOver(char c)
        {
            if (c == '\n' || c == '\0' || c == '\t' || c == ' ' || c == '\r')
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Extra methods for defining my tokens' rules
        private static void addKeywords()
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
        private static TokenType checkKeyword(string value)
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
        private static bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }
        private static bool isLetter(char ch)
        {
            return (ch == '_') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }
        private static bool isAlphaNumeric(char ch)
        {
            return isDigit(ch) || isLetter(ch);
        }
        #endregion

        #region Token recognition
        private static Token takeString()
        {
            Token token = new Token();
            string tokenTemp = token.Value.ToString();
            bool acceptedString = false;
            // check until EOF
            while (Symbol != (char)0)
            {
                tokenTemp += Symbol;
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
                token.Value = tokenTemp;
                token.Length = token.Value.ToString().Length;
                token.Type = TokenType.STRING;
            }
            return token;
        }
        private static Token takeChar()
        {
            Token token = new Token();
            string tokenTemp = token.Value.ToString();
            bool acceptedChar = false;
            while (Symbol != (char)0)
            {
                tokenTemp += Symbol;
                if (token.Value.ToString().Length > 1)
                {
                    // Char can't have length bigger than 1
                    token.Value = tokenTemp;
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
                token.Length = token.Value.ToString().Length;
                token.Type = TokenType.CHAR;
            }
            return token;
        }
        private static void consumeComment()
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
        private static Token checkTokenType()
        {
            Token token = new Token();
            string tokenTemp = token.Value.ToString();
            while (isAlphaNumeric(Symbol))
            {
                tokenTemp += Symbol;
                readNextCharacter();
            }
            token.Value = tokenTemp;
            token.Length = token.Value.ToString().Length;
            token.Type = checkKeyword(token.Value.ToString());
            return token;
        }
        private static Token checkNumber()
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
                if (token.Value.ToString().Contains(".") || token.Value.ToString().Contains("e") || token.Value.ToString().Contains("E"))
                {
                    token.Type = TokenType.REAL;
                    Tokens.Add(new Token(token.Type, Convert.ToDouble(token.Value), token.Value.ToString().Length, Line, Position));
                }
                else if (token.Value.ToString().Contains("-") == false)
                {
                    token.Type = TokenType.NATURAL;
                    Tokens.Add(new Token(token.Type, Convert.ToUInt32(token.Value), token.Value.ToString().Length, Line, Position));
                }
                else
                {
                    token.Type = TokenType.INTEGER;
                    Tokens.Add(new Token(token.Type, Convert.ToInt32(token.Value), token.Value.ToString().Length, Line, Position));
                }
            }
            else
            {
                token.Value = null;
                token.Length = 0;
                token.Type = TokenType.INVALID;
            }
            return token;
        }
        #endregion

        #region Automaton traversal
        private static void readNextCharacter()
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
        private static Token? NextToken()
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
                token.Position = Position - token.Value.ToString().Length + 1;
                token.Line = Line;
            }
            return token;
        }
        public static void Analyze(string content)
        {
            if (Line == 1 && NextPosition == 0)
            {
                addKeywords();
            }
            readNextCharacter();
            while (Symbol != (char)0)
            {
                if (stepOver(Symbol))
                {
                    readNextCharacter();
                    continue;
                }
                Token token = NextToken();
                if (token != null && token.Type != TokenType.NATURAL && token.Type != TokenType.REAL && token.Type != TokenType.INTEGER)
                {
                    Tokens.Add(token);
                }
            }
        }
        #endregion
    }
}