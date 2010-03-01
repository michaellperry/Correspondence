using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •——————————————————————•
       | fact Cancel {        |
       |     Request request; |
       | }                    |
       •——————————————————————• */

    [CorrespondenceType]
    public class Cancel : CorrespondenceFact
    {
        public Predecessor<Request> Request { get; set; }
    }
}
