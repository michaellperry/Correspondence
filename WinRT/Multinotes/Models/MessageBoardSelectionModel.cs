using Multinotes2.Model;
using UpdateControls.Fields;
using System;

namespace Multinotes.Models
{
    public class MessageBoardSelectionModel
    {
        private Independent<Share> _selectedShare = new Independent<Share>();
        private Independent<string> _topic = new Independent<string>();
        private Independent<string> _text = new Independent<string>();

        public Share SelectedShare
        {
            get { return _selectedShare; }
            set { _selectedShare.Value = value; }
        }

        public string Topic
        {
            get { return _topic; }
            set { _topic.Value = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text.Value = value; }
        }
    }
}
