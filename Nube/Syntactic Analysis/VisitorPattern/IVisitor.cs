using static Nube.Syntactic_Analysis.Expression;

namespace Nube.Syntactic_Analysis.VisitorPattern
{
    // Visitor pattern
    public interface IVisitor<T>
    {
        T visitBinaryExpression(Binary expression);
        T visitAssignExpression(Assign expression);
        T visitUnaryExpression(Unary expression);
        T? visitLiteralExpression(Literal expression);
        T visitGroupingExpression(Grouping expression);
    }
}
