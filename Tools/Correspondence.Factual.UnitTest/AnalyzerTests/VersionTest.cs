﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class VersionTest : TestBase
    {
        [TestMethod]
        public void FileWithNoHeaderUsesHashAsVersion()
        {
            string code =
                "namespace Model;   " +
                "fact Player {      " +
                "key:               " +
                "  unique;          " +
                "}                  " +
                "fact Game {        " +
                "key:               " +
                "  unique;          " +
                "  Player* players; " +
                "}                  ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(-1755949158, version);
        }

        [TestMethod]
        public void FileWithLegacyHeaderUsesOneAsVersion()
        {
            string code =
                "namespace Model;   " +
                "version legacy;    " +
                "fact Player {      " +
                "key:               " +
                "  unique;          " +
                "}                  " +
                "fact Game {        " +
                "key:               " +
                "  unique;          " +
                "  Player* players; " +
                "}                  ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(1, version);
        }

        [TestMethod]
        public void PublishingChangesTheVersionNumber()
        {
            string code =
                "namespace Model;           " +
                "fact Player {              " +
                "key:                       " +
                "  unique;                  " +
                "}                          " +
                "fact Game {                " +
                "key:                       " +
                "  unique;                  " +
                "  publish Player* players; " +
                "}                          ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(-1755949154, version);
        }

        [TestMethod]
        public void CardinalityChangesTheVersionNumber()
        {
            string code =
                "namespace Model;           " +
                "fact Player {              " +
                "key:                       " +
                "  unique;                  " +
                "}                          " +
                "fact Game {                " +
                "key:                       " +
                "  unique;                  " +
                "  Player players;          " +
                "}                          ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(-1755949150, version);
        }

        [TestMethod]
        public void PredecessorNameChangesTheVersionNumber()
        {
            string code =
                "namespace Model;           " +
                "fact Player {              " +
                "key:                       " +
                "  unique;                  " +
                "}                          " +
                "fact Game {                " +
                "key:                       " +
                "  unique;                  " +
                "  Player* opponents;       " +
                "}                          ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(528072746, version);
        }

        [TestMethod]
        public void FieldChangesTheVersionNumber()
        {
            string code =
                "namespace Model;           " +
                "fact Player {              " +
                "key:                       " +
                "  unique;                  " +
                "}                          " +
                "fact Game {                " +
                "key:                       " +
                "  unique;                  " +
                "  time timePlayed;         " +
                "  Player* opponents;       " +
                "}                          ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(-1936144882, version);
        }

        [TestMethod]
        public void FieldNameDoesNotChangeTheVersionNumber()
        {
            string code =
                "namespace Model;           " +
                "fact Player {              " +
                "key:                       " +
                "  unique;                  " +
                "}                          " +
                "fact Game {                " +
                "key:                       " +
                "  unique;                  " +
                "  time timeOfGame;         " +
                "  Player* opponents;       " +
                "}                          ";

            var analyzed = AssertNoError(code);
            var game = analyzed.HasClassNamed("Game");
            int version = game.Version;

            Assert.AreEqual(-1936144882, version);
        }

        [TestMethod]
        public void FactInFileWithHeaderHasVersion1()
        {
            string code =
                "namespace model; " +
                "version legacy;  " +
                "fact Fact { key: }";
            var analyzed = AssertNoError(code);
            var fact = analyzed.HasClassNamed("Fact");
            Assert.AreEqual(1, fact.Version);
        }

        [TestMethod]
        public void ExplicitVersionNumberOverridesHeader()
        {
            string code =
                "namespace model; " +
                "version legacy;  " +
                "fact Fact -42 { key: }";
            var analyzed = AssertNoError(code);
            var fact = analyzed.HasClassNamed("Fact");
            Assert.AreEqual(-42, fact.Version);
        }

        [TestMethod]
        public void ExplicitVersionNumberOverridesComputedVersionNumber()
        {
            string code =
                "namespace model; " +
                "fact Fact -42 { key: }";
            var analyzed = AssertNoError(code);
            var fact = analyzed.HasClassNamed("Fact");
            Assert.AreEqual(-42, fact.Version);
        }
    }
}
