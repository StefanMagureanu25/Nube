﻿using Nube.LexicalAnalysis;

namespace Nube.Semantic_Analysis.Errors
{
    public class RuntimeError : Exception
    {
        public Token Token { get; set; }

        public RuntimeError(Token token, string message) : base(message)
        {

        }
    }
}
