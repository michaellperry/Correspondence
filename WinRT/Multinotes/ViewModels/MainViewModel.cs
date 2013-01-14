using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Multinotes2.Model;
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
                    where share.MessageBoard != null
                    orderby share.MessageBoard.Topic
                    select new MessageBoardViewModel(share, _selection);
            }
        }

        public MessageBoardViewModel SelectedMessageBoard
        {
            get
            {
                return _selection.SelectedShare == null
                    ? null
                    : new MessageBoardViewModel(_selection.SelectedShare, _selection);
            }
            set
            {
                _selection.SelectedShare = value == null
                    ? null
                    : value.Share;
            }
        }

        public string Topic
        {
            get { return _selection.Topic; }
            set { _selection.Topic = value; }
        }

        public ICommand JoinGroup
        {
            get
            {
                return MakeCommand
                    .Do(async delegate
                    {
                        if (!String.IsNullOrEmpty(_selection.Topic))
                        {
                            Share share = await _individual.JoinMessageBoardAsync(_selection.Topic);
                            _selection.SelectedShare = share;
                            _selection.Topic = null;
                        }
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
                        if (ConfirmLeaveBoard == null || ConfirmLeaveBoard(_selection.SelectedShare.MessageBoard))
                        {
                            _selection.SelectedShare.Leave();
                            _selection.SelectedShare = null;
                        }
                    });
            }
        }

        public ICommand Refresh
        {
            get
            {
                return MakeCommand
                    .Do(delegate
                    {
                        _synchronizationService.Synchronize();
                    });
            }
        }

        public event Func<MessageBoard, bool> ConfirmLeaveBoard;
    }
}
