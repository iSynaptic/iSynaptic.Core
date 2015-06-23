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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic
{
    internal delegate Match<T> Pattern<T>(string input);
    internal struct Match<T>
    {
        public Maybe<T> Data;
        public string Remaining;
    }

    internal static class Text
    {
        public static Pattern<string> EndOfLine = input => input == string.Empty
            ? new Match<string> { Data = string.Empty.ToMaybe(), Remaining = string.Empty }
            : new Match<string> { Remaining = input };

        public static Pattern<T> Ref<T>(Func<Pattern<T>> selector)
        {
            return input => selector()(input);
        }

        public static Pattern<T> Return<T>(T value)
        {
            return input => new Match<T> { Data = value.ToMaybe(), Remaining = input };
        }

        public static Pattern<char> Char(char expected)
        {
            return input =>
            {
                if (input != null && input.Length >= 1 && input[0] == expected)
                    return new Match<char> { Data = expected.ToMaybe(), Remaining = input.Substring(1) };

                return new Match<char> { Remaining = input };
            };
        }

        public static Pattern<T> LeadBy<T, R>(this Pattern<T> self, Pattern<R> pattern)
        {
            return input =>
            {
                var match = pattern(input);
                if (!match.Data.HasValue) return new Match<T> { Remaining = input };

                return self(match.Remaining);
            };
        }

        public static Pattern<T> FollowedBy<T, R>(this Pattern<T> self, Pattern<R> pattern)
        {
            return input =>
            {
                var match = self(input);
                if (!match.Data.HasValue) return match;

                var nextMatch = pattern(match.Remaining);
                if (nextMatch.Data.HasValue) return new Match<T> { Data = match.Data, Remaining = nextMatch.Remaining };

                return new Match<T> { Remaining = input };
            };
        }

        public static Pattern<IEnumerable<T>> DelimitedBy<T, R>(this Pattern<T> self, Pattern<R> delimiter)
        {
            return input =>
            {
                var list = new List<T>();

                var match = self(input);
                while (match.Data.HasValue)
                {
                    list.Add(match.Data.Value);

                    var delimiterMatch = delimiter(match.Remaining);
                    if (delimiterMatch.Data.HasValue)
                        match = self(delimiterMatch.Remaining);
                    else
                        match = new Match<T> { Remaining = delimiterMatch.Remaining };
                }

                return new Match<IEnumerable<T>> { Data = list.ToMaybe<IEnumerable<T>>(), Remaining = match.Remaining };
            };
        }

        public static Pattern<Maybe<T>> Optional<T>(this Pattern<T> self)
        {
            return input =>
            {
                var match = self(input);
                return new Match<Maybe<T>> { Data = match.Data.ToMaybe(), Remaining = match.Remaining };
            };
        }

        public static Pattern<IEnumerable<T>> Optional<T>(this Pattern<IEnumerable<T>> self)
        {
            return input =>
            {
                var match = self(input);
                if (!match.Data.HasValue) return new Match<IEnumerable<T>> { Data = Enumerable.Empty<T>().ToMaybe(), Remaining = match.Remaining };
                return match;
            };
        }

        public static Pattern<string> Regex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentException("You must provide a pattern.", "pattern");

            var regex = new Regex(pattern);
            return input =>
            {
                var match = regex.Match(input);
                if (!match.Success)
                    return new Match<string> { Remaining = input };

                return new Match<string>
                {
                    Data = match.Value.ToMaybe(),
                    Remaining = input.Substring(match.Index + match.Length)
                };
            };
        }

        public static Pattern<R> Select<T, R>(this Pattern<T> self, Func<T, R> selector)
        {
            return input =>
            {
                var match = self(input);
                if (!match.Data.HasValue) return new Match<R> { Remaining = match.Remaining };

                try
                {
                    return new Match<R>
                    {
                        Data = selector(match.Data.Value).ToMaybe(),
                        Remaining = match.Remaining
                    };
                }
                catch
                {
                    return new Match<R> { Remaining = match.Remaining };
                }
            };
        }

        public static Pattern<R> SelectPattern<T, R>(this Pattern<T> self, Func<T, Pattern<R>> selector)
        {
            return input =>
            {
                var match = self(input);
                if (!match.Data.HasValue) return new Match<R> { Remaining = match.Remaining };

                var next = selector(match.Data.Value);
                var nextMatch = next(match.Remaining);

                return nextMatch;
            };
        }

        public static Pattern<R> SelectMany<T, R>(this Pattern<T> self, Func<T, Pattern<R>> selector)
        {
            return SelectPattern(self, selector);
        }

        public static Pattern<TResult> SelectMany<T, TIntermediate, TResult>(this Pattern<T> self,
                                                                            Func<T, Pattern<TIntermediate>> selector,
                                                                            Func<T, TIntermediate, TResult> combiner)
        {
            return SelectPattern(self, x => selector(x).Select(y => combiner(x, y)));
        }
    }

    public class LogicalType : IEquatable<LogicalType>
    {
        private static readonly string IdentifierRegex = @"^[a-zA-Z][a-zA-Z0-9]*";
        private static readonly string QualifiedIdentifierRegex = @"^([a-zA-Z][a-zA-Z0-9]*)(\.[a-zA-Z][a-zA-Z0-9]*)*";

        private static readonly Pattern<string> Identifier = Text.Regex(IdentifierRegex);
        private static readonly Pattern<string> QualifiedIdentifier = Text.Regex(QualifiedIdentifierRegex);
        private static readonly Pattern<int> ArityPattern = Text.Regex(@"(?<=^`)\d+").Select(int.Parse);
        private static readonly Pattern<int> VersionPattern = Text.Regex(@"(?<=^v)\d+").Select(int.Parse);

        private static readonly Pattern<LogicalType> Pattern
            = from ns in Identifier
              from type in QualifiedIdentifier.LeadBy(Text.Char(':'))
              from arity in ArityPattern.Optional()

              let typeArgPattern = !arity.HasValue
                ? Text.Ref(() => Pattern).DelimitedBy(Text.Regex(@"^,\s*")).LeadBy(Text.Char('<')).FollowedBy(Text.Char('>')).Optional()
                : Text.Return(Enumerable.Empty<LogicalType>())

              from typeArgs in typeArgPattern
              from version in VersionPattern.LeadBy(Text.Char(':')).Optional()
              select arity.HasValue
                ? new LogicalType(ns, type, arity.Value, version)
                : new LogicalType(ns, type, typeArgs, version);

        private static readonly Pattern<LogicalType> TopLevelPattern = Pattern.FollowedBy(Text.EndOfLine);
            
        private readonly string _namespaceAlias;
        private readonly string _typeName;
        private readonly int _arity;
        private readonly LogicalType[] _typeArguments;
        private readonly Maybe<int> _version;

        public static LogicalType Parse(String input)
        {
            var result = TryParse(input);

            if(!result.HasValue)
                throw new ArgumentException("Input does not match the format of a logical type.", "input");

            return result.Value;
        }

        public static Maybe<LogicalType> TryParse(String input)
        {
            try
            {
                return TopLevelPattern(input).Data;
            }
            catch { return Maybe.NoValue; }
        }

        public LogicalType(String namespaceAlias, String typeName)
            : this(namespaceAlias, typeName, 0, default(Maybe<int>))
        {
        }

        public LogicalType(String namespaceAlias, String typeName, int arity, Maybe<int> version)
            : this(namespaceAlias, typeName, arity, null, version)
        {
        }

        public LogicalType(String namespaceAlias, String typeName, IEnumerable<LogicalType> typeArguments, Maybe<int> version)
            : this(namespaceAlias, typeName, 0, typeArguments, version)
        {
        }

        private LogicalType(String namespaceAlias, String typeName, int arity, IEnumerable<LogicalType> typeArguments, Maybe<int> version)
        {
            _namespaceAlias = Guard.NotNullOrWhiteSpace(namespaceAlias, "namespaceAlias");
            _typeName = Guard.NotNullOrWhiteSpace(typeName, "typeName");

            if (!Regex.IsMatch(namespaceAlias, IdentifierRegex + "$")) throw new ArgumentOutOfRangeException("namespaceAlias", "NamespaceAlias must be an identifier.");
            if (!Regex.IsMatch(typeName, QualifiedIdentifierRegex + "$")) throw new ArgumentOutOfRangeException("typeName", "TypeName must be an identifier.");

            if (arity < 0) throw new ArgumentOutOfRangeException("arity", "Arity must not be negative.");
            _arity = arity;

            _typeArguments = typeArguments != null
                ? typeArguments.ToArray()
                : new LogicalType[0];

            if (_arity == 0 && _typeArguments.Length > 0)
                _arity = _typeArguments.Length;

            if (_typeArguments.Length > 0 && _arity != _typeArguments.Length)
                throw new ArgumentOutOfRangeException("arity", "Arity does not match the number of provided type arguments.");

            if (_typeArguments.Any(x => x.IsOpenType)) throw new ArgumentException("Type arguments cannot include open types.");

            _version = version;
        }

        public string NamespaceAlias { get { return _namespaceAlias; } }
        public string TypeName { get { return _typeName; } }
        public int Arity { get { return _arity; } }
        public IEnumerable<LogicalType> TypeArguments { get { return _typeArguments; } }
        public Maybe<int> Version { get { return _version; } }

        public bool IsOpenType { get { return Arity > 0 && TypeArguments.None(); } }
        public bool IsParameterized { get { return Arity > 0; } }

        public LogicalType GetOpenType()
        {
            if (IsOpenType) return this;
            return new LogicalType(NamespaceAlias, TypeName, Arity, Version);
        }

        public LogicalType MakeClosedType(params LogicalType[] typeArguments)
        {
            return MakeClosedType((IEnumerable<LogicalType>)typeArguments);
        }

        public LogicalType MakeClosedType(IEnumerable<LogicalType> typeArguments)
        {
            var args = Guard.NotNull(typeArguments, "typeArguments").ToArray();

            if (!IsOpenType) throw new InvalidOperationException("LogicalType is not an open type.");
            if (args.Length != Arity) throw new ArgumentException("Incorrect number of type arguments provided.", "typeArguments");

            return new LogicalType(NamespaceAlias, TypeName, typeArguments, Version);
        }

        public bool Equals(LogicalType other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!NamespaceAlias.Equals(other.NamespaceAlias, StringComparison.OrdinalIgnoreCase)) return false;
            if (!TypeName.Equals(other.TypeName, StringComparison.OrdinalIgnoreCase)) return false;
            if (!Arity.Equals(other.Arity)) return false;
            if (!TypeArguments.SequenceEqual(other.TypeArguments)) return false;
            if (!Version.Equals(other.Version)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LogicalType);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + NamespaceAlias.ToLowerInvariant().GetHashCode());
            hash = HashCode.MixJenkins32(hash + TypeName.ToLowerInvariant().GetHashCode());
            hash = HashCode.MixJenkins32(hash + Arity);
            hash = TypeArguments.Aggregate(hash, (h, lt) => HashCode.MixJenkins32(h + lt.GetHashCode()));
            hash = HashCode.MixJenkins32(hash + Version.ValueOrDefault(0));

            return hash;
        }

        public static bool operator ==(LogicalType left, LogicalType right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(LogicalType left, LogicalType right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            string genericsPart = String.Empty;

            if (TypeArguments.Any())
                genericsPart = string.Format("<{0}>", string.Join(", ", TypeArguments));
            else if (Arity > 0)
                genericsPart = "`" + Arity.ToString();

            string versionPart = Version.HasValue
                ? ":v" + Version.Value
                : "";

            return String.Format("{0}:{1}{2}{3}", NamespaceAlias, TypeName, genericsPart, versionPart);
        }
    }
}
