﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NintendoLand.DataFormats
{
    public class FruitData
    {
        // byte-to-3D-object mapping
        public enum FruitType
        {
            // Large
            WaterMelon = 1,
            Cantaloupe = 2,
            Bananas = 3,

            // Average
            Peach = 4,
            Apple = 5,
            Grapes = 6,

            // Small
            Strawberry = 7,
            Pear = 8,           // yeah, I know. the texture-file calls it that
            Orange = 9,

            PresentReferencedButNotUsed = 50,
            Present10Coins = 51,
            Present1Up = 52,
            PresentBlueCheckmark = 53
        };

        [DebuggerDisplay("{this.Readable()}")]
        public class Fruit
        {
            private byte[] unknown;                             // < 3 bytes
            public byte ID => _ID;
            private byte _ID;                                   // < 1  byte
            private byte[] unknown2;                            // < 64 bytes
            private const int fruitType_LENGTH = 3;
            public IndexResolver FruitType => fruitType;
            private IndexResolver fruitType;                    // < 3 bytes (see FruitType)
            private byte[] unknown3;                            // < 13 bytes

            /// <summary>
            /// Default null-constructor when construcing by deserializing a byte-stream with .Load()
            /// </summary>
            private Fruit() { }

            /// <summary>
            /// Creates a fruit with the ID and type specified
            /// 
            /// If the third parameter is specified and not set to FruitType.NONE, random selection using a range is created
            /// </summary>
            /// <param name="ID">ID of the fruit</param>
            /// <param name="fruitType">Type the fruit should be</param>
            /// <param name="fruitTypeRangeEnd">If not FruitType.NONE, the fruit will be a runtime-random fruit ranging from `fruitType` (inclusive) to `fruitTypeRangeEnd` (inclusive)</param>
            /// TODO MH: Replace fruit range with fruit array (via '.' instead of '~'
            public Fruit(byte ID, FruitType fruitType, FruitType fruitTypeRangeEnd)
            {
                if (fruitTypeRangeEnd != (FruitType) (-1))
                {
                    this.fruitType = new IndexResolver((int)fruitType, (int)fruitTypeRangeEnd, (byte)'~');
                }
                else
                {
                    this.fruitType = new IndexResolver((int)fruitType);
                }

                this._ID = ID;
                this.unknown = new byte[] { 0x00, 0x00, 0x00 };
                this.unknown2 = new byte[] { 0x4E, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x7E, 0x39, 0x39, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                this.unknown3 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            public string Readable()
            {
                return string.Join(", ", fruitType.Values.Select(value => (FruitData.FruitType)value));
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

            /// <summary>
            /// Convert an instance of this class into a byte-array representation inside the .exbin-format
            /// </summary>
            /// <param name="bytesLeft">How much bytes we can occupy for this order at max</param>
            public void SerializeExbin(ref List<byte> target, int bytesLeft)
            {
                Debug.Assert(bytesLeft >= 3, "No bytes left for 'unknown'");
                target.AddRange(unknown);

                Debug.Assert(bytesLeft >= 1, "No bytes left for 'ID'");
                target.Add(_ID);

                Debug.Assert(bytesLeft >= 64, "No bytes left for 'unknown2'");
                target.AddRange(unknown2);

                fruitType.SerializeExbin(ref target, fruitType_LENGTH);

                Debug.Assert(bytesLeft >= 13, "No bytes left for 'unknown3'");
                target.AddRange(unknown3);
            }
        }

        public Dictionary<int, Fruit> FruitByID => payload;

        /// <summary>
        /// Update the definition of a fruit
        /// ID is implied from the ._ID property of `fruit`
        /// </summary>
        /// <param name="fruit">Fruit object to write to container</param>
        /// <returns>Returns true, if a fruit with this ID was already present and has been overwritten</returns>
        public bool UpdateFruit(Fruit fruit)
        {
            bool IDexists = payload.ContainsKey(fruit.ID);
            payload[fruit.ID] = fruit;
            return IDexists;
        }

        public int FruitCount => payload.Count;

        private byte[] headerUnknown; // < 16 bytes
        private Dictionary<int, Fruit> payload = new Dictionary<int, Fruit>(90); // 61 is based on the game's default amount of maps (50 in-game + 10 unused maps + tutorial)
        private byte[] footerUnknown; // < 3 bytes
        public static FruitData Load(string pathToYsiExtract)
        {
            FruitData fruitData = new FruitData();

            FileStream fs = new FileStream(Path.Combine(pathToYsiExtract, "FruitData.exbin"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (BinaryReader reader = new BinaryReader(fs))
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

        /// <summary>
        /// Convert an instance of this class into a byte-array representation inside the .exbin-format
        /// </summary>
        /// <param name="bytesLeft">How much bytes we can occupy for this order at max</param>
        public void SerializeExbin(ref List<byte> target, int bytesLeft)
        {
            Debug.Assert(bytesLeft >= 15, "No bytes left for 'headerUnknown'");
            target.AddRange(headerUnknown);
            bytesLeft -= 15;

            foreach (KeyValuePair<int, Fruit> fruitByID in payload)
            {
                fruitByID.Value.SerializeExbin(ref target, 84);
                bytesLeft -= 84;
            }

            Debug.Assert(bytesLeft >= 3, "No bytes left for 'unknown2'");
            target.AddRange(footerUnknown);
            bytesLeft -= 3;
        }
    }
}