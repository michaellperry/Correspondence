using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Correspondence.UnitTest.Model
{
    public partial class Machine
    {
        public async Task LogOnUser(string userName)
        {
            await Community.AddFactAsync(new LogOn(await Community.AddFactAsync(new User(userName)), this));
        }

        public async Task LogOffUser()
        {
            foreach (LogOn logon in ActiveLogOns)
                await Community.AddFactAsync(new LogOff(logon));
        }
    }
}
