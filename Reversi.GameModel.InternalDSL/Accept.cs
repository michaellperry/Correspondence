using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •——————————————————————•
       | fact Accept {        |
       |     Request request; |
       |     Bid bid;         |
       | }                    |
       •——————————————————————• */

    [CorrespondenceType]
    public class Accept : CorrespondenceFact
    {
        public Predecessor<Request> Request { get; set; }

        public Predecessor<Bid> Bid { get; set; }
    }
}
