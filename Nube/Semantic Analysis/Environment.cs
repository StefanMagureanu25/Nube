using Nube.Errors;
using Nube.LexicalAnalysis;

namespace Nube.Semantic_Analysis
{
    public class Environment
    {
        public Environment parentScope;
        public Dictionary<object, object> _values = new Dictionary<object, object>();
        public Environment()
        {
            parentScope = null;
        }

        public Environment(Environment parentScope)
        {
            this.parentScope = parentScope;
        }

        #region Methods for variables manipulation
        public void Define(object name, object value)
        {
            try
            {
                _values.Add(name, value);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"WARNING: Variable '{name}' was already declared! Replaced with the new value!\n");
                _values[name] = value;
            }
        }

        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Value))
            {
                return _values[name.Value];
            }
            if (parentScope != null)
            {
                return parentScope.Get(name);
            }
            throw new RuntimeError(name, $"Variable {name.Value} is undefined");
        }

        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Value))
            {
                _values[name.Value] = value;
                return;
            }
            throw new RuntimeError(name, $"Variable {name.Value} is undefined");
        }
        #endregion
    }
}
