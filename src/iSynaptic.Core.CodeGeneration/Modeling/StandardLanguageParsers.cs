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
using System.Globalization;
using System.Linq;
using Sprache;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling
{
    [CLSCompliant(false)]
    public static class StandardLanguageParsers
    {
        public static Parser<Char> LetterCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.LetterNumber);
        }

        public static Parser<Char> DecimalDigitCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.DecimalDigitNumber);
        }

        public static Parser<Char> ConnectingCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.ConnectorPunctuation);
        }

        public static Parser<Char> CombiningCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.NonSpacingMark,
                UnicodeCategory.SpacingCombiningMark);
        }

        public static Parser<Char> FormattingCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.Format);
        }

        public static Parser<Char> IdentifierPartCharacter()
        {
            return LetterCharacter()
                .Or(DecimalDigitCharacter())
                .Or(ConnectingCharacter())
                .Or(CombiningCharacter())
                .Or(FormattingCharacter());
        }

        public static Parser<Char> IdentifierStartCharacter()
        {
            return LetterCharacter()
                .Or(Parse.Char('_'));
        }

        public static Parser<String> IdentifierOrKeyword()
        {
            var identifier = IdentifierStartCharacter()
                .Once()
                .Concat(IdentifierPartCharacter().Many())
                .Text();

            return identifier
                .Or(Parse.Char('@').Then(_ => identifier.Select(x => "@" + x)));
        }

        public static Parser<String> SingleLineComment()
        {
            return Parse.String("//")
                .Then(_ => Parse.AnyChar.Except(NewLineCharacter()).Many().Text())
                .Select(txt => "//" + txt);
        }

        public static Parser<String> MultiLineComment()
        {
            return Parse.String("/*")
                .Then(_ => Parse.AnyChar.Until(Parse.String("*/")).Text())
                .Select(txt => "/*" + txt + "*/");
        }

        public static Parser<String> NewLine()
        {
            return Parse.String("\u000D\u000A") // Carriage return character
                .Or(Parse.String("\u000A")) // Line feed character
                .Or(Parse.String("\u000D")) // Carriage return character followed by line feed character
                .Or(Parse.String("\u0085")) // Next line character
                .Or(Parse.String("\u2028")) // Line separator character
                .Or(Parse.String("\u2029")) // Paragraph separator character
                .Text();
        }

        public static Parser<Char> NewLineCharacter()
        {
            return Parse.Char('\u000D') // Carriage return character
                .Or(Parse.Char('\u000A')) // Line feed character
                .Or(Parse.Char('\u0085')) // Next line character
                .Or(Parse.Char('\u2028')) // Line separator character
                .Or(Parse.Char('\u2029')); // Paragraph separator character
        }

        public static Parser<Char> WhitespaceCharacter()
        {
            return CharacterByUnicodeCategory(UnicodeCategory.SpaceSeparator)
                .Or(Parse.Char('\u0009')) // Horizontal tab character
                .Or(Parse.Char('\u000B')) // Vertical tab character
                .Or(Parse.Char('\u000C')); // Form feed character
        }

        public static Parser<String> Whitespace()
        {
            return WhitespaceCharacter().AtLeastOnce().Text();
        }

        private static Parser<Char> CharacterByUnicodeCategory(params UnicodeCategory[] categories)
        {
            Guard.NotNull(categories, "categories");

            return Parse.Char(c => categories.Contains(Char.GetUnicodeCategory(c)), "characterByUnicodeCategory");
        }

        public static Parser<T> Interleave<T>(Parser<T> body)
        {
            var interleave = (
                    SingleLineComment()
                .Or(MultiLineComment())
                .Or(NewLine())
                .Or(Whitespace()))
                .Many();

            return interleave
                .SelectMany(_ => body, (_, b) => b)
                    .SelectMany(b => interleave, (b, _) => b);
        }
    }
}
