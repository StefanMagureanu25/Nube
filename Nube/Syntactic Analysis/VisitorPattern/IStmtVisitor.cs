using static Nube.Syntactic_Analysis.Statement;

namespace Nube.Syntactic_Analysis.VisitorPattern
{
    public interface IStmtVisitor<T>
    {
        T visitPrintStatement(Print print);
        T visitExprStatement(Expr expr);
        T visitVarStatement(Var var);
        T visitBlockStatement(Block block);
        T visitIfStatement(If _if);
        T visitWhileStatement(While _while);
        T visitForStatement(For _for);
    }
}
