using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Multinotes.Model;
using Multinotes.Models;
using UpdateControls.XAML;

namespace Multinotes.ViewModels
{
    public class MainViewModel
    {
        private Individual _individual;
        private SynchronizationService _synchronizationService;
        private MessageBoardSelectionModel _selection;

        public MainViewModel(Individual individual, SynchronizationService synhronizationService, MessageBoardSelectionModel selection)
        {
            _individual = individual;
            _synchronizationService = synhronizationService;
            _selection = selection;
        }

        public string Title
        {
            get { return _individual.AnonymousId; }
        }

        public bool Synchronizing
        {
            get { return _synchronizationService.Synchronizing; }
        }

        public string LastException
        {
            get
            {
                Exception lastException = _synchronizationService.LastException;
                return lastException == null
                    ? null
                    : lastException.Message;
            }
        }

        public bool ShowInstructions
        {
            get
            {
                return
                    !_synchronizationService.Synchronizing &&
                    !_individual.MessageBoards.Any();
            }
        }

        public IEnumerable<MessageBoardViewModel> MessageBoards
        {
            get
            {
                return
                    from share in _individual.Shares
                    orderby share.MessageBoard.Topic
                    select new MessageBoardViewModel(share);
            }
        }

        public void SetSelectedMessageBoard(MessageBoardViewModel value)
        {
            _selection.SelectedShare = value == null
                ? null
                : value.Share;
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
                    .When(() =>
                        _selection.SelectedShare != null &&
                        !String.IsNullOrEmpty(_selection.Text))
                    .Do(delegate
                    {
                        _selection.SelectedShare.MessageBoard.SendMessageAsync(_selection.Text);
                        _selection.Text = null;
                    });
            }
        }

        public ICommand LeaveBoard
        {
            get
            {
                return MakeCommand
                    .When(() => _selection.SelectedShare != null)
                    .Do(delegate
                    {
                        if (ConfirmLeaveBoard != null && ConfirmLeaveBoard(_selection.SelectedShare.MessageBoard))
                        {
                            _selection.SelectedShare.Leave();
                            _selection.SelectedShare = null;
                        }
                    });
            }
        }

        public event Func<MessageBoard, bool> ConfirmLeaveBoard;
    }
}
