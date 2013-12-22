using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.UnitTest.Model
{
    public partial class Machine
    {
        public async void LogOnUser(string userName)
        {
            await Community.AddFactAsync(new LogOn(await Community.AddFactAsync(new User(userName)), this));
        }

        public void LogOffUser()
        {
            foreach (LogOn logon in ActiveLogOns)
                Community.AddFactAsync(new LogOff(logon));
        }
    }
}
