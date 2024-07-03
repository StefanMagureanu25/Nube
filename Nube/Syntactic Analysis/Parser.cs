using Nube.Errors;
using Nube.LexicalAnalysis;
using Nube.Typo_Correction;

namespace Nube.Syntactic_Analysis
{
    public class Parser
    {
        #region Parser properties
        public LevenshteinAutomaton LevenshteinAutomaton { get; set; } = new LevenshteinAutomaton();
        public List<Token> Tokens { get; }
        public int CurrentPosition { get; set; }
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
            CurrentPosition = 0;
        }
        #endregion

        #region Parser method helpers
        private Token peek()
        {
            return Tokens[CurrentPosition];
        }
        private Token previous()
        {
            return Tokens[CurrentPosition - 1];
        }
        private Token next()
        {
            if (Tokens[CurrentPosition + 1].Type != TokenType.EOF)
            {
                return Tokens[CurrentPosition + 1];
            }
            return null;
        }
        private bool checkFinal()
        {
            return peek().Type == TokenType.EOF;
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
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Statements checker
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
        #endregion

        #region Parser method
        public List<Statement> parse()
        {
            List<Statement> statements = new List<Statement>();
            while (!checkFinal())
            {
                statements.Add(declaration());
            }
            return statements;
        }
        #endregion

        #region Declarations region
        private Statement declaration()
        {
            try
            {
                if (matchTokenType(new List<TokenType> { TokenType.IDENTIFIER }) && next().Type == TokenType.DECLARATIVE)
                {
                    return varDeclaration();
                }
                return statement();
            }
            catch (ParseError e)
            {
                throw;
            }
        }

        private Statement varDeclaration()
        {
            Token variableName = peek();
            readNextToken();
            Expression value = null;
            checkError(TokenType.DECLARATIVE, "Expect :: in variable declaration!");
            if (matchTokenType(new List<TokenType> { TokenType.INTEGER, TokenType.VAR, TokenType.STRING,
                TokenType.CHAR, TokenType.BOOLEAN, TokenType.REAL, TokenType.NATURAL }))
            {
                readNextToken();
                checkError(TokenType.EQUAL, "Expect '=' in assigning a value");
                value = expression();
            }
            else
            {
                List<string> elements = new List<string> { "var", "string", "integer", "boolean", "natural", "real", "char" };
                object wordModified = LevenshteinAutomaton.CanEdit(peek().Value.ToString(), elements);
                if (wordModified != null)
                {
                    Console.WriteLine($"WARNING: You wrote {peek().Value} instead of {wordModified}." +
                        $" It will be modified automatically!");
                    peek().Value = wordModified;
                    readNextToken();
                    checkError(TokenType.EQUAL, "Expect '=' in assigning a value");
                    value = expression();
                }
                else
                {
                    throw new ParseError(peek(), $"There is no type of {peek().Value}");
                }
            }
            checkError(TokenType.SEMICOLON, $"Expect ; at the end of variable on line {variableName.Line}!");
            return new Statement.Var(variableName, value);
        }
        #endregion
        /*
            Statements grammar:
                statements -> printStatement | expressionStatement | forStatement | whileStatement | ifStatement
         */
        #region Statements Region 
        private Statement statement()
        {
            if (checkToken(TokenType.PRINT))
            {
                readNextToken();
                return printStatement();
            }
            else
            {
                List<string> elements = new List<string> { "print" };
                object wordModified = LevenshteinAutomaton.CanEdit(peek().Value.ToString(), elements);
                if (wordModified != null)
                {
                    Console.WriteLine($"WARNING: You wrote {peek().Value} instead of {wordModified}." +
                        $" It will be modified automatically!");
                    readNextToken();
                    return printStatement();
                }
            }
            if (checkToken(TokenType.IF))
            {
                readNextToken();
                return ifStatement();
            }
            if (checkToken(TokenType.WHILE))
            {
                readNextToken();
                return whileStatement();
            }
            else
            {
                List<string> elements = new List<string> { "while" };
                object wordModified = LevenshteinAutomaton.CanEdit(peek().Value.ToString(), elements);
                if (wordModified != null)
                {
                    Console.WriteLine($"WARNING: You wrote {peek().Value} instead of {wordModified}." +
                        $" It will be modified automatically!");
                    readNextToken();
                    return whileStatement();
                }
            }
            if (checkToken(TokenType.FOR))
            {
                readNextToken();
                return forStatement();
            }
            if (checkToken(TokenType.LBRACE))
            {
                readNextToken();
                return new Statement.Block(block());
            }
            return expressionStatement();
        }

        private Statement printStatement()
        {
            Expression expr = expression();
            checkError(TokenType.SEMICOLON, $"Expect ; after value");
            return new Statement.Print(expr);
        }

        private Statement expressionStatement()
        {
            Expression expr = expression();
            checkError(TokenType.SEMICOLON, "Expect ; after expression.");
            return new Statement.Expr(expr);
        }


        private List<Statement> block()
        {
            List<Statement> statements = new List<Statement>();
            while (!matchTokenType(new List<TokenType> { TokenType.RBRACE }) && !checkFinal())
            {
                statements.Add(declaration());
            }
            checkError(TokenType.RBRACE, "} was expected at the end of the block!");
            return statements;
        }

        private Statement ifStatement()
        {
            Expression condition = expression();
            checkError(TokenType.LBRACE, "Need to put '{' after if condition");
            List<Statement> then = block();
            List<Statement> _else = null;
            if (checkToken(TokenType.ELSE))
            {
                readNextToken();
                checkError(TokenType.LBRACE, "Need to put '{' after else");
                _else = block();
            }
            return new Statement.If(condition, then, _else);
        }

        private Statement whileStatement()
        {
            Expression condition = expression();
            checkError(TokenType.LBRACE, "Need to put '{' after while condition");
            List<Statement> body = block();

            return new Statement.While(condition, body);
        }

        private Statement forStatement()
        {
            Statement declaration;
            if (checkToken(TokenType.SEMICOLON))
            {
                declaration = null;
            }
            else
            {
                declaration = varDeclaration();
            }
            Expression condition = null;
            if (!checkToken(TokenType.SEMICOLON))
            {
                condition = expression();
            }
            checkError(TokenType.STEP, "Need to put 'step' after right parenthesis in for loop!");
            Expression incrementValue = expression();
            checkError(TokenType.LBRACE, "Need to put '{' after for loop");
            List<Statement> body = block();
            return new Statement.For(condition, body, declaration, incrementValue);
        }
        #endregion

        /*
            Expressions grammar:
                expression  -> equal
                equal       -> compare ( ( "!=" | "==" ) compare )* ;
                compare     -> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
                term        -> factor ( ( "-" | "+" ) factor )* ;
                factor      -> unary ( ( "/" | "*" ) unary )* ;
                unary       -> ( "!" | "-" ) unary | primary ;
                primary     -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" ;
         */
        #region Expression operations

        private Expression assignment()
        {
            int currentPosition = CurrentPosition;
            Expression expr = or();
            int currentPositionAfter = CurrentPosition;
            CurrentPosition = currentPosition + 1;
            if (checkToken(TokenType.EQUAL))
            {
                Token equalToken = peek();
                readNextToken();
                Expression value = assignment();
                if (expr is Expression.Binary)
                {
                    Expression.Variable expr_left = (Expression.Variable)((Expression.Binary)expr).Expr_left;
                    Token name = expr_left.Name;
                    return new Expression.Assign(name, value);
                }
                throw new RuntimeError(equalToken, "Invalid assignment");

            }
            else
            {
                CurrentPosition = currentPositionAfter;
            }
            return expr;
        }
        private Expression expression()
        {
            return assignment();
        }
        private Expression equal()
        {
            Expression lhs = compare();
            List<TokenType> equalityTokens = new List<TokenType> { TokenType.EQUAL, TokenType.EQUAL_EQUAL };
            while (matchTokenType(equalityTokens))
            {
                readNextToken();
                Token _operator = previous();
                Expression rhs = compare();
                lhs = new Expression.Binary(lhs, _operator, rhs);
            }
            return lhs;
        }

        private Expression or()
        {
            Expression expr = and();
            while (checkToken(TokenType.OR))
            {
                readNextToken();
                Token _operator = previous();
                Expression rhs = and();
                expr = new Expression.Logical(expr, _operator, rhs);
            }
            return expr;
        }
        private Expression and()
        {
            Expression expr = equal();
            while (checkToken(TokenType.AND))
            {
                readNextToken();
                Token _operator = previous();
                Expression rhs = equal();
                expr = new Expression.Logical(expr, _operator, rhs);
            }
            return expr;
        }
        private Expression compare()
        {
            Expression lhs = term();
            List<TokenType> compareTokens = new List<TokenType> { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL };
            while (matchTokenType(compareTokens))
            {
                readNextToken();
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
                readNextToken();
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
                readNextToken();
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
                readNextToken();
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
                readNextToken();
                return new Expression.Literal(true);
            }
            if (matchTokenType(new List<TokenType> { TokenType.FALSE }))
            {
                readNextToken();
                return new Expression.Literal(false);
            }
            if (matchTokenType(new List<TokenType> { TokenType.NULL }))
            {
                readNextToken();
                return new Expression.Literal(null);
            }
            if (matchTokenType(new List<TokenType> { TokenType.STRING, TokenType.INTEGER, TokenType.NATURAL, TokenType.REAL }))
            {
                readNextToken();
                return new Expression.Literal(previous().Value);
            }
            if (matchTokenType(new List<TokenType> { TokenType.IDENTIFIER }))
            {
                readNextToken();
                return new Expression.Variable(previous());
            }
            if (matchTokenType(new List<TokenType> { TokenType.LPAREN }))
            {
                readNextToken();
                Expression _expression = expression();
                checkError(TokenType.RPAREN, "Expect ')' at the end.");
                return new Expression.Grouping(_expression);
            }
            throw new ParseError(peek(), "An expression was expected here!");
        }
        #endregion
    }
}
