using System.ComponentModel;
using Multinotes.Models;
using UpdateControls.XAML;

namespace Multinotes.ViewModels
{
    public class ViewModelLocator : ViewModelBase
    {
        private readonly SynchronizationService _synchronizationService;

        private readonly MessageBoardSelectionModel _selection;

        public ViewModelLocator()
        {
            _synchronizationService = new SynchronizationService();
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                _synchronizationService.Initialize();
            _selection = new MessageBoardSelectionModel();
        }

        public object Main
        {
            get
            {
                return Get(() =>
                    _synchronizationService.Individual == null
                    ? null
                    : ForView.Wrap(new MainViewModel(_synchronizationService.Individual, _synchronizationService, _selection)));
            }
        }

        public object Join
        {
            get
            {
                return Get(() =>
                    _synchronizationService.Individual == null
                    ? null
                    : ForView.Wrap(new JoinMessageBoardViewModel(_selection, _synchronizationService.Individual)));
            }
        }
    }
}
