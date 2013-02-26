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
using System.Text.RegularExpressions;
using iSynaptic.Commons;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration
{
    public abstract class CodeAuthoringVisitor<TState> : Visitor<TState>
    {
        protected CodeAuthoringVisitor(TextWriter writer, String indentationToken)
            : this(new IndentingTextWriter(writer, indentationToken))
        {
        }

        protected CodeAuthoringVisitor(IndentingTextWriter writer)
        {
            Writer = Guard.NotNull(writer, "writer");
        }

        protected virtual IDisposable WithBlock(String start, String end)
        {
            WriteLine(start);
            var indentation = WithIndentation();

            Action onDispose = () =>
            {
                indentation.Dispose();
                WriteLine(end);
            };

            return onDispose.ToDisposable();
        }

        protected IDisposable WithIndentation()
        {
            Writer.IncreaseIndentation();
            return ((Action)Writer.DecreaseIndentation).ToDisposable();
        }

        protected virtual IDisposable WriteBlock(String start, String end, String formatString, params object[] args)
        {
            WriteLine(formatString, args);
            return WithBlock(start, end);
        }

        protected virtual void Write(string text)
        {
            Writer.Write(text);
        }

        protected void Write(string formatString, params object[] args)
        {
            Write(string.Format(formatString, args));
        }

        protected void WriteLine()
        {
            Write(Environment.NewLine);
        }

        protected void WriteLine(string text)
        {
            Write(text);
            WriteLine();
        }

        protected void WriteLine(string formatString, params object[] args)
        {
            Write(string.Format(formatString, args));
            WriteLine();
        }

        protected virtual void IncreaseIndentation()
        {
            Writer.IncreaseIndentation();
        }

        protected virtual void DecreaseIndentation()
        {
            Writer.DecreaseIndentation();
        }

        protected TState Delimit<T>(IEnumerable<T> subjects, TState state, String delimiter)
        {
            return Delimit(subjects, state, (st, t1, t2) => delimiter);
        }

        protected TState Delimit<T>(IEnumerable<T> subjects, TState state, Func<TState, T, T, String> delimiter)
        {
            return Dispatch(subjects, state, (st, t1, t2) => { Write(delimiter(st, t1, t2)); return st; });
        }

        protected static string Pascalize(string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
                                 match => match.Groups[1].Value.ToUpper());
        }

        protected static string Camelize(string lowercaseAndUnderscoredWord)
        {
            return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
        }

        protected static string Uncapitalize(string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        protected static string Capitalize(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

        protected IndentingTextWriter Writer { get; private set; }
    }
}
