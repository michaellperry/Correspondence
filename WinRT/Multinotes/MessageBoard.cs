using System;

namespace Multinotes.Model
{
    public partial class MessageBoard
    {
        public async void SendMessageAsync(string text)
        {
            Domain theDomain = await Community.AddFactAsync(new Domain());
            await Community.AddFactAsync(new Message(this, theDomain, text));
        }
    }
}
