using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •————————————————————————•
       | fact Queue {           |
       |     string identifier; |
       | }                      |
       •————————————————————————• */

    [CorrespondenceType]
    public class Queue : CorrespondenceFact
    {
        public string Identifier { get; set; }
    }
}