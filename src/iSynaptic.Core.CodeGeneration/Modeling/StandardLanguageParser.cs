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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sprache;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.CodeGeneration.Modeling
{
    [CLSCompliant(false)]
    public abstract class StandardLanguageParser
    {
        protected static readonly Parser<String> InheritsOperator = Parse.String(":").Text();

        protected static readonly Parser<String> BlockStart = Parse.String("{").Text().Named("block start");
        protected static readonly Parser<String> BlockEnd = Parse.String("}").Text().Named("block end");
        protected static readonly Parser<String> StatementEnd = Parse.String(";").Text();

        protected static readonly Parser<String> SingleLineComment
            = Parse.String("//")
                .Then(_ => Parse.AnyChar.Except(NewLineCharacter).Many().Text())
                .Select(txt => "//" + txt);

        protected static readonly Parser<String> MultiLineComment
            = Parse.String("/*")
                .Then(_ => Parse.AnyChar.Until(Parse.String("*/")).Text())
                .Select(txt => "/*" + txt + "*/");

        protected static readonly Parser<String> NewLine
            = Parse.String("\u000D\u000A") // Carriage return character
                .Or(Parse.String("\u000A")) // Line feed character
                .Or(Parse.String("\u000D")) // Carriage return character followed by line feed character
                .Or(Parse.String("\u0085")) // Next line character
                .Or(Parse.String("\u2028")) // Line separator character
                .Or(Parse.String("\u2029")) // Paragraph separator character
                .Text();

        protected static readonly Parser<Char> NewLineCharacter
            = Parse.Char('\u000D') // Carriage return character
                .Or(Parse.Char('\u000A')) // Line feed character
                .Or(Parse.Char('\u0085')) // Next line character
                .Or(Parse.Char('\u2028')) // Line separator character
                .Or(Parse.Char('\u2029')); // Paragraph separator character

        protected static readonly Parser<Char> WhitespaceCharacter
            = CharacterByUnicodeCategory(UnicodeCategory.SpaceSeparator)
                .Or(Parse.Char('\u0009')) // Horizontal tab character
                .Or(Parse.Char('\u000B')) // Vertical tab character
                .Or(Parse.Char('\u000C')); // Form feed character

        protected static readonly Parser<String> Whitespace
            = WhitespaceCharacter.AtLeastOnce().Text();

        protected static readonly Parser<Char> LetterCharacter = 
            CharacterByUnicodeCategory(
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.LetterNumber);

        protected static readonly Parser<Char> DecimalDigitCharacter
            = CharacterByUnicodeCategory(UnicodeCategory.DecimalDigitNumber);

        protected static readonly Parser<Char> ConnectingCharacter
            = CharacterByUnicodeCategory(UnicodeCategory.ConnectorPunctuation);

        protected static readonly Parser<Char> CombiningCharacter
            = CharacterByUnicodeCategory(
                UnicodeCategory.NonSpacingMark,
                UnicodeCategory.SpacingCombiningMark);

        protected static readonly Parser<Char> FormattingCharacter
            = CharacterByUnicodeCategory(UnicodeCategory.Format);

        protected static readonly Parser<Char> IdentifierPartCharacter
            = LetterCharacter
                .Or(DecimalDigitCharacter)
                .Or(ConnectingCharacter)
                .Or(CombiningCharacter)
                .Or(FormattingCharacter);

        protected static readonly Parser<Char> IdentifierStartCharacter
            = LetterCharacter
                .Or(Parse.Char('_'));

        protected static readonly Parser<String> IdentifierOrKeyword
            = from _ in Parse.Return(new Unit())
              let identifier = IdentifierStartCharacter
                .Once()
                .Concat(IdentifierPartCharacter.Many())
                .Text()
              from id in identifier
                .Or(Parse.Char('@').Then(__ => identifier.Select(x => "@" + x)))
              select id;

        protected static Parser<Boolean> Flag(String text)
        {
            return Parse.String(text)
                .Text()
                .Select(x => x.ToMaybe())
                .Or(Parse.Return(Maybe<String>.NoValue))
                .Select(x => x.HasValue);
        }

        protected static readonly Parser<IEnumerable<String>> TypeArgumentList
            = Parse.Ref(() => NamespaceOrTypeName).Delimit(',').Surround('<', '>');

        protected static readonly Parser<String> NamespaceOrTypeName
            = from ids in IdentifierOrKeyword.Delimit('.')
              from arguments in TypeArgumentList.Optional().ToArray()
              select String.Format(arguments.Length > 0 ? "{0}<{1}>" : "{0}", ids.Delimit("."), arguments.Delimit(", "));

        protected static Parser<Char> CharacterByUnicodeCategory(params UnicodeCategory[] categories)
        {
            Guard.NotNull(categories, "categories");

            return Parse.Char(c => categories.Contains(Char.GetUnicodeCategory(c)), "characterByUnicodeCategory");
        }

        protected static Parser<T> Concept<T, TDefinition>(String keyword, Parser<TDefinition> definition, Func<String, TDefinition, T> selector)
        {
            return ConceptCore(keyword, false, definition, (id, @base, def) => selector(id, def));
        }

        protected static Parser<T> Concept<T, TDefinition>(String keyword, Parser<TDefinition> definition, Func<String, Maybe<String>, TDefinition, T> selector)
        {
            return ConceptCore(keyword, true, definition, selector);
        }

        private static Parser<T> ConceptCore<T, TDefinition>(String keyword, bool canInherit, Parser<TDefinition> definition, Func<String, Maybe<String>, TDefinition, T> selector)
        {
            return from k in Parse.String(keyword)
                   from id in IdentifierOrKeyword

                   from @base in canInherit
                        ? InheritsOperator.Interleave().Then(_ => IdentifierOrKeyword).Optional()
                        : Parse.Return(Maybe<String>.NoValue)

                   from def in definition
                   select selector(id, @base, def);
        }

        protected static Parser<T> Blocked<T>(Parser<T> body)
        {
            return body.Surround(BlockStart, BlockEnd);
        }
    }
}
