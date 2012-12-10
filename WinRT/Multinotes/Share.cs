using System;
using System.Net;
using System.Windows;

namespace Multinotes2.Model
{
    public partial class Share
    {
        public void Leave()
        {
            Community.AddFactAsync(new ShareDelete(this));
        }
    }
}
