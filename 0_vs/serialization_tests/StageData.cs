using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NintendoLand.Tests.DataFormats
{
    [TestFixture]
    class StageData
    {
        string pathToYsiExtract = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../../../5_azurescripts/testresources/";

        [SetUp]
        public void SetUpFilePaths()
        {
            Assert.IsTrue(
                File.Exists(pathToYsiExtract + "StageData.exbin"),
                "StageData.exbin needs to exist inside /5_azurescripts/testresources/ for tests to be run! (More info in README.MD -> 'Running tests')"
            );
        }

        /// <summary>
        /// Load the currently existing StageData.exbin data, deserialize it, serialize it and compare it on a byte-level
        /// </summary>
        [Test]
        public void SerializeCleanExbin()
        {
            NintendoLand.DataFormats.StageData stageData = NintendoLand.DataFormats.StageData.Load(pathToYsiExtract);

            Assert.AreEqual(51, stageData.LevelCount, "Default game files require exactly 50 levels + tutorial level"); // while there are 61 MapData**.exbin files, 10 of them are unused by not being present in StageData.exbin - hence 51

            List<byte> serializedData = new List<byte>();
            stageData.SerializeExbin(ref serializedData, 20 + (NintendoLand.DataFormats.StageData.Stage.BYTES_REQUIRED * 61));

            #if DEBUG
            // Write inspectable files during debug builds to allow diffing of generated output and regular files in external tools
            File.WriteAllText(pathToYsiExtract + "StageData.exbin" + ".gen", string.Empty);
            File.WriteAllBytes(pathToYsiExtract + "StageData.exbin" + ".gen", serializedData.ToArray());
            #endif

            CollectionAssert.AreEqual(
                File.ReadAllBytes(Path.Combine(pathToYsiExtract, "StageData.exbin")),
                serializedData
            );
        }

        /// <summary>
        /// Load the currently existing StageData.exbin data, make modifications and ensure updating fruits changes the bytes returned
        /// </summary>
        [Test]
        public void SerializeDirtyExbin()
        {
            NintendoLand.DataFormats.StageData stageData = NintendoLand.DataFormats.StageData.Load(pathToYsiExtract);

            NintendoLand.DataFormats.StageData.Stage stage01 = stageData.GetLevelByID(1);
            stage01.backgroundID = new NintendoLand.DataFormats.IndexResolver(8);
            stageData.UpdateLevelByID(1, stage01);

            List<byte> serializedDefaultStageData = new List<byte>();
            stageData.SerializeExbin(ref serializedDefaultStageData, 20 + (NintendoLand.DataFormats.StageData.Stage.BYTES_REQUIRED * 61));

            CollectionAssert.AreNotEqual(
                File.ReadAllBytes(Path.Combine(pathToYsiExtract, "StageData.exbin")),
                serializedDefaultStageData
            );
        }

        /// <summary>
        /// Load the currently existing StageData.exbin data and ensure .UpdateFruit() returns true for modifications and false for newly added fruit
        /// </summary>
        [Test]
        public void InsertUpdateFruitReturnValue()
        {
            NintendoLand.DataFormats.StageData stageData = NintendoLand.DataFormats.StageData.Load(pathToYsiExtract);
            Assert.IsTrue(stageData.UpdateLevelByID(1, new NintendoLand.DataFormats.StageData.Stage()));
            Assert.IsFalse(stageData.UpdateLevelByID(200, new NintendoLand.DataFormats.StageData.Stage()));
        }
    }
}
