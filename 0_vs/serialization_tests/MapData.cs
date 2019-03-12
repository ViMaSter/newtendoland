using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace tileeditor.Tests.DataFormats
{
    [TestFixture]
    class MapData
    {
        string pathToYsiExtract = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../../../5_azurescripts/testresources/";

        void SetUpFilePaths()
        {
            for (int i = 0; i < 60; i++)
            {
                string fileName = "MapData" + i.ToString("D2") + ".exbin";
                Assert.IsTrue(
                    File.Exists(pathToYsiExtract + fileName),
                    fileName + " needs to exist inside /5_azurescripts/testresources/ for tests to be run! (More info in README.MD -> 'Running tests')"
                );
            }
            {
                // exception for tutorial stage
                string fileName = "MapData99.exbin";
                Assert.IsTrue(
                    File.Exists(pathToYsiExtract + fileName),
                    fileName + "MapData.exbin needs to exist inside /5_azurescripts/testresources/ for tests to be run! (More info in README.MD -> 'Running tests')"
                );
            }
        }

        void SetUpTileTypeRegistrar()
        {
            TileTypes.Registrar.Populate();
        }

        [SetUp]
        public void SetUp()
        {
            SetUpFilePaths();
            SetUpTileTypeRegistrar();
        }

        [TearDown]
        public void TearDown()
        {
            TileTypes.Registrar.TearDown();
        }

        /// <summary>
        /// Load the currently existing MapData.exbin data, deserialize it, serialize it and compare it on a byte-level
        /// </summary>
        [Test]
        public void SerializeCleanExbin()
        {
            IEnumerable<string> maps = Directory.EnumerateFiles(pathToYsiExtract, "MapData*.exbin");

            int levelCount = 0;
            foreach (string map in maps)
            {
                tileeditor.DataFormats.MapData mapData = tileeditor.DataFormats.MapData.Load(map);
                List<byte> serializedData = new List<byte>();
                mapData.SerializeExbin(ref serializedData,
                    16 + 4 + 18 +   // header
                    TileTypes.RotatingObjectHeader.BYTES_PER_CONTAINER + // rotating objects header
                    (tileeditor.DataFormats.MapData.ROWS_TOTAL * tileeditor.DataFormats.MapData.COLUMNS_TOTAL * (1 + tileeditor.DataFormats.MapData.CELL_TILE_PADDING )) // payload of actual cells
                ); // magic number is the max length of a default MapData.exbin-file

                File.WriteAllText(map + ".gen", string.Empty);
                File.WriteAllBytes(map + ".gen", serializedData.ToArray());

                CollectionAssert.AreEqual(
                    File.ReadAllBytes(map),
                    serializedData
                );

                levelCount++;
            }

            Assert.AreEqual(61, levelCount, "Default game files require exactly 50 levels + 10 used levels + tutorial level");
        }

        /// <summary>
        /// Load the currently existing MapData.exbin data, make modifications and ensure updating fruits changes the bytes returned
        /// </summary>
        [Test]
        public void SerializeDirtyExbin()
        {
            IEnumerable<string> maps = Directory.EnumerateFiles(pathToYsiExtract, "MapData*.exbin");
            foreach (string map in maps)
            {
                tileeditor.DataFormats.MapData mapData = tileeditor.DataFormats.MapData.Load(map);
                List<byte> serializedData = new List<byte>();
                mapData.SetItem(0, 0, TileTypes.Registrar.GetTypeByMemoryIdentifier('G'));
                mapData.SerializeExbin(ref serializedData,
                    16 + 4 + 18 +   // header
                    TileTypes.RotatingObjectHeader.BYTES_PER_CONTAINER + // rotating objects header
                    (tileeditor.DataFormats.MapData.ROWS_TOTAL * tileeditor.DataFormats.MapData.COLUMNS_TOTAL * (1 + tileeditor.DataFormats.MapData.CELL_TILE_PADDING)) // payload of actual cells
                ); // magic number is the max length of a default MapData.exbin-file

                CollectionAssert.AreNotEqual(
                    File.ReadAllBytes(map),
                    serializedData
                );
            }
        }
    }
}
