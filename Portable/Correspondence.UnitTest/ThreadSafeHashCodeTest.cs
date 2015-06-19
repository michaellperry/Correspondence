using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Correspondence.Mementos;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class ThreadSafeHashCodeTest
    {
        private const int KnownHashCode = 1611906182;
        private const int HashCount = 2000000;

        private CountdownEvent _threadsRunning;
        private int _computedHashCode = KnownHashCode;

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

        [TestMethod]
        public void TypeHashCodeIsThreadSafe()
        {
            Thread noise = new Thread(NoiseThreadProc);
            Thread test = new Thread(TestThreadProc);
            noise.Start();
            test.Start();
            _threadsRunning.Wait();

            Assert.AreEqual(KnownHashCode, _computedHashCode);
        }

        private void NoiseThreadProc()
        {
            for (int i = 0; i < HashCount; i++)
            {
                int hashCode = new CorrespondenceFactType(Guid.NewGuid().ToString(), 1).GetHashCode();
            }
            _threadsRunning.Signal();
        }

        private void TestThreadProc()
        {
            for (int i = 0; i < HashCount; i++)
            {
                int hashCode = KnownType.GetHashCode();
                if (hashCode != KnownHashCode)
                {
                    _computedHashCode = hashCode;
                    break;
                }
            }
            _threadsRunning.Signal();
        }

        private static CorrespondenceFactType KnownType
        {
            get { return new CorrespondenceFactType("FacetedWorlds.Model.IdentityListShare", 1); }
        }
    }
}
