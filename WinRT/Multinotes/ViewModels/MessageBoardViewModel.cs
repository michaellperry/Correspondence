using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Multinotes.Model;
using Multinotes.Models;
using UpdateControls.XAML;

namespace Multinotes.ViewModels
{
    public class MessageBoardViewModel
    {
        private readonly Share _share;
        private readonly MessageBoardSelectionModel _selection;
        
        public MessageBoardViewModel(Share share, MessageBoardSelectionModel selection)
        {
            _share = share;
            _selection = selection;
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

        public string Text
        {
            get { return _selection.Text; }
            set { _selection.Text = value; }
        }

        public ICommand SendMessage
        {
            get
            {
                return MakeCommand
                    .Do(delegate
                    {
                        if (_selection.SelectedShare != null &&
                            !String.IsNullOrEmpty(_selection.Text))
                        {
                            _selection.SelectedShare.MessageBoard.SendMessageAsync(_selection.Text);
                            _selection.Text = null;
                        }
                    });
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
