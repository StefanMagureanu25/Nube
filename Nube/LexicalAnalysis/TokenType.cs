namespace Nube.LexicalAnalysis
{
    public enum TokenType
    {

        #region Keywords
        MAIN = 1000,
        FN,
        IMPORT,
        IF,
        FOR,
        WHILE,
        STOP,
        TRUE,
        FALSE,
        NOTHING,
        RETURN,
        CONTINUE,
        TO,
        STEP,
        NULL,
        #endregion

        #region Delimiters
        DOT, // .
        COMMA, // ,
        LPAREN, // (
        RPAREN,
        COLON, // :
        LBRACE,  // {
        RBRACE,
        SEMICOLON, // ;
        LBRACKET, // [
        RBRACKET,
        QUOTATION,// "
        APOSTROPHE,
        // I use :: for variable declarations: Ex: a :: integer = 3;
        DECLARATIVE, // ::
        #endregion

        #region Operators
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        MOD,

        EQUAL,
        GREATER,
        LESS,
        NOT,

        PLUS_PLUS,
        MINUS_MINUS,
        LEFT_SHIFT,
        RIGHT_SHIFT,
        AND, // and
        OR, // or

        // Operatori pe biti
        BITWISE_AND, // &
        BITWISE_XOR, // ^
        BITWISE_OR, // |


        PLUS_EQUAL,
        MINUS_EQUAL,
        MULTIPLY_EQUAL,
        DIVIDE_EQUAL,
        MOD_EQUAL,
        LEFT_SHIFT_EQUAL,
        RIGHT_SHIFT_EQUAL,

        EQUAL_EQUAL,
        LESS_EQUAL,
        GREATER_EQUAL,
        NOT_EQUAL,

        BITWISE_AND_EQUAL,
        BITWISE_XOR_EQUAL,
        BITWISE_OR_EQUAL,
        #endregion

        #region Literals
        IDENTIFIER,
        STRING,
        INTEGER,
        BOOLEAN,
        REAL,
        NATURAL,
        CHAR,
        CONST,
        #endregion

        #region Extra
        INVALID,
        EOF
        #endregion
    }
}
