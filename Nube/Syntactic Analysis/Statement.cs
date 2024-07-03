using Nube.LexicalAnalysis;
using Nube.Syntactic_Analysis.VisitorPattern;

namespace Nube.Syntactic_Analysis
{
    public abstract class Statement
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
        public class Print : Statement
        {
            public Expression Expr { get; set; }
            public Print(Expression expr)
            {
                Expr = expr;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitPrintStatement(this);
            }
        }
        public class Expr : Statement
        {
            public Expression Expression { get; set; }
            public Expr(Expression expr)
            {
                Expression = expr;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitExprStatement(this);
            }
        }

        public class Var : Statement
        {
            public Token VariableName { get; set; }
            public Expression AssignedValue { get; set; }

            public Var(Token variableName, Expression assignedValue)
            {
                VariableName = variableName;
                AssignedValue = assignedValue;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitVarStatement(this);
            }
        }
        public class Block : Statement
        {
            public List<Statement> statements { get; set; }
            public Block(List<Statement> statements)
            {
                this.statements = statements;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitBlockStatement(this);
            }
        }
        public class If : Statement
        {
            public Expression Condition { get; set; }
            public List<Statement> Then { get; set; }
            public List<Statement> Else { get; set; }

            public If(Expression condition, List<Statement> _then, List<Statement> _else)
            {
                Condition = condition;
                Then = _then;
                Else = _else;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitIfStatement(this);
            }
        }

        public class While : Statement
        {
            public Expression Condition { get; set; }
            public List<Statement> Body { get; set; }

            public While(Expression condition, List<Statement> body)
            {
                Condition = condition;
                Body = body;
            }
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.visitWhileStatement(this);
            }
        }

    }
}
