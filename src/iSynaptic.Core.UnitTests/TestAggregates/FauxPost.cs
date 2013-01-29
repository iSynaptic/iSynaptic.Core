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
using iSynaptic.Commons;

namespace iSynaptic.TestAggregates
{
    public class FauxPost : Aggregate<Guid>
    {
        #region Events

        public class Created : AggregateEvent<Guid>
        {
            public Created(String title, String description, Decimal price) : base(Guid.NewGuid(), 1)
            {
                Title = title;
                Description = description;
                Price = price;
            }

            public String Title { get; private set; }
            public String Description { get; private set; }
            public Decimal Price { get; private set; }
        }

        public class TextCopyUpdated : AggregateEvent<Guid>
        {
            public TextCopyUpdated(Guid id, int version, String title, String description) : base(id, version)
            {
                Title = title;
                Description = description;
            }

            public String Title { get; private set; }
            public String Description { get; private set; }
        }

        public class PriceChanged : AggregateEvent<Guid>
        {
            public PriceChanged(Guid id, int version, Decimal newPrice) : base(id, version)
            {
                NewPrice = newPrice;
            }

            public Decimal NewPrice { get; private set; }
        }

        #endregion

        public class Snapshot : AggregateSnapshot<Guid>
        {
            public Snapshot(Guid id, int version, DateTime takenAt, String title, String description, Decimal price) 
                : base(id, version, takenAt)
            {
                Title = title;
                Description = description;
                Price = price;
            }

            public string Title { get; private set; }
            public string Description { get; private set; }
            public decimal Price { get; private set; }
        }

        public FauxPost(String title, String description, Decimal price)
        {
            ApplyEvent(new Created(title, description, price));
        }

        public void UpdateTextCopy(String title, String description)
        {
            ApplyEvent(new TextCopyUpdated(Id, Version + 1, title, description));
        }

        public void ChangePrice(Decimal newPrice)
        {
            ApplyEvent(new PriceChanged(Id, Version + 1, newPrice));
        }

        private void On(Created @event)
        {
            Title = @event.Title;
            Description = @event.Description;
            Price = @event.Price;
        }

        protected override void OnApplySnapshot(AggregateSnapshot<Guid> snapshot)
        {
            var s = snapshot as Snapshot;
            if(s == null)
                throw new ArgumentException("Unrecognized snapshot type.", "snapshot");

            Title = s.Title;
            Description = s.Description;
            Price = s.Price;
        }

        protected override AggregateSnapshot<Guid> OnTakeSnapshot()
        {
            return new Snapshot(Id, Version, SystemClock.UtcNow, Title, Description, Price);
        }

        private void On(TextCopyUpdated @event)
        {
            Title = @event.Title;
            Description = @event.Description;
        }

        private void On(PriceChanged @event)
        {
            Price = @event.NewPrice;
        }

        // these exists ONLY for testing purposes; aggregates should not expose state
        public String Title { get; private set; }
        public String Description { get; private set; }
        public Decimal Price { get; private set; }
    }
}