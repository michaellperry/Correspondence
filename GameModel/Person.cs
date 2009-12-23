using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class Person : CorrespondenceFact
    {
        [CorrespondenceField]
        private Guid _unique;

        public Person()
        {
            _unique = Guid.NewGuid();
        }

        public Person(Memento memento)
        {
        }
    }
}
