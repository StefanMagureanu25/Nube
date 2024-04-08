using System.Data;

namespace Nube.LexicalAnalysis
{
    public static class TokenType
    {
        public static string[] keywords = {
        "abstract", "as" ,"base","bool","break",
        "byte","case","catch","char","checked",
        "class","const","continue","decimal","default",
        "delegate", "do","double","else","enum",
        "event","explicit","extern","false","finally",
        "fixed","float","for","foreach","goto",
        "if","implicit","in","int","interface",
        "internal","is","lock","long","namespace",
        "new","null","object","operator","out",
        "override","params","private","protected","public",
        "readonly","ref","return","sbyte","sealed",
        "short","sizeof","stackalloc","static","string",
        "struct","switch","this", "throw","true",
        "try","typeof","uint","ulong","unchecked",
        "unsafe","ushort","using","virtual","void",
        "volatile","while", "Main"
    };

        public static class Keywords
        {
            public const string STRING = "key_word";
            public const string BOOL = "key_word";
            public const string INT = "key_word";
            public const string CHAR = "key_word";
            public const string FLOAT = "key_word";
            public const string MAIN = "key_word";
            public const string FUNCTION = "key_word";
            public const string IMPORT = "key_word";
            public const string IF = "key_word";
            public const string FOR = "key_word";
            public const string WHILE = "key_word";
            public const string BREAK = "key_word";
            public const string TRUE = "key_word";
            public const string FALSE = "key_word";
            public const string VOID = "key_word";
            public const string RETURN = "key_word";
        }
        public static class Delimiters
        {
            public const string DOT = "delimitator";
            public const string COMMA = "delimitator";
            public const string LPAREN = "delimitator";
            public const string RPAREN = "delimitator";
            public const string COLON = "delimitator";
            public const string LBRACE = "delimitator";
            public const string RBRACE = "delimitator";
            public const string SEMICOLON = "delimitator";
            public const string LBRACKET = "delimitator";
            public const string RBRACKET = "delimitator";
            public const string QUOTATION = "delimitator";
            public const string APOSTROPHE = "delimitator";
        }
        public static class Operators
        {
            public const string PLUS = "operator";
            public const string MINUS = "operator";
            public const string MULTIPLY = "operator";
            public const string DIVIDE = "operator";
            public const string MOD = "operator";

            public const string EQUAL = "operator";
            public const string GREATER = "operator";
            public const string LESS = "operator";
            public const string NOT = "operator";

            public const string PLUS_PLUS = "operator";
            public const string MINUS_MINUS = "operator";
            public const string LEFT_SHIFT = "operator";
            public const string RIGHT_SHIFT = "operator";
            public const string AND = "operator";
            public const string OR = "operator";

            public const string BITWISE_AND = "operator";
            public const string BITWISE_XOR = "operator";
            public const string BITWISE_OR = "operator";


            public const string PLUS_EQUAL = "operator";
            public const string MINUS_EQUAL = "operator";
            public const string MULTIPLY_EQUAL = "operator";
            public const string DIVIDE_EQUAL = "operator";
            public const string MOD_EQUAL = "operator";

            public const string EQUAL_EQUAL = "operator";
            public const string LESS_EQUAL = "operator";
            public const string GREATER_EQUAL = "operator";
            public const string NOT_EQUAL = "operator";

            public const string BITWISE_AND_EQUAL = "operator";
            public const string BITWISE_XOR_EQUAL = "operator";
            public const string BITWISE_OR_EQUAL = "operator";

        }
        public const string KEYWORD = "key_word";
        public const string IDENT = "identifier";
        public const string INT_CONSTANT = "int_constant";
        public const string FLOAT_CONSTANT = "float_constant";
        public const string CHAR_CONSTANT = "char_constant";
        public const string STRING_CONSTANT = "string_constant";
        public const string COMMENT = "comment";
        public const string ILLEGAL = "illegal";
        public const string EOF = "null";
    }
}
