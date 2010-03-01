using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •——————————————————————————————————————————————————————————————•
       | fact Request {                                               |
       |     Frame frame;                                             |
       |     Person requester;                                        |
       |                                                              |
       |     bool isOutstanding {                                     |
       |         not exists Accept accept : accept.request = this and |
       |         not exists Cancel cancel : cancel.request = this     |
       |     }                                                        |
       |                                                              |
       |     Bid* bids {                                              |
       |         Bid bid : bid.request = this                         |
       |     }                                                        |
       | }                                                            |
       •——————————————————————————————————————————————————————————————• */

    [CorrespondenceType]
    public class Request : CorrespondenceFact
    {
        public Predecessor<Frame> Frame { get; set; }

        public Predecessor<Person> Requester { get; set; }

        public CorrespondencePredicate IsOutstanding
        {
            get
            {
                return
                    Community.Query<Accept>(accept => accept.Request.Is(this)).DoesNotExist() &
                    Community.Query<Cancel>(cancel => cancel.Request.Is(this)).DoesNotExist();
            }
        }

        public QueryResult<Bid> Bids
        {
            get { return Community.Query<Bid>(bid => bid.Request.Is(this)); }
        }

        // Not represented in Factual. Would be in a partial class.
        public Bid NewBid(Person bidder)
        {
            return Community.AddFact(new Bid() { Request = this, Bidder = bidder });
        }
    }
}
