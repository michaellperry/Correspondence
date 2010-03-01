using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* 風覧覧覧覧覧覧覧�
       | fact Person { |
       |     unique;   |
       | }             |
       風覧覧覧覧覧覧覧� */

    [CorrespondenceType]
    public class Person : CorrespondenceFact
    {
        public Guid Unique { get; set; }
    }
}
