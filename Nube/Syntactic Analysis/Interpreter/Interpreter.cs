using Nube.Errors;
using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis.VisitorPattern;
using static Nube.Syntactic_Analysis.Expression;

namespace Nube.Syntactic_Analysis.Interpreter
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        private Semantic_Analysis.Environment _environment = new Semantic_Analysis.Environment();
        #region Evaluate the input
        public void interpret(List<Statement> statements)
        {
            try
            {
                foreach (Statement statement in statements)
                {
                    execute(statement);
                }
            }
            catch (RuntimeError ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void execute(Statement statement)
        {
            statement.Accept(this);
        }
        private object eval(Expression expression)
        {
            return expression.Accept(this);
        }
        #endregion

        #region Types of expressions
        public object visitBinaryExpression(Binary expression)
        {
            var lhs = eval(expression.Expr_left);
            var rhs = eval(expression.Expr_right);
            switch (expression.Operator.Type)
            {
                case TokenType.PLUS:
                    {
                        return AddObjects(lhs, rhs);
                    }
                case TokenType.MINUS:
                    {
                        return SubtractObjects(lhs, rhs);
                    }
                case TokenType.MULTIPLY:
                    {
                        return MultiplyObjects(lhs, rhs);
                    }
                case TokenType.DIVIDE:
                    {
                        return DivideObjects(lhs, rhs);
                    }
                case TokenType.GREATER:
                    return CompareObjects(lhs, rhs, TokenType.GREATER);
                case TokenType.LESS:
                    return CompareObjects(lhs, rhs, TokenType.LESS);
                case TokenType.GREATER_EQUAL:
                    return CompareObjects(lhs, rhs, TokenType.GREATER_EQUAL);
                case TokenType.LESS_EQUAL:
                    return CompareObjects(lhs, rhs, TokenType.LESS_EQUAL);
                case TokenType.EQUAL:
                    return CompareObjects(lhs, rhs, TokenType.EQUAL);
                case TokenType.NOT_EQUAL:
                    return CompareObjects(lhs, rhs, TokenType.NOT_EQUAL);
            }
            return 0;
        }

        public object visitAssignExpression(Assign expression)
        {
            object value = eval(expression.Value);
            _environment.Assign(expression.Name, value);
            return value;
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

        public object visitVariableExpression(Variable variable)
        {
            return _environment.Get(variable.Name);
        }
        public object visitLogicalExpression(Logical logical)
        {
            object lhs = eval(logical.Expr_left);
            if (logical.Operator.Type == TokenType.OR)
            {
                if (isTrue(lhs))
                {
                    return lhs;
                }
            }
            else
            {
                if (!isTrue(lhs))
                {
                    return lhs;
                }
            }
            return eval(logical.Expr_right);
        }
        #endregion

        #region Types of statements
        public object visitPrintStatement(Statement.Print print)
        {
            object value = eval(print.Expr);
            Console.WriteLine(value);
            return null;
        }

        public object visitExprStatement(Statement.Expr expr)
        {
            eval(expr.Expression);
            return null;
        }

        public object visitVarStatement(Statement.Var var)
        {
            object value = null;
            if (var.AssignedValue != null)
            {
                value = eval(var.AssignedValue);
            }
            _environment.Define(var.VariableName.Value, value);
            return null;
        }

        public object visitBlockStatement(Statement.Block block)
        {
            executeBlock(block.statements, new Semantic_Analysis.Environment(_environment));
            return null;
        }

        public void executeBlock(List<Statement> statements, Semantic_Analysis.Environment environment)
        {
            Semantic_Analysis.Environment prevEnvironment = this._environment;
            try
            {
                this._environment = environment;
                foreach (Statement statement in statements)
                {
                    execute(statement);
                }
            }
            finally
            {
                this._environment = prevEnvironment;
            }
        }

        public object visitIfStatement(Statement.If _if)
        {
            if (isTrue(eval(_if.Condition)))
            {
                foreach (Statement statement in _if.Then)
                {
                    execute(statement);
                }
            }
            else if (_if.Else != null)
            {
                foreach (Statement statement in _if.Else)
                {
                    execute(statement);
                }
            }
            return null;
        }

        public object visitWhileStatement(Statement.While _while)
        {
            while (isTrue(eval(_while.Condition)))
            {
                foreach (Statement statement in _while.Body)
                {
                    execute(statement);
                }
            }
            return null;
        }
        #endregion

        #region Binary expressions helper
        #region Compare section
        private bool CompareObjects(object? lhs, object? rhs, TokenType operation)
        {
            checkTypes(ref lhs, ref rhs);

            return (lhs, rhs) switch
            {
                (uint left, uint right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (uint left, int right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (uint left, double right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (int left, uint right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (int left, int right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (int left, double right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (double left, uint right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (double left, int right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (double left, double right) => operation switch
                {
                    TokenType.LESS => left < right,
                    TokenType.GREATER => left > right,
                    TokenType.GREATER_EQUAL => left >= right,
                    TokenType.LESS_EQUAL => left <= right,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                (string left, string right) => operation switch
                {
                    TokenType.LESS => string.Compare(left, right) < 0,
                    TokenType.GREATER => string.Compare(left, right) > 0,
                    TokenType.GREATER_EQUAL => string.Compare(left, right) >= 0,
                    TokenType.LESS_EQUAL => string.Compare(left, right) <= 0,
                    TokenType.EQUAL => left == right,
                    TokenType.NOT_EQUAL => left != right,
                    _ => throw new InvalidOperationException("Unsupported operation.")
                },
                _ => throw new InvalidOperationException("Unsupported type combination.")
            };
        }
        #endregion

        #region Add section
        private object AddObjects(object? lhs, object? rhs)
        {
            checkTypes(ref lhs, ref rhs);

            return (lhs, rhs) switch
            {
                (uint left, uint right) => left + right,
                (uint left, int right) => left + right,
                (uint left, double right) => left + right,
                (int left, uint right) => left + right,
                (int left, int right) => left + right,
                (int left, double right) => left + right,
                (double left, uint right) => left + right,
                (double left, int right) => left + right,
                (double left, double right) => left + right,
                (string left, string right) => left + right,
                _ => throw new InvalidOperationException("Invalid type combination.")
            };
        }
        #endregion

        #region Subtract section
        private object SubtractObjects(object? lhs, object? rhs)
        {
            checkTypes(ref lhs, ref rhs);

            return (lhs, rhs) switch
            {
                (uint left, uint right) => left - right,
                (uint left, int right) => left - right,
                (uint left, double right) => left - right,
                (int left, uint right) => left - right,
                (int left, int right) => left - right,
                (int left, double right) => left - right,
                (double left, uint right) => left - right,
                (double left, int right) => left - right,
                (double left, double right) => left - right,
                _ => throw new InvalidOperationException("Unsupported type combination.")
            };
        }
        #endregion

        #region Multiply section
        private object MultiplyObjects(object? lhs, object? rhs)
        {
            checkTypes(ref lhs, ref rhs);

            return (lhs, rhs) switch
            {
                (uint left, uint right) => left * right,
                (uint left, int right) => left * right,
                (uint left, double right) => left * right,
                (int left, uint right) => left * right,
                (int left, int right) => left * right,
                (int left, double right) => left * right,
                (double left, uint right) => left * right,
                (double left, int right) => left * right,
                (double left, double right) => left * right,
                _ => throw new InvalidOperationException("Unsupported type combination.")
            };
        }
        #endregion

        #region Divide section
        private object DivideObjects(object? lhs, object? rhs)
        {
            checkTypes(ref lhs, ref rhs);

            return (lhs, rhs) switch
            {
                (uint left, uint right) => left / right,
                (uint left, int right) => left / right,
                (uint left, double right) => left / right,
                (int left, uint right) => left / right,
                (int left, int right) => left / right,
                (int left, double right) => left / right,
                (double left, uint right) => left / right,
                (double left, int right) => left / right,
                (double left, double right) => left / right,
                _ => throw new InvalidOperationException("Unsupported type combination.")
            };
        }
        #endregion

        #region Type checking
        private void checkTypes(ref object? lhs, ref object? rhs)
        {
            switch (lhs)
            {
                case uint _ when rhs is uint:
                case uint _ when rhs is int:
                case uint _ when rhs is double:
                case int _ when rhs is uint:
                case int _ when rhs is int:
                case int _ when rhs is double:
                case double _ when rhs is uint:
                case double _ when rhs is int:
                case double _ when rhs is double:
                case string _ when rhs is string:
                    // Types are valid; no action needed
                    break;

                default:
                    // Handle invalid type combinations or null values if necessary
                    throw new ArgumentException("Invalid type combination.");
            }
        }
        #endregion
        #endregion

        private bool isTrue(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is bool)
            {
                return (bool)value;
            }
            return true;
        }
    }
}
