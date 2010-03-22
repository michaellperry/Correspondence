using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System;

namespace Reversi.GameModel
{
    [CorrespondenceType]
    public class Queue : CorrespondenceFact
    {
        // Roles

        // Queries

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
            _identifier = identifier;
        }

        // Hydration constructor
        public Queue(FactMemento memento)
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
    public class Time : CorrespondenceFact
    {
        // Roles

        // Queries

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
            _start = start;
        }

        // Hydration constructor
        public Time(FactMemento memento)
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
    public class Frame : CorrespondenceFact
    {
        // Roles
        public static Role<Queue> RoleQueue = new Role<Queue>("queue");
        public static Role<Time> RoleTimestamp = new Role<Time>("timestamp");

        // Queries
        private static Query QueryOutstandingRequests = new Query()
            .JoinSuccessors(Request.RoleFrame)
            ;

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
            _queue = new PredecessorObj<Queue>(this, RoleQueue, queue);
            _timestamp = new PredecessorObj<Time>(this, RoleTimestamp, timestamp);
        }

        // Hydration constructor
        public Frame(FactMemento memento)
        {
            _queue = new PredecessorObj<Queue>(this, RoleQueue, memento);
            _timestamp = new PredecessorObj<Time>(this, RoleTimestamp, memento);
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
    public class Player : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predecessors

        // Fields

        // Results

        // Business constructor
        public Player(
            )
        {
        }

        // Hydration constructor
        public Player(FactMemento memento)
        {
        }

        // Predecessor access

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        // Roles
        public static Role<Player> RolePlayers = new Role<Player>("players");

        // Queries

        // Predecessors
        private PredecessorList<Player> _players;

        // Fields

        // Results

        // Business constructor
        public Game(
            IEnumerable<Player> players
            )
        {
            _players = new PredecessorList<Player>(this, RolePlayers, players);
        }

        // Hydration constructor
        public Game(FactMemento memento)
        {
            _players = new PredecessorList<Player>(this, RolePlayers, memento);
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
    public class Person : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predecessors

        // Fields

        // Results

        // Business constructor
        public Person(
            )
        {
        }

        // Hydration constructor
        public Person(FactMemento memento)
        {
        }

        // Predecessor access

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public class Request : CorrespondenceFact
    {
        // Roles
        public static Role<Frame> RoleFrame = new Role<Frame>("frame");
        public static Role<Person> RoleRequester = new Role<Person>("requester");

        // Queries
        private static Query QueryIsOutstanding = new Query()
            .JoinSuccessors(Accept.RoleRequest)
            ;
        private static Query QueryBids = new Query()
            .JoinSuccessors(Bid.RoleRequest)
            ;

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
            _frame = new PredecessorObj<Frame>(this, RoleFrame, frame);
            _requester = new PredecessorObj<Person>(this, RoleRequester, requester);
        }

        // Hydration constructor
        public Request(FactMemento memento)
        {
            _frame = new PredecessorObj<Frame>(this, RoleFrame, memento);
            _requester = new PredecessorObj<Person>(this, RoleRequester, memento);
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
    public class Bid : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");
        public static Role<Person> RoleBidder = new Role<Person>("bidder");

        // Queries

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
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
            _bidder = new PredecessorObj<Person>(this, RoleBidder, bidder);
        }

        // Hydration constructor
        public Bid(FactMemento memento)
        {
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
            _bidder = new PredecessorObj<Person>(this, RoleBidder, memento);
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
    public class Accept : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");
        public static Role<Bid> RoleBid = new Role<Bid>("bid");

        // Queries

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
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
            _bid = new PredecessorObj<Bid>(this, RoleBid, bid);
        }

        // Hydration constructor
        public Accept(FactMemento memento)
        {
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
            _bid = new PredecessorObj<Bid>(this, RoleBid, memento);
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
    public class Reject : CorrespondenceFact
    {
        // Roles
        public static Role<Bid> RoleBid = new Role<Bid>("bid");

        // Queries

        // Predecessors
        private PredecessorObj<Bid> _bid;

        // Fields

        // Results

        // Business constructor
        public Reject(
            Bid bid
            )
        {
            _bid = new PredecessorObj<Bid>(this, RoleBid, bid);
        }

        // Hydration constructor
        public Reject(FactMemento memento)
        {
            _bid = new PredecessorObj<Bid>(this, RoleBid, memento);
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
    public class Cancel : CorrespondenceFact
    {
        // Roles
        public static Role<Request> RoleRequest = new Role<Request>("request");

        // Queries

        // Predecessors
        private PredecessorObj<Request> _request;

        // Fields

        // Results

        // Business constructor
        public Cancel(
            Request request
            )
        {
            _request = new PredecessorObj<Request>(this, RoleRequest, request);
        }

        // Hydration constructor
        public Cancel(FactMemento memento)
        {
            _request = new PredecessorObj<Request>(this, RoleRequest, memento);
        }

        // Predecessor access
        public Request Request
        {
            get { return _request.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public class Subscriber : CorrespondenceFact
    {
        // Roles

        // Queries
        private static Query QueryArticles = new Query()
            .JoinSuccessors(Subscription.RoleSubscriber)
            .JoinPredecessors(Subscription.RoleMagazine)
            .JoinSuccessors(Article.RoleMagazine)
            ;

        // Predecessors

        // Fields

        // Results
        private Result<Article> _articles;

        // Business constructor
        public Subscriber(
            )
        {
        }

        // Hydration constructor
        public Subscriber(FactMemento memento)
        {
        }

        // Predecessor access

        // Field access

        // Query result access
        public IEnumerable<Article> Articles
        {
            get { return _articles; }
        }
    }
    
    [CorrespondenceType]
    public class Magazine : CorrespondenceFact
    {
        // Roles

        // Queries

        // Predecessors

        // Fields

        // Results

        // Business constructor
        public Magazine(
            )
        {
        }

        // Hydration constructor
        public Magazine(FactMemento memento)
        {
        }

        // Predecessor access

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public class Subscription : CorrespondenceFact
    {
        // Roles
        public static Role<Subscriber> RoleSubscriber = new Role<Subscriber>("subscriber");
        public static Role<Magazine> RoleMagazine = new Role<Magazine>("magazine");

        // Queries

        // Predecessors
        private PredecessorObj<Subscriber> _subscriber;
        private PredecessorObj<Magazine> _magazine;

        // Fields

        // Results

        // Business constructor
        public Subscription(
            Subscriber subscriber
            ,Magazine magazine
            )
        {
            _subscriber = new PredecessorObj<Subscriber>(this, RoleSubscriber, subscriber);
            _magazine = new PredecessorObj<Magazine>(this, RoleMagazine, magazine);
        }

        // Hydration constructor
        public Subscription(FactMemento memento)
        {
            _subscriber = new PredecessorObj<Subscriber>(this, RoleSubscriber, memento);
            _magazine = new PredecessorObj<Magazine>(this, RoleMagazine, memento);
        }

        // Predecessor access
        public Subscriber Subscriber
        {
            get { return _subscriber.Fact; }
        }
        public Magazine Magazine
        {
            get { return _magazine.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public class Article : CorrespondenceFact
    {
        // Roles
        public static Role<Magazine> RoleMagazine = new Role<Magazine>("magazine");

        // Queries

        // Predecessors
        private PredecessorObj<Magazine> _magazine;

        // Fields

        // Results

        // Business constructor
        public Article(
            Magazine magazine
            )
        {
            _magazine = new PredecessorObj<Magazine>(this, RoleMagazine, magazine);
        }

        // Hydration constructor
        public Article(FactMemento memento)
        {
            _magazine = new PredecessorObj<Magazine>(this, RoleMagazine, memento);
        }

        // Predecessor access
        public Magazine Magazine
        {
            get { return _magazine.Fact; }
        }

        // Field access

        // Query result access
    }
    
}
