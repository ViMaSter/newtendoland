using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace tileeditor.DataFormats
{
    // 
    class FruitData
    {
        // byte-to-3D-object mapping
        public enum FruitMapping
        {
            NONE = 0,

            // Large
            WaterMelon = 1,
            Cantaloupe = 2,
            Bananas = 3,

            // Average
            PinkPeach = 4,
            Tomato = 5,
            Grapes = 6,

            // Small
            Strawberry = 7,
            Pear = 8,           // yeah, I know. the texture-file calls it that
            Banana = 9,

            PresentReferencedButNotUsed = 50,
            Present10Coins = 51,
            Present1Up = 52,
            PresentBlueCheckMark = 53
        };
        
        [DebuggerDisplay("{this.Readable()}")]
        class Fruit
        {
            private byte[] unknown;                             // < 3 bytes
            private byte _ID;                                   // < 1  byte
            private byte[] unknown2;                            // < 64 bytes
            private const int fruitType_LENGTH = 3;
            private IndexResolver fruitType;                    // < 3 bytes (see FruitMapping)
            private byte[] unknown3;                            // < 13 bytes

            public string Readable()
            {
                if (fruitType.IsRange)
                {
                    // when a range is given a fruit is being picked at random using the min/max properties of this.fruitType
                    List<FruitMapping> possibleFruits = Enum.GetValues(typeof(FruitMapping)).Cast<FruitMapping>().Where((n, x) => x >= fruitType.Min && x <= fruitType.Max).ToList();
                    return "Random range: [" + String.Join(", ", possibleFruits) + "]";
                }
                else
                {
                    return ((FruitMapping)fruitType.Value).ToString();
                }
            }
            public byte ID
            {
                get
                {
                    return _ID;
                }
            }

            public static Fruit Load(BinaryReader reader)
            {
                Fruit fruit = new Fruit();
                fruit.unknown = reader.ReadBytes(3);
                fruit._ID = reader.ReadByte();
                fruit.unknown2 = reader.ReadBytes(64);
                fruit.fruitType = IndexResolver.Load(reader, fruitType_LENGTH);
                fruit.unknown3 = reader.ReadBytes(13);

                return fruit;
            }
        }

        private byte[] headerUnknown; // < 16 bytes
        private Dictionary<int, Fruit> payload = new Dictionary<int, Fruit>(61); // 61 is based on the game's default amount of maps (50 in-game + 10 unused maps + tutorial)
        private byte[] footerUnknown; // < 3 bytes
        public static FruitData Load(string pathToYsiExtract)
        {
            FruitData fruitData = new FruitData();

            using (BinaryReader reader = new BinaryReader(File.Open(Path.Combine(pathToYsiExtract, "FruitData.exbin"), FileMode.Open)))
            {
                fruitData.headerUnknown = reader.ReadBytes(16);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    Fruit fruit = Fruit.Load(reader);
                    fruitData.payload[fruit.ID] = fruit;
                }
                Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length, "");
                fruitData.footerUnknown = reader.ReadBytes(13);
            }

            return fruitData;
        }
    }
}