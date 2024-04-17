using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Nube.LexicalAnalysis
{
    public class Lexer
    {
        public static string[] keywords =
        {
            "string",
            "boolean",
            "integer",
            "char",
            "real",
            "natural",
            "nothing",

            "main",
            "fn",
            "import",

            "if",
            "for",
            "while",

            "stop",
            "continue",

            "true",
            "false",

            "or",
            "and",

            "return"
        };
        public string Content { get; set; }
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
        }
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
        // I want to step over \t,\r, ' ', \n, \0 (EOF)
        private bool stepOver(char c)
        {
            if (c == '\n' || c == '\0' || c == '\t' || c == ' ' || c == '\r')
            {
                return true;
            }
            return false;
        }
        private bool checkKeyword(string value)
        {
            foreach (string keyword in keywords)
            {
                if (value == keyword)
                {
                    return true;
                }
            }
            return false;
        }
        private bool isLetter(char ch)
        {
            return (ch == '_') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
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
        private TokenType KeywordType(string keyword)
        {
            switch (keyword)
            {
                case "string":
                    return TokenType.STRING;
                case "boolean":
                    return TokenType.BOOLEAN;
                case "integer":
                    return TokenType.INTEGER;
                case "char":
                    return TokenType.CHAR;
                case "real":
                    return TokenType.REAL;
                case "natural":
                    return TokenType.NATURAL;
                case "nothing":
                    return TokenType.NOTHING;
                case "null":
                    return TokenType.NULL;

                case "main":
                    return TokenType.MAIN;
                case "fn":
                    return TokenType.FN;
                case "import":
                    return TokenType.IMPORT;

                case "if":
                    return TokenType.IF;
                case "for":
                    return TokenType.FOR;
                case "while":
                    return TokenType.WHILE;

                case "stop":
                    return TokenType.STOP;
                case "continue":
                    return TokenType.CONTINUE;

                case "true":
                    return TokenType.TRUE;
                case "false":
                    return TokenType.FALSE;

                case "or":
                    return TokenType.OR;
                case "and":
                    return TokenType.AND;

                case "return":
                    return TokenType.RETURN;

                case "to":
                    return TokenType.TO;
                case "step":
                    return TokenType.STEP;

                default:
                    return TokenType.INVALID;
            }
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
                    (token.Type, token.Value, token.Length) = (TokenType.COLON, ":", 1);
                    readNextCharacter();
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
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = TakeString();
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
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = TakeChar();
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
                        (token.Type, token.Value, token.Length, token.Line, token.Position) = TakeComment();
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
            }
            token.Position = Position - token.Value.Length + 1;
            token.Line = Line;
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
                token.Type = TokenType.STRING;
            }
            else
            {
                token.Type = TokenType.INVALID;
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
                token.Type = TokenType.CHAR;
            }
            else
            {
                token.Type = TokenType.INVALID;
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
                    token.Type = TokenType.INVALID;
                }
            }
            return token;
        }
        private Token checkTokenType()
        {
            TokenType tokenType;
            Token token = new Token();
            string value = "";
            while (Char.IsLetterOrDigit(Symbol) || Symbol == '_')
            {
                value += Symbol;
                readNextCharacter();
            }
            token.Value = value;
            token.Length = value.Length;
            bool isKeyword = checkKeyword(value);
            if (isKeyword == true)
            {
                tokenType = KeywordType(value);
            }
            else
            {
                tokenType = TokenType.IDENTIFIER;
            }
            token.Type = tokenType;
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
                Console.WriteLine(token.ToString());
            }
        }
    }
}