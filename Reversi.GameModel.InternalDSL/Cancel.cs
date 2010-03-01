using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* 風覧覧覧覧覧覧覧覧覧覧藍
       | fact Cancel {        |
       |     Request request; |
       | }                    |
       風覧覧覧覧覧覧覧覧覧覧藍 */

    [CorrespondenceType]
    public class Cancel : CorrespondenceFact
    {
        public Predecessor<Request> Request { get; set; }
    }
}
