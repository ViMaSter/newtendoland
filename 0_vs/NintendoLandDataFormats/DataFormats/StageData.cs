using NintendoLand.TileTypes;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NintendoLand.DataFormats
{
    public class StageData
    {
        public class Stage
        {
            [System.Flags]
            enum ContentFlag
            {
                NONE = 0,
                NormalGoal = 'N',
                SpecialGoal = 'V',
                Unknown1 = 'H',
                Unknown2 = 'B',
            };

            public enum PepperOrSwitchFlag
            {
                NONE = 0,
                Switch = '1',
                Pepper = '2',
                Unset = 0x30
            };

            enum MovementPattern
            {
                NONE = 0,
                SmallCircle = 4,
                LargeCircle = 5,
                HorizontalQuad_Reg = 6,
                HorizontalQuad_Offset = 7,
                CircleSize0_Reg = 8,
                CircleSize1_Reg = 9,
                CircleSize2_Reg = 10,
                CircleSize3_Reg = 11,
                CircleSize0_Offset = 12,
                CircleSize1_Offset = 13,
                CircleSize2_Offset = 14,
                BigBee_SmallCircle = 15,
                LargeCircle2 = 16,
                BottomLeftToTopRight_Offset0 = 17,
                BottomLeftToTopRight_Offset1 = 18,
                BottomLeftToTopRight_Offset2 = 19,
                VerticalQuad_Reg = 20,
                VerticalQuad_Offset = 21,
                CircleSize4_Offset0 = 25,
                CircleSize4_Offset1 = 26,
                CircleSize4_Offset2 = 27,
                CircleSize5_Offset0 = 28,
                CircleSize5_Offset1 = 29,
                CircleSize5_Offset2 = 30,
                CircleSize6_Offset0 = 31,
                CircleSize6_Offset1 = 32,
                CircleSize6_Offset2 = 33,
                CircleSize7_Offset0 = 34,
                CircleSize7_Offset1 = 35,
                CircleSize7_Offset2 = 36
            };

            public static readonly byte[] HOLE_DEFINITION_KEYS = { (byte)'a', (byte)'b', (byte)'c'};
            private const int MAXIMUM_CONTENT_FLAGS = 6;
            private const int SWITCHORPEPPER_DEFINITIONS = 16;
            private const int MOVEMENTPATTERN_DEFINITIONS = 16;
            private const int FRUIT_ORDER_DEFINITIONS = 16;
            private const int FRUIT_ASSOCIATIONS = 32;
            public const int BYTES_REQUIRED = 956;

            // this seems to determine which textboxes are shown after map start
            // before a player gains control over the level
            public byte[] tutorialText1;                       // < 2 bytes
            public byte[] tutorialText2;                       // < 2 bytes
            // potential buffer for tutorial texts
            private byte[] tutorialTextbuffer;                  // < 2 bytes
            // this is always set to [0xC3, 0x50] in default levels
            // @TODO VM play with this value a bit
            private byte[] unknownCP;                           // < 2 bytes
            // this is always set to [0x00, 0x00] in default levels
            // @TODO VM play with this value a bit
            private byte[] unknownPostCP;                       // < 3 bytes
            private byte _ID;                                   // < 1 byte
            public byte ID => _ID;

            // loaded from Ysi_Cmn.pack / Common/Model/Ysi_Field.szs / gsys.bfres / Textures -> Field*
            // @TODO VM build renderer/selector for this
            // @TODO VM check if a range could be used here (only direct values, but structure supports more)
            private const int backgroundID_LENGTH = 8;
            public IndexResolver backgroundID;
            // unknown - definitively unrelated to the door                                                                         
            private const int notDoor_LENGTH = 8;
            private IndexResolver notDoor;
            // BUFFER < 6 bytes
            // single or range of potential teleports?                                                                              
            private const int teleportIndices_LENGTH = 8;
            private IndexResolver teleportIndices;
            private const int holeDefinitions_LENGTH = 4;
            public Dictionary<byte, NintendoLand.TileTypes.Hole.Size> holeDefinitions = new Dictionary<byte, TileTypes.Hole.Size>(3);
            private const int contentFlag_LENGTH = 2;
            private List<ContentFlag> contentFlags = new List<ContentFlag>();
            private const int switchOrPepperDefinitions_LENGTH = 8;
            public PepperOrSwitchFlag[] switchOrPepperDefinitions = new PepperOrSwitchFlag[SWITCHORPEPPER_DEFINITIONS];
            private const int movementPattern_LENGTH = 12;
            private IndexResolver[] movementPatterns = new IndexResolver[MOVEMENTPATTERN_DEFINITIONS];
            // related to fruit order?                                                                                              
            private const int orderedFruitDefinition_LENGTH = 12;
            private IndexResolver[] orderedFruitDefinition = new IndexResolver[FRUIT_ORDER_DEFINITIONS];
            // resolved an index to a specific fruit (See FruitData)                                                                
            private const int fruitAssociations_LENGTH = 12;
            private IndexResolver[] fruitAssociations = new IndexResolver[FRUIT_ASSOCIATIONS];

            private Stage() { }

            public static Stage CreateBlankDefault(int index)
            {
                return new Stage()
                {
                    _ID = (byte)index,
                    backgroundID = new IndexResolver(3),
                    contentFlags = new List<ContentFlag>() {ContentFlag.NormalGoal},
                    fruitAssociations = Enumerable.Range(0, Stage.FRUIT_ASSOCIATIONS).Select(i=>new IndexResolver(13)).ToArray(),
                    holeDefinitions = Stage.HOLE_DEFINITION_KEYS.ToDictionary(keyByte => keyByte, keyByte => Hole.Size.Small),
                    movementPatterns = Enumerable.Range(0, Stage.MOVEMENTPATTERN_DEFINITIONS).Select(i => new IndexResolver(0)).ToArray(),
                    orderedFruitDefinition = Enumerable.Range(0, Stage.MOVEMENTPATTERN_DEFINITIONS).Select(i => new IndexResolver(0)).ToArray(),
                    switchOrPepperDefinitions = Enumerable.Range(0, Stage.SWITCHORPEPPER_DEFINITIONS).Select(i => PepperOrSwitchFlag.Unset).ToArray(),
                    notDoor = new IndexResolver(20),
                    teleportIndices = new IndexResolver(index),
                    tutorialText1 = new byte[] { 0xFF, 0xFF },
                    tutorialText2 = new byte[] { 0xFF, 0xFF },
                    tutorialTextbuffer = new byte[] { 0x00, 0x00 },
                    unknownCP = new byte[] { 0xC3, 0x50 },
                    unknownPostCP = new byte[] { 0x00, 0x00, 0x00 }
                };
            }

            public static Stage Load(BinaryReader reader)
            {
                Stage level = new Stage();
                level.tutorialText1 = reader.ReadBytes(2);
                level.tutorialText2 = reader.ReadBytes(2);
                level.tutorialTextbuffer = reader.ReadBytes(2);

                level.unknownCP = reader.ReadBytes(2);
                level.unknownPostCP = reader.ReadBytes(3);
                level._ID = reader.ReadByte();
                if (level._ID == 0)
                {
                    level._ID = 99;
                }
                else
                {
                    --level._ID;
                }

                level.backgroundID = IndexResolver.Load(reader, backgroundID_LENGTH);
                level.notDoor = IndexResolver.Load(reader, notDoor_LENGTH);
                level.teleportIndices = IndexResolver.Load(reader, teleportIndices_LENGTH);

                foreach (byte key in HOLE_DEFINITION_KEYS)
                {
                    level.holeDefinitions.Add(key, (NintendoLand.TileTypes.Hole.Size)reader.ReadByte());
                    reader.ReadBytes(holeDefinitions_LENGTH - 1);
                }

                for (int contentFlagIndex = 0; contentFlagIndex < MAXIMUM_CONTENT_FLAGS; contentFlagIndex++)
                {
                    level.contentFlags.Add((ContentFlag)reader.ReadByte());
                    bool isDone = reader.ReadByte() != 0x20;
                    if (isDone)
                    {
                        contentFlagIndex++;
                        for (int i = contentFlagIndex; i < MAXIMUM_CONTENT_FLAGS; i++)
                        {
                            reader.ReadBytes(contentFlag_LENGTH);
                        }
                        break;
                    }
                }

                for (int switchOrPepperIndex = 0; switchOrPepperIndex < SWITCHORPEPPER_DEFINITIONS; switchOrPepperIndex++)
                {
                    level.switchOrPepperDefinitions[switchOrPepperIndex] = (PepperOrSwitchFlag)reader.ReadByte(); // < SWITCHORPEPPER_DEFINITIONS * (1 byte (value) + 7 bytes (padding))
                    reader.ReadBytes(switchOrPepperDefinitions_LENGTH - 1);
                }

                for (int patternIndex = 0; patternIndex < MOVEMENTPATTERN_DEFINITIONS; patternIndex++)
                {
                    level.movementPatterns[patternIndex] = IndexResolver.Load(reader, movementPattern_LENGTH);
                }

                for (int FDefinitionIndex = 0; FDefinitionIndex < FRUIT_ORDER_DEFINITIONS; FDefinitionIndex++)  // < FRUIT_ORDER_DEFINITIONS * (1 or 2 bytes (value) + 10 or 11 bytes (padding))
                {                                                                                               // related to fruit order? actual usage unknown
                    level.orderedFruitDefinition[FDefinitionIndex] = IndexResolver.Load(reader, orderedFruitDefinition_LENGTH);
                }

                for (int fruitAssociationIndex = 0; fruitAssociationIndex < FRUIT_ASSOCIATIONS; fruitAssociationIndex++)  // < FRUIT_ASSOCIATIONS * (2 bytes (min) + 1 byte (delimiter) + 2 bytes (max)) + 7 bytes (padding))
                {                                                                                               // related to fruit order? actual usage unknown
                    level.fruitAssociations[fruitAssociationIndex] = IndexResolver.Load(reader, fruitAssociations_LENGTH);
                }

                return level;
            }

            public void SerializeExbin(ref List<byte> serializedData, int bytesLeft)
            {
                serializedData.AddRange(tutorialText1);
                serializedData.AddRange(tutorialText2);
                serializedData.AddRange(tutorialTextbuffer);

                serializedData.AddRange(unknownCP);
                serializedData.AddRange(unknownPostCP);
                serializedData.Add((byte)(_ID == 99 ? 0 : ++_ID));
                backgroundID.SerializeExbin(ref serializedData, backgroundID_LENGTH);
                notDoor.SerializeExbin(ref serializedData, notDoor_LENGTH);
                teleportIndices.SerializeExbin(ref serializedData, teleportIndices_LENGTH);

                foreach (byte key in HOLE_DEFINITION_KEYS)
                {
                    serializedData.Add((byte)holeDefinitions[key]);
                    for (int i = 0; i < holeDefinitions_LENGTH - 1; i++)
                    {
                        serializedData.Add(0x00);
                    }
                }

                for (int contentFlagIndex = 0; contentFlagIndex < MAXIMUM_CONTENT_FLAGS; contentFlagIndex++)
                {
                    if (contentFlagIndex < contentFlags.Count)
                    {
                        serializedData.Add((byte)contentFlags[contentFlagIndex]);
                        if (contentFlagIndex+1 < contentFlags.Count)
                        {
                            serializedData.Add(0x20);
                        }
                        else
                        {
                            serializedData.Add(0x00);
                        }
                    }
                    else
                    {
                        serializedData.Add(0x00);
                        serializedData.Add(0x00);
                    }
                }

                for (int switchOrPepperIndex = 0; switchOrPepperIndex < SWITCHORPEPPER_DEFINITIONS; switchOrPepperIndex++)
                {
                    serializedData.Add((byte)switchOrPepperDefinitions[switchOrPepperIndex]);
                    for (int i = 0; i < switchOrPepperDefinitions_LENGTH - 1; i++)
                    {
                        serializedData.Add(0x00);
                    }
                }

                for (int patternIndex = 0; patternIndex < MOVEMENTPATTERN_DEFINITIONS; patternIndex++)
                {
                    movementPatterns[patternIndex].SerializeExbin(ref serializedData, movementPattern_LENGTH);
                }

                for (int FDefinitionIndex = 0; FDefinitionIndex < FRUIT_ORDER_DEFINITIONS; FDefinitionIndex++)
                {
                    orderedFruitDefinition[FDefinitionIndex].SerializeExbin(ref serializedData, orderedFruitDefinition_LENGTH);
                }

                for (int fruitAssociationIndex = 0; fruitAssociationIndex < FRUIT_ASSOCIATIONS; fruitAssociationIndex++)
                {
                    fruitAssociations[fruitAssociationIndex].SerializeExbin(ref serializedData, fruitAssociations_LENGTH);
                }
            }

        };

        #region File description
        private byte[] headerUnknown; // < 20 bytes
        private Dictionary<int, Stage> payload = new Dictionary<int, Stage>(61); // < 956 bytes * 61 (61 is based on the game's default amount of maps (50 in-game + 10 unused maps + tutorial))
        public bool HasLevelWithID(int mapID)
        {
            return payload.ContainsKey(mapID);
        }
        public Stage GetLevelByID(int mapID)
        {
            return payload[mapID];
        }
        public int LevelCount => payload.Count;

        /// <summary>
        /// Update stage data - currently limited to overwriting stages (no inserting)
        /// </summary>
        public bool UpdateLevelByID(int mapID, Stage stage)
        {
            if (!payload.ContainsKey(mapID))
            {
                return false;
            }

            payload[mapID] = stage;
            return true;
        }
        #endregion

        public static StageData Load(string pathToYsiExtract)
        {
            StageData stageData = new StageData();

            FileStream fs = new FileStream(Path.Combine(pathToYsiExtract, "StageData.exbin"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (BinaryReader reader = new BinaryReader(fs))
            {
                stageData.headerUnknown = reader.ReadBytes(20);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    Stage level = Stage.Load(reader);
                    stageData.payload[level.ID] = level;
                }
                Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length, "");
            }

            return stageData;
        }

        public void SerializeExbin(ref List<byte> serializedData, int bytesLeft)
        {
            serializedData.AddRange(headerUnknown);
            foreach(KeyValuePair<int, Stage> stageWithIndex in payload)
            {
                stageWithIndex.Value.SerializeExbin(ref serializedData, NintendoLand.DataFormats.StageData.Stage.BYTES_REQUIRED);
            }

        }

    }
}
