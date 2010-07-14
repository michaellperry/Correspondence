using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System;

namespace Reversi.GameModel
{
    [CorrespondenceType]
    public partial class Queue : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields
        [CorrespondenceField]
        private string _identifier;

        // Results

        // Business constructor
        public Queue(
            string identifier
            )
        {
            InitializeResults();
            _identifier = identifier;
        }

        // Hydration constructor
        public Queue(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Time : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields
        [CorrespondenceField]
        private DateTime _start;

        // Results

        // Business constructor
        public Time(
            DateTime start
            )
        {
            InitializeResults();
            _start = start;
        }

        // Hydration constructor
        public Time(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access
        public DateTime Start
        {
            get { return _start; }
        }

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Frame : CorrespondenceFact
    {
        // Roles
        public static Role<Queue> RoleQueue = new Role<Queue>("queue");
        public static Role<Time> RoleTimestamp = new Role<Time>("timestamp");

        // Queries
        public static Query QueryOutstandingRequests = new Query()
            .JoinSuccessors(Request.RoleFrame, Condition.WhereIsEmpty(Request.QueryIsAccepted)
                .And().IsEmpty(Request.QueryIsCanceled)
            )
            ;

        // Predicates

        // Predecessors
        private PredecessorObj<Queue> _queue;
        private PredecessorObj<Time> _timestamp;

        // Fields

        // Results
        private Result<Request> _outstandingRequests;

        // Business constructor
        public Frame(
            Queue queue
            ,Time timestamp
            )
        {
            InitializeResults();
            _queue = new PredecessorObj<Queue>(this, RoleQueue, queue);
            _timestamp = new PredecessorObj<Time>(this, RoleTimestamp, timestamp);
        }

        // Hydration constructor
        public Frame(FactMemento memento)
        {
            InitializeResults();
            _queue = new PredecessorObj<Queue>(this, RoleQueue, memento);
            _timestamp = new PredecessorObj<Time>(this, RoleTimestamp, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _outstandingRequests = new Result<Request>(this, QueryOutstandingRequests);
        }

        // Predecessor access
        public Queue Queue
        {
            get { return _queue.Fact; }
        }
        public Time Timestamp
        {
            get { return _timestamp.Fact; }
        }

        // Field access

        // Query result access
        public IEnumerable<Request> OutstandingRequests
        {
            get { return _outstandingRequests; }
        }
    }
    
    [CorrespondenceType]
    public partial class Player : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Fields

        // Results

        // Business constructor
        public Player(
            )
        {
            InitializeResults();
        }

        // Hydration constructor
        public Player(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Game : CorrespondenceFact
    {
        // Roles
        public static Role<Player> RolePlayers = new Role<Player>("players");

        // Queries

        // Predicates

        // Predecessors
        private PredecessorList<Player> _players;

        // Fields

        // Results

        // Business constructor
        public Game(
            IEnumerable<Player> players
            )
        {
            InitializeResults();
            _players = new PredecessorList<Player>(this, RolePlayers, players);
        }

        // Hydration constructor
        public Game(FactMemento memento)
        {
            InitializeResults();
            _players = new PredecessorList<Player>(this, RolePlayers, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public IEnumerable<Player> Players
        {
            get { return _players; }
        }
     
        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Person : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predicates

        // Predecessors

        // Unique
        [CorrespondenceField]
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public Person(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        public Person(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Request : CorrespondenceFact
    {
        // Roles
        public static Role<Frame> RoleFrame = new Role<Frame>("frame", RoleRelationship.Pivot);
        public static Role<Person> RoleRequester = new Role<Person>("requester");

        // Queries
        public static Query QueryIsAccepted = new Query()
            .JoinSuccessors(Accept.RoleRequest)
            ;
        public static Query QueryIsCanceled = new Query()
            .JoinSuccessors(Cancel.RoleRequest)
            ;
        public static Query QueryBids = new Query()
            .JoinSuccessors(Bid.RoleRequest, Condition.WhereIsNotEmpty(Bid.QueryIsAccepted)
                .And().IsNotEmpty(Bid.QueryIsRejected)
            )
            ;

        // Predicates
        public static Condition IsAccepted = Condition.WhereIsNotEmpty(QueryIsAccepted);
        public static Condition IsCanceled = Condition.WhereIsNotEmpty(QueryIsCanceled);

        // Predecessors
        private PredecessorObj<Frame> _frame;
        private PredecessorObj<Person> _requester;

        // Fields

        // Results
        private Result<Bid> _bids;

        // Business constructor
        public Request(
            Frame frame
            ,Person requester
            )
        {
            InitializeResults();
            _frame = new PredecessorObj<Frame>(this, RoleFrame, frame);
            _requester = new PredecessorObj<Person>(this, RoleRequester, requester);
        }

        // Hydration constructor
        public Request(FactMemento memento)
        {
            InitializeResults();
            _frame = new PredecessorObj<Frame>(this, RoleFrame, memento);
            _requester = new PredecessorObj<Person>(this, RoleRequester, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _bids = new Result<Bid>(this, QueryBids);
        }

        // Predecessor access
        public Frame Frame
        {
            get { return _frame.Fact; }
        }
        public Person Requester
        {
            get { return _requester.Fact; }
        }

        // Field access

        // Query result access
        public IEnumerable<Bid> Bids
        {
            get { return _bids; }
        }
    }
    
    [CorrespondenceType]
    public partial class Bid : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");
        public static Role<Person> RoleBidder = new Role<Person>("bidder");

        // Queries
        public static Query QueryIsAccepted = new Query()
            .JoinSuccessors(Accept.RoleBid)
            ;
        public static Query QueryIsRejected = new Query()
            .JoinSuccessors(Reject.RoleBid)
            ;

        // Predicates
        public static Condition IsAccepted = Condition.WhereIsEmpty(QueryIsAccepted);
        public static Condition IsRejected = Condition.WhereIsEmpty(QueryIsRejected);

        // Predecessors
        private PredecessorObj<Request> _request;
        private PredecessorObj<Person> _bidder;

        // Fields

        // Results

        // Business constructor
        public Bid(
            Request request
            ,Person bidder
            )
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
            _bidder = new PredecessorObj<Person>(this, RoleBidder, bidder);
        }

        // Hydration constructor
        public Bid(FactMemento memento)
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
            _bidder = new PredecessorObj<Person>(this, RoleBidder, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Request Request
        {
            get { return _request.Fact; }
        }
        public Person Bidder
        {
            get { return _bidder.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Accept : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");
        public static Role<Bid> RoleBid = new Role<Bid>("bid");

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Request> _request;
        private PredecessorObj<Bid> _bid;

        // Fields

        // Results

        // Business constructor
        public Accept(
            Request request
            ,Bid bid
            )
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
            _bid = new PredecessorObj<Bid>(this, RoleBid, bid);
        }

        // Hydration constructor
        public Accept(FactMemento memento)
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
            _bid = new PredecessorObj<Bid>(this, RoleBid, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Request Request
        {
            get { return _request.Fact; }
        }
        public Bid Bid
        {
            get { return _bid.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Reject : CorrespondenceFact
    {
        // Roles
        public static Role<Bid> RoleBid = new Role<Bid>("bid");

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Bid> _bid;

        // Fields

        // Results

        // Business constructor
        public Reject(
            Bid bid
            )
        {
            InitializeResults();
            _bid = new PredecessorObj<Bid>(this, RoleBid, bid);
        }

        // Hydration constructor
        public Reject(FactMemento memento)
        {
            InitializeResults();
            _bid = new PredecessorObj<Bid>(this, RoleBid, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Bid Bid
        {
            get { return _bid.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class Cancel : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Request> _request;

        // Fields

        // Results

        // Business constructor
        public Cancel(
            Request request
            )
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
        }

        // Hydration constructor
        public Cancel(FactMemento memento)
        {
            InitializeResults();
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Request Request
        {
            get { return _request.Fact; }
        }

        // Field access

        // Query result access
    }
    
}
