using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •—————————————————————•
       | fact Time {         |
       |     datetime start; |
       | }                   |
       •—————————————————————• */

    [CorrespondenceType]
    public class Time : CorrespondenceFact
    {
        public DateTime Start { get; set; }
    }
}
