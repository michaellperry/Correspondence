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

        // Predecessors

        // Fields
        [CorrespondenceField]
        private string _identifier;

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
    }
    
    [CorrespondenceType]
    public class Time : CorrespondenceFact
    {
        // Roles

        // Predecessors

        // Fields
        [CorrespondenceField]
        private DateTime _start;

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
    }
    
    [CorrespondenceType]
    public class Frame : CorrespondenceFact
    {
        // Roles
        public static Role<Queue> RoleQueue = new Role<Queue>("queue");
        public static Role<Time> RoleTimestamp = new Role<Time>("timestamp");

        // Predecessors
        private PredecessorObj<Queue> _queue;
        private PredecessorObj<Time> _timestamp;

        // Fields

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
    }
    
    [CorrespondenceType]
    public class Player : CorrespondenceFact
    {
        // Roles

        // Predecessors

        // Fields

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
    }
    
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        // Roles
        public static Role<Player> RolePlayers = new Role<Player>("players");

        // Predecessors
        private PredecessorList<Player> _players;

        // Fields

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
    }
    
}
