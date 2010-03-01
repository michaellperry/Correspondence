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
    }
}
