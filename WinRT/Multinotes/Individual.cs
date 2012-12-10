using System;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence;

namespace Multinotes2.Model
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

        public async Task<Share> JoinMessageBoardAsync(string topic)
        {
            MessageBoard messageBoard = await Community.AddFactAsync(new MessageBoard(topic));
            Share share = await Community.AddFactAsync(new Share(this, messageBoard));
            return share;
        }
    }
}
