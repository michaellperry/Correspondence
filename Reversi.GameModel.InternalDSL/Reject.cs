using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* 風覧覧覧覧覧覧覧�
       | fact Reject { |
       |     Bid bid;  |
       | }             |
       風覧覧覧覧覧覧覧� */

    [CorrespondenceType]
    public class Reject : CorrespondenceFact
    {
        public Predecessor<Bid> Bid { get; set; }
    }
}
