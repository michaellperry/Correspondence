using System;
using System.Collections.Generic;
using System.Linq;
using Multinotes.Model;

namespace Multinotes.ViewModels
{
    public class MessageViewModel
    {
        private readonly Message _message;

        public MessageViewModel(Message message)
        {
            _message = message;
        }

        public string Text
        {
            get { return _message.Text; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            MessageViewModel that = obj as MessageViewModel;
            if (that == null)
                return false;
            return Object.Equals(this._message, that._message);
        }

        public override int GetHashCode()
        {
            return _message.GetHashCode();
        }
    }
}
