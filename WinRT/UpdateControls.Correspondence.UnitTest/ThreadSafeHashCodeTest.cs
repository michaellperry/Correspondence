using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class ThreadSafeHashCodeTest
    {
        private const int KnownHashCode = 1611906182;

        private CountdownEvent _threadsRunning;

        [TestInitialize]
        public void Initialize()
        {
            _threadsRunning = new CountdownEvent(2);
        }

        [TestMethod]
        public void HashCodeOfKnownType()
        {
            int hashCode = KnownType.GetHashCode();
            Assert.AreEqual(KnownHashCode, hashCode);
        }

        private static CorrespondenceFactType KnownType
        {
            get { return new CorrespondenceFactType("FacetedWorlds.Model.IdentityListShare", 1); }
        }
    }
}
