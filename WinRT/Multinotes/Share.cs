using System;
using System.Net;
using System.Windows;

namespace Multinotes.Model
{
    public partial class Share
    {
        public void Leave()
        {
            Community.AddFactAsync(new ShareDelete(this));
        }
    }
}
