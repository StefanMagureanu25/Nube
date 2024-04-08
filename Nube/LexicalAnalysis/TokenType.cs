using System.Data;

namespace Nube.LexicalAnalysis
{
    public static class TokenType
    {
        public static string[] keywords =
        {
            Keywords.INT,
            Keywords.STRING,
            Keywords.FLOAT,
            Keywords.MAIN,
            Keywords.BOOL,
            Keywords.FUNCTION,
            Keywords.IMPORT,
            Keywords.FOR,
            Keywords.WHILE,
            Keywords.BREAK,
            Keywords.TRUE,
            Keywords.FALSE,
            Keywords.VOID,
            Keywords.RETURN,
            Keywords.CHAR,
            Keywords.IF
        };
        public static class Keywords
        {
            public const string STRING = "string";
            public const string BOOL = "boolean";
            public const string INT = "int";  // integer
            public const string CHAR = "char";
            public const string FLOAT = "rational";
            public const string MAIN = "main";
            public const string FUNCTION = "fn";
            public const string IMPORT = "import";
            public const string IF = "if";
            public const string FOR = "for";
            public const string WHILE = "while";
            public const string BREAK = "stop";
            public const string TRUE = "true";
            public const string FALSE = "false";
            public const string VOID = "void"; // nothing
            public const string RETURN = "return";
        }
        public static class Delimiters
        {
            public const string DOT = ".";
            public const string COMMA = ",";
            public const string LPAREN = "(";
            public const string RPAREN = ")";
            public const string COLON = ":";
            public const string LBRACE = "{";
            public const string RBRACE = "}";
            public const string SEMICOLON = ";";
            public const string LBRACKET = "[";
            public const string RBRACKET = "]";
            public const string QUOTATION = "\"";
            public const string APOSTROPHE = "'";
        }
        public static class Operators
        {
            public const string PLUS = "+";
            public const string MINUS = "-";
            public const string MULTIPLY = "*";
            public const string DIVIDE = "/";
            public const string MOD = "%";

            public const string EQUAL = "=";
            public const string GREATER = ">";
            public const string LESS = "<";
            public const string NOT = "!";

            public const string PLUS_PLUS = "++";
            public const string MINUS_MINUS = "--";
            public const string LEFT_SHIFT = "<<";
            public const string RIGHT_SHIFT = ">>";
            public const string AND = "&&";
            public const string OR = "||";

            public const string BITWISE_AND = "&";
            public const string BITWISE_XOR = "^";
            public const string BITWISE_OR = "|";


            public const string PLUS_EQUAL = "+=";
            public const string MINUS_EQUAL = "-=";
            public const string MULTIPLY_EQUAL = "*=";
            public const string DIVIDE_EQUAL = "/=";
            public const string MOD_EQUAL = "%=";

            public const string EQUAL_EQUAL = "==";
            public const string LESS_EQUAL = "<=";
            public const string GREATER_EQUAL = ">=";
            public const string NOT_EQUAL = "!=";

            public const string BITWISE_AND_EQUAL = "&=";
            public const string BITWISE_XOR_EQUAL = "^=";
            public const string BITWISE_OR_EQUAL = "|=";

        }
        public const string IDENT = "IDENTIFIER";
        public const string INT_CONSTANT = "INT_CONSTANT";
        public const string FLOAT_CONSTANT = "FLOAT_CONSTANT";
        public const string CHAR_CONSTANT = "CHAR_CONSTANT";
        public const string STRING_CONSTANT = "STRING_CONSTANT";
        public const string COMMENT = "COMMENT";
        public const string ILLEGAL = "illegal";
        public const string EOF = "null";
    }
}
