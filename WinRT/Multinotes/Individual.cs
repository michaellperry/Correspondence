using System;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence;

namespace Multinotes.Model
{
    public partial class Individual
    {
        public bool ToastNotificationEnabled
        {
            get { return IsToastNotificationEnabled.Any(); }
            set
            {
                if (IsToastNotificationEnabled.Any() && !value)
                {
                    Community.AddFactAsync(new DisableToastNotification(IsToastNotificationEnabled));
                }
                else if (!IsToastNotificationEnabled.Any() && value)
                {
                    Community.AddFactAsync(new EnableToastNotification(this));
                }
            }
        }

        public async Task<MessageBoard> JoinMessageBoardAsync(string topic)
        {
            MessageBoard messageBoard = await Community.AddFactAsync(new MessageBoard(topic));
            await Community.AddFactAsync(new Share(this, messageBoard));
            return messageBoard;
        }
    }
}
