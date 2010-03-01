using System;
using Reversi.GameModel.InternalDSL.Correspondence;

namespace Reversi.GameModel.InternalDSL
{
    /* •————————————————————————————————————————————————•
       | fact Frame {                                   |
       |     Queue queue;                               |
       |     Time time;                                 |
       |                                                |
       |     Request* outstandingRequests {             |
       |         Request request : request.frame = this |
       |             where request.isOutstanding        |
       |     }                                          |
       | }                                              |
       •————————————————————————————————————————————————• */

    [CorrespondenceType]
    public class Frame : CorrespondenceFact
    {
        public Predecessor<Queue> Queue { get; set; }

        public Predecessor<Time> Time { get; set; }

        public QueryResult<Request> OutstandingRequests
        {
            get { return Community.Query<Request>(request => request.Frame.Is(this) && request.IsOutstanding); }
        }

        // Not represented in Factual. Would be in a partial class.
        public Request NewRequest(Person requester)
        {
            return Community.AddFact(new Request() { Frame = this, Requester = requester });
        }
    }
}
