using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;
using ISymbol = iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel.ISymbol;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public struct SymbolResolution
    {
        private readonly ISymbol _symbol;
        private readonly SymbolResolutionStatus _status;
        private readonly IEnumerable<ISymbol> _otherSymbols;

        public SymbolResolution(ISymbol symbol)
            : this()
        {
            _symbol = Guard.NotNull(symbol, "symbol");
            _status = SymbolResolutionStatus.Found;
        }

        private SymbolResolution(ISymbol symbol, IEnumerable<ISymbol> otherSymbols)
            : this()
        {
            _symbol = symbol;
            _status = SymbolResolutionStatus.Ambiguous;
            _otherSymbols = otherSymbols.ToArray();
        }

        public static SymbolResolution operator &(SymbolResolution left, SymbolResolution right)
        {
            if (left.Status == SymbolResolutionStatus.NotFound)
                return right;

            if (right.Status == SymbolResolutionStatus.NotFound)
                return left;

            var otherSymbols = left._otherSymbols ?? Enumerable.Empty<ISymbol>();

            return new SymbolResolution(left.Symbol, otherSymbols.Concat(right.Symbols));
        }

        public SymbolResolutionStatus Status { get { return _status; } }
        public ISymbol Symbol
        {
            get
            {
                if (Status == SymbolResolutionStatus.NotFound)
                    throw new InvalidOperationException("No symbol could be resolved.");

                if(Status == SymbolResolutionStatus.Ambiguous)
                    throw new InvalidOperationException("More than one symbol was resolved.");

                return _symbol;
            }
        }

        public IEnumerable<ISymbol> Symbols
        {
            get
            {
                if (_status == SymbolResolutionStatus.NotFound)
                    yield break;

                yield return _symbol;
                
                if(_otherSymbols == null)
                    yield break;

                foreach (var symbol in _otherSymbols)
                    yield return symbol;
            }
        }
    }
}