using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class QueryDefinitionTest
    {
        [TestMethod]
        public void SuccessorQueriesAreEqual()
        {
            QueryDefinition q1 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void PredecessorQueriesAreEqual()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void TargetTypeDoesntMatter()
        {
            QueryDefinition q1 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", new CorrespondenceFactType("parent", 1), false)))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void PivotDoesntMatter()
        {
            QueryDefinition q1 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, true)))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void PredecessorQueriesAreNotEqualToSuccessorQueries()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void TypeNameMatters()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("childz", 1), "parent", null, false)))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void TypeVersionMatters()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 2), "parent", null, false)))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void RoleNameMatters()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parentz", null, false)))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void MultipleJoinsAreEqual()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void OrderMatters()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void ConditionsAreEqual()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void ConditionsEmptyMatters()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsNotEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false)))))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }

        [TestMethod]
        public void CompoundConditionsAreEqual()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false))))
                    .And().IsNotEmpty(new Query()
                        .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("grandchild", 1), "parent", null, false)))))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false))))
                    .And().IsNotEmpty(new Query()
                        .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("grandchild", 1), "parent", null, false)))))
                .QueryDefinition;

            Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreEqual(q1, q2);
        }

        [TestMethod]
        public void CompoundConditionsMatter()
        {
            QueryDefinition q1 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false))))
                    .And().IsNotEmpty(new Query()
                        .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("grandchild", 1), "parent", null, false)))))
                .QueryDefinition;
            QueryDefinition q2 = new Query()
                .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("child", 1), "parent", null, false)),
                    Condition.WhereIsEmpty(new Query()
                        .JoinPredecessors(new Role(new RoleMemento(new CorrespondenceFactType("parent", 1), "grandparent", null, false))))
                    .And().IsNotEmpty(new Query()
                        .JoinSuccessors(new Role(new RoleMemento(new CorrespondenceFactType("grandchildz", 1), "parent", null, false)))))
                .QueryDefinition;

            Assert.AreNotEqual(q1.GetHashCode(), q2.GetHashCode());
            Assert.AreNotEqual(q1, q2);
        }
    }
}
