using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •———————————————•
       | fact Reject { |
       |     Bid bid;  |
       | }             |
       •———————————————• */

    [CorrespondenceType]
    public class Reject : CorrespondenceFact
    {
        public Predecessor<Bid> Bid { get; set; }

        public QueryResult<Person> LuckyBidder
        {
            get
            {
                return Community
                    .Query<Accept>(accept => accept.Request.Is(((Bid)this.Bid).Request))
                    .Query<Person>((person, accept) => ((Bid)accept.Bid).Bidder.Is(person));
            }
        }
    }
}
