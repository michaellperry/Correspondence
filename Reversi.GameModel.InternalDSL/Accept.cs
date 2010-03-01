using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* 風覧覧覧覧覧覧覧覧覧覧藍
       | fact Accept {        |
       |     Request request; |
       |     Bid bid;         |
       | }                    |
       風覧覧覧覧覧覧覧覧覧覧藍 */

    [CorrespondenceType]
    public class Accept : CorrespondenceFact
    {
        public Predecessor<Request> Request { get; set; }

        public Predecessor<Bid> Bid { get; set; }
    }
}
