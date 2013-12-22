using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Target = UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class PurgeTest : TestBase
    {
        [TestMethod]
        public void QueryExplicitlyExcludesFinishedGames()
        {
            var analyzed = AssertNoError(
                "namespace Chess;                     " +
                "fact Player {                        " +
                "key:                                 " +
                "  unique;                            " +
                "query:                               " +
                "  Game* games {                      " +
                "    Game g : g.players = this        " +
                "      where not g.isFinished         " +
                "  }                                  " +
                "}                                    " +
                "fact Game {                          " +
                "key:                                 " +
                "  unique;                            " +
                "  Player* players;                   " +
                "query:                               " +
                "  bool isFinished {                  " +
                "    exists Outcome o : o.game = this " +
                "  }                                  " +
                "}                                    " +
                "fact Outcome {                       " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    ");

            AssertPlayerExcludesFinishedGames(analyzed);
        }

        [TestMethod]
        public void PurgeClauseIncludedInQuery()
        {
            var analyzed = AssertNoError(
                "namespace Chess;                     " +
                "fact Player {                        " +
                "key:                                 " +
                "  unique;                            " +
                "query:                               " +
                "  Game* games {                      " +
                "    Game g : g.players = this        " +
                "  }                                  " +
                "}                                    " +
                "fact Game {                          " +
                "purge where this.isFinished;         " +
                "key:                                 " +
                "  unique;                            " +
                "  Player* players;                   " +
                "query:                               " +
                "  bool isFinished {                  " +
                "    exists Outcome o : o.game = this " +
                "  }                                  " +
                "}                                    " +
                "fact Outcome {                       " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    ");

            AssertPlayerExcludesFinishedGames(analyzed);
        }

        [TestMethod]
        public void QueryExplicitlyExcludesMovesOfFinishedGames()
        {
            var analyzed = AssertNoError(
                "namespace Chess;                     " +
                "fact Player {                        " +
                "key:                                 " +
                "  unique;                            " +
                "query:                               " +
                "  Move* moves {                      " +
                "    Game g : g.players = this        " +
                "      where not g.isFinished         " +
                "    Move m : m.game = g              " +
                "  }                                  " +
                "}                                    " +
                "fact Game {                          " +
                "key:                                 " +
                "  unique;                            " +
                "  Player* players;                   " +
                "query:                               " +
                "  bool isFinished {                  " +
                "    exists Outcome o : o.game = this " +
                "  }                                  " +
                "}                                    " +
                "fact Move {                          " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    " +
                "fact Outcome {                       " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    ");

            AssertPlayerExcludesMovesOfFinishedGames(analyzed);
        }

        [TestMethod]
        public void PurgeClauseIncludedInIntermediateJoin()
        {
            var analyzed = AssertNoError(
                "namespace Chess;                     " +
                "fact Player {                        " +
                "key:                                 " +
                "  unique;                            " +
                "query:                               " +
                "  Move* moves {                      " +
                "    Move m : m.game.players = this   " +
                "  }                                  " +
                "}                                    " +
                "fact Game {                          " +
                "purge where this.isFinished;         " +
                "key:                                 " +
                "  unique;                            " +
                "  Player* players;                   " +
                "query:                               " +
                "  bool isFinished {                  " +
                "    exists Outcome o : o.game = this " +
                "  }                                  " +
                "}                                    " +
                "fact Move {                          " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    " +
                "fact Outcome {                       " +
                "key:                                 " +
                "  Game game;                         " +
                "}                                    ");

            AssertPlayerExcludesMovesOfFinishedGames(analyzed);
        }

        private static void AssertPlayerExcludesFinishedGames(Target.Analyzed analyzed)
        {
            var player = analyzed.HasClassNamed("Player");
            var games = player.HasQueryNamed("games");
            Assert.AreEqual(1, games.Joins.Count());
            var join = games.Joins.Single();
            Assert.AreEqual(1, join.Conditions.Count());
            var condition = join.Conditions.Single();
            Assert.AreEqual("isFinished", condition.Name);
            Assert.AreEqual(Metadata.ConditionModifier.Negative, condition.Modifier);
        }

        private static void AssertPlayerExcludesMovesOfFinishedGames(Target.Analyzed analyzed)
        {
            var player = analyzed.HasClassNamed("Player");
            var games = player.HasQueryNamed("moves");
            Assert.AreEqual(2, games.Joins.Count());
            var join = games.Joins.ElementAt(0);
            Assert.AreEqual("Game", join.Type);
            Assert.AreEqual(Target.Direction.Successors, join.Direction);
            Assert.AreEqual(1, join.Conditions.Count());
            var condition = join.Conditions.Single();
            Assert.AreEqual("isFinished", condition.Name);
            Assert.AreEqual(Metadata.ConditionModifier.Negative, condition.Modifier);
        }
    }
}
