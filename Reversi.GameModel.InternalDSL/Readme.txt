This is an experiment in representing a Correspondence model. This internal
DSL doesn't actually work yet. It doesn't even depend upon
UpdateControls.Correspondence.dll.

The Correspondence subdirectory represents the API that Correspondence would
have to expose to make this work.

Compare this internal DSL to the factual code:

namespace Reversi.GameModel.InternalDSL;

fact Queue {
	string identifier;
}

fact Time {
	datetime start;
}

fact Frame {
	Queue queue;
	Time time;
	
	Request* outstandingRequests {
		Request request : request.frame = this
			where request.isOutstanding
	}
}

fact Person {
	unique;
}

fact Request {
	Frame frame;
	Person requester;
	
	bool isOutstanding {
		not exists Accept accept : accept.request = this and
		not exists Cancel cancel : cancel.request = this
	}
	
	Bid* bids {
		Bid bid : bid.request = this
	}
}

fact Bid {
	Request request;
	Perdon bidder;
	
	bool isOutstanding {
		not exists Accept accept : accept.bid = this and
		not exists Reject reject : reject.bid = this
	}
}

fact Accept {
	Request request;
	Bid bid;
}

fact Reject {
	Bid bid;
}

fact Cancel {
	Request request;
}
