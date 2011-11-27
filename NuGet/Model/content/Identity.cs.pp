using System;
using System.Linq;
using UpdateControls.Correspondence;

namespace $rootnamespace$
{
    public partial class Identity
    {
        public void SendMessage(string recipientId, string text)
        {
            Identity remoteIdentity = Community.AddFact(new Identity(recipientId));
            Community.AddFact(new Message(this, remoteIdentity, text));
        }

        public bool ToastNotificationEnabled
        {
            get { return !IsToastNotificationDisabled.Any(); }
            set
            {
                if (IsToastNotificationDisabled.Any() && value)
                {
                    Community.AddFact(new EnableToastNotification(IsToastNotificationDisabled));
                }
                else if (!IsToastNotificationDisabled.Any() && !value)
                {
                    Community.AddFact(new DisableToastNotification(this));
                }
            }
        }
    }
}
