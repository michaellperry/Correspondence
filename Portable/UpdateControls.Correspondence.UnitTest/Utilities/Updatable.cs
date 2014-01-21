using System;

namespace UpdateControls.Correspondence.UnitTest.Utilities
{
    public class Updatable : IUpdatable
    {
        private readonly Action _action;

        public Updatable(Action action)
        {
            _action = action;
        }

        public void UpdateNow()
        {
            _action();
        }
    }
}
