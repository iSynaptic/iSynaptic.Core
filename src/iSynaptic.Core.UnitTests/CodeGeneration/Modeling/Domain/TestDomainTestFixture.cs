// The MIT License
// 
// Copyright (c) 2013 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sprache;
using iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    public abstract class TestDomainTestFixture
    {
        private readonly Compilation _compilation;
        private readonly SymbolTable _symbolTable;

        protected TestDomainTestFixture()
        {
            var streamNames = Assembly.GetExecutingAssembly()
                                      .GetManifestResourceNames()
                                      .Where(x => x.StartsWith("iSynaptic.CodeGeneration.Modeling.Domain."))
                                      .Where(x => x.EndsWith(".dom"));

            var trees = new List<SyntaxTree>();

            foreach (var streamName in streamNames)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName))
                using (var reader = new StreamReader(stream))
                {
                    var tree = Parser.SyntaxTree.Parse(reader.ReadToEnd());
                    trees.Add(tree);
                }

            }

            _compilation = Syntax.Compilation(trees);
            _symbolTable = SymbolTableConstructionVisitor.BuildSymbolTable(Compilation);
        }

        public Compilation Compilation { get { return _compilation; } }
        public SymbolTable SymbolTable { get { return _symbolTable; } }
    }
}