using Nube.Errors;
using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis.VisitorPattern;
using System.Formats.Asn1;
using static Nube.Syntactic_Analysis.Expression;

namespace Nube.Syntactic_Analysis.Interpreter
{
    public class Interpreter : IVisitor<object>
    {
        public void interpret(Expression expression)
        {
            try
            {
                Object value = eval(expression);
                Console.WriteLine(value.ToString());
            }
            catch (RuntimeError ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public object visitBinaryExpression(Binary expression)
        {
            var lhs = eval(expression.Expr_left);
            var rhs = eval(expression.Expr_right);
            switch (expression.Operator.Type)
            {
                case TokenType.PLUS:
                    {
                        if (lhs is uint && rhs is uint)
                        {
                            return (uint)lhs + (uint)rhs;
                        }
                        if (lhs is uint && rhs is int)
                        {
                            return (uint)lhs + (int)rhs;
                        }
                        else if (lhs is uint && rhs is double)
                        {
                            return (uint)lhs + (double)rhs;
                        }
                        else if (lhs is int && rhs is uint)
                        {
                            return (int)lhs + (uint)rhs;
                        }
                        else if (lhs is double && rhs is uint)
                        {
                            return (double)lhs + (uint)rhs;
                        }
                        if (lhs is int && rhs is int)
                        {
                            return (int)lhs + (int)rhs;
                        }
                        else if (lhs is int && rhs is double)
                        {
                            return (int)lhs + (double)rhs;
                        }
                        else if (lhs is double && rhs is int)
                        {
                            return (double)lhs + (int)rhs;
                        }
                        else if (lhs is string && rhs is string)
                        {
                            return (string)lhs + (string)rhs;
                        }
                        throw new RuntimeError(expression.Operator, "The input you entered must be numbers or strings both!"); ;
                    }
                case TokenType.MINUS:
                    {
                        if (lhs is uint && rhs is uint)
                        {
                            return (uint)lhs - (uint)rhs;
                        }
                        if (lhs is uint && rhs is int)
                        {
                            return (uint)lhs - (int)rhs;
                        }
                        else if (lhs is uint && rhs is double)
                        {
                            return (uint)lhs - (double)rhs;
                        }
                        else if (lhs is int && rhs is uint)
                        {
                            return (int)lhs - (uint)rhs;
                        }
                        else if (lhs is double && rhs is uint)
                        {
                            return (double)lhs - (uint)rhs;
                        }
                        if (lhs is int && rhs is int)
                        {
                            return (int)lhs - (int)rhs;
                        }
                        else if (lhs is int && rhs is double)
                        {
                            return (int)lhs - (double)rhs;
                        }
                        else if (lhs is double && rhs is int)
                        {
                            return (double)lhs - (int)rhs;
                        }
                        else if (lhs is double && rhs is double)
                        {
                            return (double)lhs - (double)rhs;
                        }
                        throw new RuntimeError(expression.Operator, "The input you entered must be numbers!");
                    }
                case TokenType.MULTIPLY:
                    {
                        if (lhs is uint && rhs is uint)
                        {
                            return (uint)lhs * (uint)rhs;
                        }
                        if (lhs is uint && rhs is int)
                        {
                            return (uint)lhs * (int)rhs;
                        }
                        else if (lhs is uint && rhs is double)
                        {
                            return (uint)lhs * (double)rhs;
                        }
                        else if (lhs is int && rhs is uint)
                        {
                            return (int)lhs * (uint)rhs;
                        }
                        else if (lhs is double && rhs is uint)
                        {
                            return (double)lhs * (uint)rhs;
                        }
                        if (lhs is int && rhs is int)
                        {
                            return (int)lhs * (int)rhs;
                        }
                        else if (lhs is int && rhs is double)
                        {
                            return (int)lhs * (double)rhs;
                        }
                        else if (lhs is double && rhs is int)
                        {
                            return (double)lhs * (int)rhs;
                        }
                        else if (lhs is double && rhs is double)
                        {
                            return (double)lhs * (double)rhs;
                        }
                        throw new RuntimeError(expression.Operator, "The input you entered must be numbers!");
                    }
                case TokenType.DIVIDE:
                    {
                        if (lhs is uint && rhs is uint)
                        {
                            return (uint)lhs / (uint)rhs;
                        }
                        if (lhs is uint && rhs is int)
                        {
                            return (uint)lhs / (uint)rhs;
                        }
                        else if (lhs is uint && rhs is double)
                        {
                            return (uint)lhs / (double)rhs;
                        }
                        else if (lhs is int && rhs is uint)
                        {
                            return (int)lhs / (uint)rhs;
                        }
                        else if (lhs is double && rhs is uint)
                        {
                            return (double)lhs / (uint)rhs;
                        }
                        if (lhs is int && rhs is int)
                        {
                            return (int)lhs / (int)rhs;
                        }
                        else if (lhs is int && rhs is double)
                        {
                            return (int)lhs / (double)rhs;
                        }
                        else if (lhs is double && rhs is int)
                        {
                            return (double)lhs / (int)rhs;
                        }
                        else if (lhs is double && rhs is double)
                        {
                            return (double)lhs / (double)rhs;
                        }
                        throw new RuntimeError(expression.Operator, "The input you entered must be numbers!");
                    }
                case TokenType.GREATER:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return (double)lhs > (double)rhs;
                case TokenType.LESS:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return (double)lhs < (double)rhs;
                case TokenType.GREATER_EQUAL:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return (double)lhs >= (double)rhs;
                case TokenType.LESS_EQUAL:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return (double)lhs <= (double)rhs;
                case TokenType.EQUAL:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return isEqual(lhs, rhs);
                case TokenType.NOT_EQUAL:
                    checkNumbers(expression.Operator, lhs, rhs);
                    return !isEqual(lhs, rhs);
            }
            return 0;
        }
        public object visitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }
        public object visitUnaryExpression(Unary expression)
        {
            var rhs = eval(expression.Expr_right);
            return rhs;
        }
        public object visitLiteralExpression(Literal expression)
        {
            return expression.Value;
        }
        public object visitGroupingExpression(Grouping expression)
        {
            return eval(expression.Expression);
        }
        private bool isEqual(Object lhs, Object rhs)
        {
            if (lhs == null && rhs == null)
            {
                return true;
            }
            if ((lhs == null && rhs != null) || (lhs != null && rhs == null))
            {
                return false;
            }
            return lhs.Equals(rhs);
        }
        private object eval(Expression expression)
        {
            return expression.Accept(this);
        }
        private void checkNumbers(Token _operator, object lhs, object rhs)
        {
            if ((lhs is int || lhs is double) && (rhs is int || rhs is double))
            {
                return;
            }
            throw new RuntimeError(_operator, "The input you entered must be numbers!");
        }
    }
}
