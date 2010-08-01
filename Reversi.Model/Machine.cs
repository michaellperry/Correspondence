using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model
{
    public partial class Machine
    {
        public void LogOnUser(string userName)
        {
            Community.AddFact(new LogOn(Community.AddFact(new User(userName)), this));
        }

        public void LogOffUser()
        {
            foreach (LogOn logon in ActiveLogOns)
                Community.AddFact(new LogOff(logon));
        }
    }
}
