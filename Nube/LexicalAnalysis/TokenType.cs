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
            Keywords.VOID
        };
        public static class Keywords
        {

            public const string INT = "integer";
            public const string FLOAT = "rational";
            public const string MAIN = "main";
            public const string STRING = "string";
            public const string BOOL = "boolean";
            public const string FUNCTION = "fn";
            public const string IMPORT = "import";
            public const string FOR = "for";
            public const string WHILE = "while";
            public const string BREAK = "stop";
            public const string TRUE = "true";
            public const string FALSE = "false";
            public const string VOID = "nothing";
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
            public const string EQUAL = "=";
            public const string GREATER = ">";
            public const string LESS = "<";
            public const string MULTIPLY = "*";
            public const string DIVIDE = "/";
            public const string MOD = "%";
        }
        public const string IDENT = "IDENTIFIER";
        public const string INT_CONSTANT = "INT_CONSTANT";
        public const string FLOAT_CONSTANT = "FLOAT_CONSTANT";
        public const string COMMENT = "COMMENT";
        public const string ILLEGAL = "illegal";
        public const string EOF = "null";
    }
}
