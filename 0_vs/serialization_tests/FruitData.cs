using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace tileeditor.Tests.DataFormats
{
    [TestFixture]
    class FruitData
    {
        string pathToYsiExtract = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../../../5_azurescripts/testresources/";

        [SetUp]
        public void SetUpFilePaths()
        {
            Assert.IsTrue(
                File.Exists(pathToYsiExtract + "FruitData.exbin"),
                "FruitData.exbin needs to exist inside /5_azurescripts/testresources/ for tests to be run! (More info in README.MD -> 'Running tests')"
            );
        }

        /// <summary>
        /// Load the currently existing FruitData.exbin data, deserialize it, serialize it and compare it on a byte-level
        /// </summary>
        [Test]
        public void SerializeCleanExbin()
        {
            tileeditor.DataFormats.FruitData fruitData = tileeditor.DataFormats.FruitData.Load(pathToYsiExtract);
            List<byte> serializedData = new List<byte>();
            fruitData.SerializeExbin(ref serializedData, 16 + 84 * 90 + 3); // magic number is the max length of the default FruitData.exbin-file

            CollectionAssert.AreEqual(
                File.ReadAllBytes(Path.Combine(pathToYsiExtract, "FruitData.exbin")),
                serializedData
            );
        }

        /// <summary>
        /// Load the currently existing FruitData.exbin data, make modifications and ensure updating fruits changes the bytes returned
        /// </summary>
        [Test]
        public void SerializeDirtyExbin()
        {
            tileeditor.DataFormats.FruitData fruitData = tileeditor.DataFormats.FruitData.Load(pathToYsiExtract);
            fruitData.UpdateFruit(new tileeditor.DataFormats.FruitData.Fruit(10, tileeditor.DataFormats.FruitData.FruitMapping.Bananas, tileeditor.DataFormats.FruitData.FruitMapping.Strawberry));
            List<byte> serializedData = new List<byte>();
            fruitData.SerializeExbin(ref serializedData, 16 + 84 * 90 + 3); // magic number is the max length of the default FruitData.exbin-file (16 [header] + 84 [one fruit definition] * 90 + 3 [footer])

            CollectionAssert.AreNotEqual(
                File.ReadAllBytes(Path.Combine(pathToYsiExtract, "FruitData.exbin")),
                serializedData
            );
        }

        /// <summary>
        /// Load the currently existing FruitData.exbin data, make modifications and ensure updating fruits changes the bytes returned
        /// </summary>
        [Test]
        public void SerializeFruitSanityTest()
        {
            tileeditor.DataFormats.FruitData fruitData = tileeditor.DataFormats.FruitData.Load(pathToYsiExtract);

            List<byte> serializedDefaultFruit = new List<byte>(84);
            fruitData.FruitByID[10].SerializeExbin(ref serializedDefaultFruit, 84);

            {
                // ensure deserialized data is identical
                tileeditor.DataFormats.FruitData.Fruit waterMelonID10 = new tileeditor.DataFormats.FruitData.Fruit(10, tileeditor.DataFormats.FruitData.FruitMapping.WaterMelon);

                Assert.AreEqual(fruitData.FruitByID[10].ID, waterMelonID10.ID);
                Assert.AreEqual(fruitData.FruitByID[10].FruitType, waterMelonID10.FruitType);

                // ensure serialized data is identical
                List<byte> serializedCreatedFruit = new List<byte>(84);
                waterMelonID10.SerializeExbin(ref serializedCreatedFruit, 84);

                CollectionAssert.AreEqual(serializedDefaultFruit, serializedCreatedFruit);
            }

            // changed ID
            {
                tileeditor.DataFormats.FruitData.Fruit waterMelonID11 = new tileeditor.DataFormats.FruitData.Fruit(11, tileeditor.DataFormats.FruitData.FruitMapping.WaterMelon);

                Assert.AreNotEqual(fruitData.FruitByID[10].ID, waterMelonID11.ID);
                Assert.AreEqual(fruitData.FruitByID[10].FruitType, waterMelonID11.FruitType);

                // ensure changed serialized data is different
                List<byte> serializedCreatedFruit = new List<byte>(84);
                waterMelonID11.SerializeExbin(ref serializedCreatedFruit, 84);

                CollectionAssert.AreNotEqual(serializedDefaultFruit, serializedCreatedFruit);
            }

            // changed type
            {
                tileeditor.DataFormats.FruitData.Fruit bananasID10 = new tileeditor.DataFormats.FruitData.Fruit(10, tileeditor.DataFormats.FruitData.FruitMapping.Bananas);

                Assert.AreEqual(fruitData.FruitByID[10].ID, bananasID10.ID);
                Assert.AreNotEqual(fruitData.FruitByID[10].FruitType, bananasID10.FruitType);

                // ensure changed serialized data is different
                List<byte> serializedChangedFruit = new List<byte>(84);
                bananasID10.SerializeExbin(ref serializedChangedFruit, 84);

                CollectionAssert.AreNotEqual(serializedDefaultFruit, serializedChangedFruit);
            }

            // changed type to range
            {
                tileeditor.DataFormats.FruitData.Fruit waterMelonAppleID10 = new tileeditor.DataFormats.FruitData.Fruit(10, tileeditor.DataFormats.FruitData.FruitMapping.WaterMelon, tileeditor.DataFormats.FruitData.FruitMapping.Apple);

                Assert.AreEqual(fruitData.FruitByID[10].ID, waterMelonAppleID10.ID);
                Assert.AreNotEqual(fruitData.FruitByID[10].FruitType, waterMelonAppleID10.FruitType);

                // ensure changed serialized data is different
                List<byte> serializedChangedFruit = new List<byte>(84);
                waterMelonAppleID10.SerializeExbin(ref serializedChangedFruit, 84);

                CollectionAssert.AreNotEqual(serializedDefaultFruit, serializedChangedFruit);
            }
        }

        /// <summary>
        /// Load the currently existing FruitData.exbin data and ensure .UpdateFruit() returns true for modifications and false for newly added fruit
        /// </summary>
        [Test]
        public void InsertUpdateFruitReturnValue()
        {
            tileeditor.DataFormats.FruitData fruitData = tileeditor.DataFormats.FruitData.Load(pathToYsiExtract);
            Assert.IsTrue(fruitData.UpdateFruit(new tileeditor.DataFormats.FruitData.Fruit(10, tileeditor.DataFormats.FruitData.FruitMapping.Bananas, tileeditor.DataFormats.FruitData.FruitMapping.Strawberry)));
            Assert.IsFalse(fruitData.UpdateFruit(new tileeditor.DataFormats.FruitData.Fruit(200, tileeditor.DataFormats.FruitData.FruitMapping.Bananas, tileeditor.DataFormats.FruitData.FruitMapping.Strawberry)));
        }
    }
}
