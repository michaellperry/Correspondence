using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •——————————————————————————————————————————————————————————•
       | fact Bid {                                               |
       |     Request request;                                     |
       |     Perdon bidder;                                       |
       |                                                          |
       |     bool isOutstanding {                                 |
       |         not exists Accept accept : accept.bid = this and |
       |         not exists Reject reject : reject.bid = this     |
       |     }                                                    |
       | }                                                        |
       •——————————————————————————————————————————————————————————• */

    [CorrespondenceType]
    public class Bid : CorrespondenceFact
    {
        public Predecessor<Request> Request { get; set; }

        public Predecessor<Person> Bidder { get; set; }

        public CorrespondencePredicate IsOutstanding
        {
            get
            {
                return
                    Community.Query<Accept>(accept => accept.Bid.Is(this)).DoesNotExist() &
                    Community.Query<Reject>(reject => reject.Bid.Is(this)).DoesNotExist();
            }
        }

        // Not represented in Factual. Would be in a partial class.
        public void Accept()
        {
            Community.AddFact(new Accept() { Bid = this, Request = this.Request });
        }

        // Not represented in Factual. Would be in a partial class.
        public void Reject()
        {
            Community.AddFact(new Reject() { Bid = this });
        }
    }
}
