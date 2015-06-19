using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Correspondence.Data.Spec
{
    public static class RedBlackTreeParameterizedTest
    {
        public static void VerifyRedBlackTree(int[] hashCodes, int target)
        {
            if (hashCodes == null)
                return;

            RedBlackTree tree = new RedBlackTree(new MemoryStream(), new RedBlackTree.NodeCache());
            tree.CheckInvariant();
            List<long> targetIds = new List<long>();
            for (long id = 1; id <= hashCodes.Length; id++)
            {
                int hashCode = hashCodes[id-1];
                if (hashCode == target)
                    targetIds.Add(id);
                tree.AddFact(hashCode, id);
                tree.CheckInvariant();
            }

            var matchingIds = tree.FindFacts(target)
                .OrderBy(id => id)
                .ToList();

            Debug.Assert(matchingIds.Count == targetIds.Count);
            for (int index = 0; index < matchingIds.Count; index++)
                Debug.Assert(matchingIds[index] == targetIds[index]);
        }
    }
}
