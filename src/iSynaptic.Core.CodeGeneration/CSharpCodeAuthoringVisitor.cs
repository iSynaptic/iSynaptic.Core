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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using iSynaptic.Commons.Text;

namespace iSynaptic.CodeGeneration
{
    public abstract class CSharpCodeAuthoringVisitor<TState> : CodeAuthoringVisitor<TState>
    {
        private static readonly Regex SafeIdentifierRegex = new Regex("^(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while)$");

        protected CSharpCodeAuthoringVisitor(TextWriter writer) 
            : base(writer, "    ")
        {
        }

        protected CSharpCodeAuthoringVisitor(IndentingTextWriter writer)
            : base(writer)
        {
        }

        protected virtual IDisposable WithBlock()
        {
            return WithBlock("{", "}");
        }

        protected virtual IDisposable WithStatementBlock()
        {
            return WithBlock("{", "};");
        }

        protected virtual IDisposable WriteBlock(String formatString, params object[] args)
        {
            WriteLine(formatString, args);
            return WithBlock();
        }

        protected virtual IDisposable WriteStatementBlock(String formatString, params object[] args)
        {
            WriteLine(formatString, args);
            return WithStatementBlock();
        }

        protected virtual void WriteGeneratedCodeAttribute(string tool)
        {
            var asm = Assembly.GetCallingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);

            string version = versionInfo.FileVersion;
            WriteGeneratedCodeAttribute(tool, version);
        }

        protected virtual void WriteGeneratedCodeAttribute(string tool, string version)
        {
            Write("[GeneratedCode(\"{0}\", \"{1}\")]", tool, version);
        }

        protected static String SafeIdentifier(String identifier)
        {
            return SafeIdentifierRegex.IsMatch(identifier) 
                ? String.Format("@{0}", identifier) 
                : identifier;
        }
    }
}
