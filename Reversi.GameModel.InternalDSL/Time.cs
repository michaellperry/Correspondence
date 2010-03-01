using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* 風覧覧覧覧覧覧覧覧覧覧�
       | fact Time {         |
       |     datetime start; |
       | }                   |
       風覧覧覧覧覧覧覧覧覧覧� */

    [CorrespondenceType]
    public class Time : CorrespondenceFact
    {
        public DateTime Start { get; set; }
    }
}
