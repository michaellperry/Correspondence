using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •———————————————•
       | fact Person { |
       |     unique;   |
       | }             |
       •———————————————• */

    [CorrespondenceType]
    public class Person : CorrespondenceFact
    {
        public Guid Unique { get; set; }
    }
}
