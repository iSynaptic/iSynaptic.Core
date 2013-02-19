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

using System;
using System.Collections.Generic;
using System.IO;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration
{
    public abstract class CSharpCodeAuthoringVisitor<TState> : CodeAuthoringVisitor<TState>
    {
        private readonly List<String> _usings = new List<String>();

        protected CSharpCodeAuthoringVisitor(TextWriter writer) 
            : this(writer, "  ")
        {
        }

        protected CSharpCodeAuthoringVisitor(TextWriter writer, string indentationToken)
            : base(writer, indentationToken)
        {
            AddUsings();
        }

        protected virtual void AddUsings()
        {
            AddUsing("System");
            AddUsing("System.Collections.Generic");
            AddUsing("System.ComponentModel");
            AddUsing("System.Linq");
        }

        protected void AddUsing(String identifier)
        {
            _usings.Add(identifier);
        }

        protected virtual void WriteUsings()
        {
            WriteLine(_usings.Delimit("\r\n", x => String.Format("using {0};", x)));
            WriteLine();
        }

        protected virtual IDisposable WithBlock()
        {
            return WithBlock(false);
        }

        protected virtual IDisposable WithBlock(Boolean withTrailingSemicolun)
        {
            return WithBlock("{", withTrailingSemicolun ? "};" : "}");
        }

        protected virtual IDisposable WriteBlock(Boolean withTrailingSemicolun, String formatString, params object[] args)
        {
            WriteLine(formatString, args);
            return WithBlock(withTrailingSemicolun);
        }
    }
}
