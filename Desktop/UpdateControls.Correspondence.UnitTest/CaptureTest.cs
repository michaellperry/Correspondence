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
            memento.Data[0] = (byte)bytes.Length;
            memento.Data[1] = 0;
            for (int i = 0; i < bytes.Length; i++)
                memento.Data[i + 2] = bytes[i];

            int hashCode = memento.GetHashCode();
            Assert.AreEqual(0x4be7ff23, hashCode);
        }
    }
}
