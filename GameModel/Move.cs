using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class Move : CorrespondenceFact
    {
        public static Role<Player> RolePlayer = new Role<Player>("player");

        private PredecessorObj<Player> _player;

        [CorrespondenceField]
        private int _index;

        [CorrespondenceField]
        private int _square;

        public Move(Player player, int index, int square)
        {
            _player = new PredecessorObj<Player>(this, RolePlayer, player);
            _index = index;
            _square = square;
        }

        public Move(FactMemento memento)
        {
            _player = new PredecessorObj<Player>(this, RolePlayer, memento);
        }

        public Player Player
        {
            get { return _player.Fact; }
        }

        public int Index
        {
            get { return _index; }
        }

        public int Square
        {
            get { return _square; }
        }
    }
}
