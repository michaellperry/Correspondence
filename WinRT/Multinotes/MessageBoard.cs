using System;

namespace Multinotes2.Model
{
    public partial class MessageBoard
    {
        public async void SendMessageAsync(string text)
        {
            await Community.AddFactAsync(new Message(this, text));
        }
    }
}
