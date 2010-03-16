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


        [CorrespondenceField]
        private string _identifier;

        public Queue(
            string identifier
            )
        {
            _identifier = identifier;
        }

        public Queue(FactMemento memento)
        {
        }


        public string identifier
        {
            get { return _identifier; }
        }
    }
    
    [CorrespondenceType]
    public class Time : CorrespondenceFact
    {


        [CorrespondenceField]
        private DateTime _start;

        public Time(
            DateTime start
            )
        {
            _start = start;
        }

        public Time(FactMemento memento)
        {
        }


        public DateTime start
        {
            get { return _start; }
        }
    }
    
    [CorrespondenceType]
    public class Frame : CorrespondenceFact
    {
        public static Role<Queue> Role_queue = new Role<Queue>("queue");
        public static Role<Time> Role_timestamp = new Role<Time>("timestamp");

        private PredecessorObj<Queue> _queue;
        private PredecessorObj<Time> _timestamp;


        public Frame(
            Queue queue
            ,Time timestamp
            )
        {
            _queue = new PredecessorObj<Queue>(this, Role_queue, queue);
            _timestamp = new PredecessorObj<Time>(this, Role_timestamp, timestamp);
        }

        public Frame(FactMemento memento)
        {
            _queue = new PredecessorObj<Queue>(this, Role_queue, memento);
            _timestamp = new PredecessorObj<Time>(this, Role_timestamp, memento);
        }

        public Queue queue
        {
            get { return _queue.Fact; }
        }
        public Time timestamp
        {
            get { return _timestamp.Fact; }
        }

    }
    
    [CorrespondenceType]
    public class Player : CorrespondenceFact
    {



        public Player(
            )
        {
        }

        public Player(FactMemento memento)
        {
        }


    }
    
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        public static Role<Player> Role_players = new Role<Player>("players");

        private PredecessorList<Player> _players;


        public Game(
            IEnumerable<Player> players
            )
        {
            _players = new PredecessorList<Player>(this, Role_players, players);
        }

        public Game(FactMemento memento)
        {
            _players = new PredecessorList<Player>(this, Role_players, memento);
        }

        public IEnumerable<Player> players
        {
            get { return _players; }
        }
     
    }
    
}
