using System;

namespace Multinotes.Model
{
    public partial class MessageBoard
    {
        public async void SendMessageAsync(string text)
        {
            var domain = await Community.AddFactAsync(new Domain());
            await Community.AddFactAsync(new Message(this, domain, text));
        }
    }
}
