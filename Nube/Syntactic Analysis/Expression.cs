using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis.VisitorPattern;

namespace Nube.Syntactic_Analysis
{
    // Expression -> Literal | Unary | Binary | Grouping
    public abstract class Expression
    {
        // abstract method to implement for each particular expression
        public abstract T Accept<T>(IExprVisitor<T> visitor);

        public class Literal : Expression
        {
            public object Value { get; set; }
            public Literal(object value)
            {
                Value = value;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitLiteralExpression(this);
            }
        }
        // Binary -> Expression Operator Expression
        public class Binary : Expression
        {
            public Expression Expr_left { get; set; }
            public Expression Expr_right { get; set; }
            public Token Operator { get; set; }
            public Binary(Expression expr_left, Token _operator, Expression expr_right)
            {
                Expr_left = expr_left;
                Operator = _operator;
                Expr_right = expr_right;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitBinaryExpression(this);
            }
        }
        // Unary -> Operator Expression
        public class Unary : Expression
        {
            public Token Operator { get; set; }
            public Expression Expr_right { get; set; }
            public Unary(Token _operator, Expression expr_right)
            {
                Operator = _operator;
                Expr_right = expr_right;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitUnaryExpression(this);
            }
        }
        // Grouping -> ( Expression )
        public class Grouping : Expression
        {
            public Expression Expression { get; set; }
            public Grouping(Expression expression)
            {
                Expression = expression;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitGroupingExpression(this);
            }
        }
        // Assign -> Name = Value
        public class Assign : Expression
        {
            public Token Name { get; set; }
            public Expression Value { get; set; }
            public Assign(Token name, Expression value)
            {
                Name = name;
                Value = value;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitAssignExpression(this);
            }

        }
        public class Variable : Expression
        {
            public Token Name { get; set; }
            public Variable(Token name)
            {
                Name = name;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitVariableExpression(this);
            }
        }
        public class Logical : Expression
        {
            public Expression Expr_left { get; set; }
            public Token Operator { get; set; }
            public Expression Expr_right { get; set; }
            public Logical(Expression expr_left, Token _operator, Expression expr_right)
            {
                Expr_left = expr_left;
                Expr_right = expr_right;
                Operator = _operator;
            }
            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.visitLogicalExpression(this);
            }
        }
    }
}
