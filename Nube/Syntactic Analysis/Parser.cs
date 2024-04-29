using Nube.Errors;
using Nube.LexicalAnalysis;

namespace Nube.Syntactic_Analysis
{
    public class Parser
    {
        public List<Token> Tokens { get; }
        public int CurrentPosition { get; set; }
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
            CurrentPosition = 0;
        }
        private Token peek()
        {
            return Tokens[CurrentPosition];
        }
        private Token previous()
        {
            return Tokens[CurrentPosition - 1];
        }
        private bool checkFinal()
        {
            return peek().Type == TokenType.EOF;
        }
        private void jumpToNextStatement()
        {
            readNextToken();
            while (!checkFinal())
            {
                if (previous().Type == TokenType.SEMICOLON)
                {
                    return;
                }
                switch (peek().Type)
                {
                    case TokenType.FOR:
                    case TokenType.FN:
                    case TokenType.WHILE:
                    case TokenType.IF:
                    case TokenType.RETURN:
                    case TokenType.NATURAL:
                    case TokenType.INTEGER:
                    case TokenType.REAL:
                    case TokenType.STRING:
                    case TokenType.CHAR:
                        return;
                    default:
                        readNextToken();
                        break;
                }
            }
        }
        private Token checkError(TokenType tokenType, string message)
        {
            if (checkToken(tokenType))
            {
                return readNextToken();
            }
            throw new ParseError(peek(), message);
        }
        private Token? readNextToken()
        {
            CurrentPosition++;
            if (!checkFinal())
            {
                return peek();
            }
            return null;
        }
        private bool checkToken(TokenType tokenType)
        {
            return peek().Type == tokenType;
        }
        private bool matchTokenType(List<TokenType> tokens)
        {
            foreach (TokenType token in tokens)
            {
                if (checkToken(token))
                {
                    readNextToken();
                    return true;
                }
            }
            return false;
        }
        public Expression parse()
        {
            try
            {
                return expression();
            }
            catch (ParseError e)
            {
                return null;
            }
        }
        private Expression expression()
        {
            return equal();
        }
        private Expression equal()
        {
            Expression lhs = compare();
            List<TokenType> equalityTokens = new List<TokenType> { TokenType.EQUAL, TokenType.EQUAL_EQUAL };
            while (matchTokenType(equalityTokens))
            {
                Token _operator = previous();
                Expression rhs = compare();
                lhs = new Expression.Binary(lhs, _operator, rhs);
            }
            return lhs;
        }
        private Expression compare()
        {
            Expression lhs = term();
            List<TokenType> compareTokens = new List<TokenType> { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL };
            while (matchTokenType(compareTokens))
            {
                Token _operator = previous();
                Expression rhs = term();
                lhs = new Expression.Binary(lhs, _operator, rhs);
            }
            return lhs;
        }
        private Expression term()
        {
            Expression lhs = factor();
            List<TokenType> plusMinusSigns = new List<TokenType> { TokenType.PLUS, TokenType.MINUS };
            while (matchTokenType(plusMinusSigns))
            {
                Token _operator = previous();
                Expression rhs = factor();
                lhs = new Expression.Binary(lhs, _operator, rhs);
            }
            return lhs;
        }
        private Expression factor()
        {
            Expression lhs = unary();
            List<TokenType> factorOperators = new List<TokenType> { TokenType.DIVIDE, TokenType.MULTIPLY };
            while (matchTokenType(factorOperators))
            {
                Token _operator = previous();
                Expression rhs = unary();
                lhs = new Expression.Binary(lhs, _operator, rhs);
            }
            return lhs;
        }
        private Expression unary()
        {
            List<TokenType> unaryOperators = new List<TokenType> { TokenType.NOT, TokenType.MINUS };
            while (matchTokenType(unaryOperators))
            {
                Token _operator = previous();
                Expression rhs = unary();
                return new Expression.Unary(_operator, rhs);
            }
            return primary();
        }
        private Expression primary()
        {
            if (matchTokenType(new List<TokenType> { TokenType.TRUE }))
            {
                return new Expression.Literal(true);
            }
            if (matchTokenType(new List<TokenType> { TokenType.FALSE }))
            {
                return new Expression.Literal(false);
            }
            if (matchTokenType(new List<TokenType> { TokenType.NULL }))
            {
                return new Expression.Literal(null);
            }
            if (matchTokenType(new List<TokenType> { TokenType.STRING, TokenType.INTEGER, TokenType.NATURAL, TokenType.REAL }))
            {
                return new Expression.Literal(previous().Value);
            }
            if (matchTokenType(new List<TokenType> { TokenType.LPAREN }))
            {
                Expression _expression = expression();
                if (peek().Type != TokenType.RPAREN)
                {
                    // eroare, pentru ca noi asteptam o paranteza ')'
                }
                return new Expression.Grouping(_expression);
            }
            throw new ParseError(peek(), "An expression was expected here!");
        }
    }
}
