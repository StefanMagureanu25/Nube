using Nube.Syntactic_Analysis.VisitorPattern;
using System.Text;
using static Nube.Syntactic_Analysis.Expression;

namespace Nube.Syntactic_Analysis.Printer;
public class AstPrinter : IVisitor<string>
{
    public string Print(Expression expression)
    {
        return expression.Accept(this);
    }
    public string visitBinaryExpression(Binary expression)
    {
        return outputFormat(expression.Operator.Value, new List<Expression> { expression.Expr_left, expression.Expr_right });
    }
    public string visitAssignExpression(Assign expression)
    {
        throw new NotImplementedException();
    }
    public string visitUnaryExpression(Unary expression)
    {
        return outputFormat(expression.Operator.Value, new List<Expression> { expression.Expr_right });
    }
    public string? visitLiteralExpression(Literal expression)
    {
        return expression.Value.ToString();
    }
    public string visitGroupingExpression(Grouping expression)
    {
        return outputFormat("group", new List<Expression> { expression.Expression });
    }
    private string outputFormat(string name, List<Expression> expressions)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("(");
        stringBuilder.Append(name);
        foreach (Expression e in expressions)
        {
            stringBuilder.Append(" ");
            stringBuilder.Append(e.Accept(this));
        }
        stringBuilder.Append(")");
        return stringBuilder.ToString();
    }
}
