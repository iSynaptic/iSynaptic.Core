using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public abstract class ValidationVisitor<TObservation> : Visitor<Unit>
    {
        protected ValidationVisitor(SymbolTable symbolTable)
        {
            SymbolTable = Guard.NotNull(symbolTable, "symbolTable");
        }

        protected void Validate<T>(IEnumerable<T> subjects)
        {
            Dispatch(subjects);
        }

        protected void Validate<T>(T subject)
        {
            this.Dispatch(subject);
        }

        protected void FailIf(bool shouldFail, TObservation observation)
        {
            if (shouldFail)
                Result &= Outcome.Failure(observation);
        }

        protected void Fail(TObservation observation)
        {
            Result &= Outcome.Failure(observation);
        }

        protected Outcome<TObservation> Result { get; set; }
        protected SymbolTable SymbolTable { get; private set; }
    }
}
