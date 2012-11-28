using System.Windows.Input;
using Multinotes.Model;
using Multinotes.Models;
using UpdateControls.XAML;

namespace Multinotes.ViewModels
{
    public class JoinMessageBoardViewModel
    {
        private readonly MessageBoardSelectionModel _selection;
        private readonly Individual _individual;

        public JoinMessageBoardViewModel(MessageBoardSelectionModel selection, Individual individual)
        {
            _selection = selection;
            _individual = individual;
        }

        public string Topic
        {
            get { return _selection.Topic; }
            set { _selection.Topic = value; }
        }

        public ICommand Join
        {
            get
            {
                return MakeCommand
                    .When(() => !string.IsNullOrWhiteSpace(_selection.Topic))
                    .Do(delegate
                    {
                        var share = _individual.JoinMessageBoardAsync(_selection.Topic);
                        _selection.Topic = null;
                    });
            }
        }
    }
}
