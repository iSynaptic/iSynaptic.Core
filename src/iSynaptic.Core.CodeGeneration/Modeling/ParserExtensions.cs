using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling
{
    [CLSCompliant(false)]
    public static class ParserExtensions
    {
        private class ParserAccess : StandardLanguageParser
        {
            private ParserAccess()
            {
            }

            public static Parser<String> GetSingleLineComment() { return SingleLineComment(); }
            public static Parser<String> GetMultiLineComment() { return MultiLineComment(); }
            public static Parser<String> GetNewLine() { return NewLine(); }
            public static Parser<String> GetWhitespace() { return Whitespace(); }
        }

        public static Parser<T> Interleave<T>(this Parser<T> body)
        {
            var interleave = 
                       (ParserAccess.GetSingleLineComment()
                    .Or(ParserAccess.GetMultiLineComment())
                    .Or(ParserAccess.GetNewLine())
                    .Or(ParserAccess.GetWhitespace()))
                .Many();

            return Parse.SelectMany(
                Parse.SelectMany(interleave, _ => body, (_, b) => b), b => interleave, (b, _) => b);
        }

        public static Parser<IEnumerable<T>> Delimit<T>(this Parser<T> parser, Char delimiter)
        {
            return parser.Delimit(Parse.Char(delimiter));
        }

        public static Parser<IEnumerable<T>> Delimit<T>(this Parser<T> parser, String delimiter)
        {
            return parser.Delimit(Parse.String(delimiter));
        }

        public static Parser<IEnumerable<T>> Delimit<T, TDelimiter>(this Parser<T> parser, Parser<TDelimiter> delimiter)
        {
            return from first in parser
                   from remaining in
                       (
                           from d in delimiter
                           from item in parser
                           select item
                       ).Many()
                   select new[] { first }.Concat(remaining);
        }

        public static Parser<Maybe<T>> Optional<T>(this Parser<T> parser)
        {
            return parser.Select(x => x.ToMaybe())
                    .Or(Parse.Return(Maybe<T>.NoValue));
        }

        public static Parser<IEnumerable<T>> Optional<T>(this Parser<IEnumerable<T>> parser)
        {
            return parser.Or(Parse.Return(Enumerable.Empty<T>()));
        }

        public static Parser<T[]> ToArray<T>(this Parser<IEnumerable<T>> parser)
        {
            return parser.Select(x => x.ToArray());
        }

        public static Parser<V> SelectMany<T, U, V>(this Parser<T> source, Func<T, Parser<U>> selector, Func<T, U, V> projector)
        {
            return Parse.SelectMany(source.Interleave(), selector, projector);
        }

        public static Parser<T> Surround<T>(this Parser<T> body, Char left, Char right)
        {
            return Surround(body, Parse.Char(left), Parse.Char(right));
        }

        public static Parser<T> Surround<T>(this Parser<T> body, String left, String right)
        {
            return Surround(body, Parse.String(left), Parse.String(right));
        }

        public static Parser<T> Surround<T, TLeft, TRight>(this Parser<T> body, Parser<TLeft> left, Parser<TRight> right)
        {
            return from l in left
                   from b in body
                   from r in right
                   select b;
        }
    }
}