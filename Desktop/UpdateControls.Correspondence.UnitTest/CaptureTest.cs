using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class CaptureTest
    {
        [TestMethod]
        public void HashCodeIsConsistent()
        {
            FactMemento memento = new FactMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Conference", 1));
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes("B1E0F2BB4CF24C2492B872112042797E");
            memento.Data = new byte[bytes.Length + 2];
            memento.Data[0] = 0;
            memento.Data[1] = (byte)bytes.Length;
            for (int i = 0; i < bytes.Length; i++)
                memento.Data[i + 2] = bytes[i];

            int hashCode = memento.GetHashCode();
            Assert.AreEqual(0x03f5afb5, hashCode);
        }

        [TestMethod]
        public void PredecessorHashCodeIsConsistentToo()
        {
            FactMemento memento = new FactMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Attendee", 1));
            memento.Data = new byte[0];
            memento.AddPredecessor(new RoleMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Identity", 1), "identity", null, false), new FactID { key = 1 }, false);
            memento.AddPredecessor(new RoleMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Conference", 1), "conference", null, false), new FactID { key = 2 }, false);

            int hashCode = memento.GetHashCode();
            Assert.AreEqual(-1244599490, hashCode);
        }

        [TestMethod]
        public void SmallChangeInRoleNameCausesBigChangeInHash()
        {
            FactMemento memento = new FactMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Attendee", 1));
            memento.Data = new byte[0];
            memento.AddPredecessor(new RoleMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Identity", 1), "identitz", null, false), new FactID { key = 1 }, false);
            memento.AddPredecessor(new RoleMemento(new CorrespondenceFactType("FacetedWorlds.MyCon.Model.Conference", 1), "conference", null, false), new FactID { key = 2 }, false);

            int hashCode = memento.GetHashCode();
            Assert.AreEqual(752466564, hashCode);
        }
    }
}
