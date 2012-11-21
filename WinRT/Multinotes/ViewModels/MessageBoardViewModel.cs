using System;
using System.Collections.Generic;
using System.Linq;
using Multinotes.Model;

namespace Multinotes.ViewModels
{
    public class MessageBoardViewModel
    {
        private readonly Share _share;

        public MessageBoardViewModel(Share share)
        {
            _share = share;
        }

        internal Share Share
        {
            get { return _share; }
        }

        public string Topic
        {
            get { return _share.MessageBoard.Topic; }
        }

        public IEnumerable<MessageViewModel> Messages
        {
            get
            {
                return
                    from message in _share.MessageBoard.Messages
                    select new MessageViewModel(message);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            MessageBoardViewModel that = obj as MessageBoardViewModel;
            if (that == null)
                return false;
            return Object.Equals(this._share, that._share);
        }

        public override int GetHashCode()
        {
            return _share.GetHashCode();
        }
    }
}
