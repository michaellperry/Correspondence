using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class GameQueue : CorrespondenceFact
    {
        [CorrespondenceField]
        private string _identifier;

        public GameQueue(string identifier)
        {
            _identifier = identifier;
        }

        public GameQueue(Memento memento)
        {
        }
    }
}
