﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace tileeditor.DataFormats
{
    class StageData
    {
        private const int LEVEL_AMOUNT = 50;

        class Level
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

            enum SwitchOrPepper
            {
                NONE = 0,
                Switch = '1',
                Pepper = '2'
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

            /// <summary>
            /// Helper-class for parsing indices used to refer to rows in other "*Data"-files.
            ///  (i.e. Fruits used in a Level (MapData02.exbin) to which type they are and fuel they provide (FruitData.exbin)
            /// 
            /// This class is required, as indices are NOT stored as raw byte values, but human readable-representations (strings) of those numbers in a fixed length, null-terminated string.
            /// I.e. "1" = 0x49, "4" = 0x52, "17" = ['1', '7'] = [0x49, 0x55], 20 = ['2', '0'] = [0x50, 0x48]
            /// Additionally, both a direct index value and a range of indices are supported:
            /// - Direct values simply being a string of the number i.e. "1", "47", "20"
            /// - Ranges being formatted using a direct value like above, delimited by "~" or 0x00 followed by another direct value i.e. "2~4", "1~17", "20.30"
            /// Notice how there is NO padding used for the min and max value
            /// 
            /// This class helps with parsing these type of references. Use IndexResolver.Load() and pass two parameters:
            ///  - a reader pointed to the beginning of the index to parse
            ///  - the maximum amount of bytes that can be used (this is always fixed per field)
            /// 
            /// The returned object can be asked
            /// - if it's a range or direct value (.IsRange)
            /// - (direct values only) for it's direct value (.Value)
            /// - (ranges only)        for the min value (.Min)
            /// - (ranges only)        for the INCLUSIVE max value (.Max)
            /// - (ranges only)        a random number between that Range (GetRandom())
            /// </summary>
            class IndexResolver
            {
                public bool IsRange
                {
                    get
                    {
                        return max != -1;
                    }
                }

                int minOrValue;
                public int Min
                {
                    get
                    {
                        Debug.Assert(!IsRange, "Value is not a range - use .Value instead");
                        return minOrValue;
                    }
                }
                public int Value
                {
                    get
                    {
                        Debug.Assert(!IsRange, "Value is a range - use .Min instead");
                        return minOrValue;
                    }
                }


                int max = -1;
                public int Max
                {
                    get
                    {
                        Debug.Assert(!IsRange, "Value is not a range - this value will be invalid");
                        return max;
                    }
                }

                private IndexResolver(int min, int max)
                {
                    this.minOrValue = min;
                    this.max = max;
                }
                private IndexResolver(int value)
                {
                    this.minOrValue = value;
                }

                public int GetRandom()
                {
                    return IsRange ? new System.Random().Next(minOrValue, max + 1) : minOrValue;
                }

                private static int ParseNumber(BinaryReader reader, ref int remainingBytes)
                {
                    // peek for a number
                    List<byte> buffer = new List<byte>{ reader.ReadByte() };
                    Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");

                    if (buffer[buffer.Count - 1] == 0x00 || buffer[buffer.Count - 1] == '~')
                    {
                        // if that's already invalid we can't handle it
                        return -1;
                    }

                    // otherwise peek for another number
                    buffer.Add(reader.ReadByte());
                    Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");

                    while (buffer[buffer.Count - 1] != 0x00 || buffer[buffer.Count - 1] != '~')
                    {
                        buffer.Add(reader.ReadByte());

                        // peek for the next number
                        Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");
                    };

                    buffer.RemoveAt(buffer.Count - 1);

                    // otherwise concatenate both bytes and return the value
                    return System.Byte.Parse(System.Text.Encoding.Default.GetString(buffer.ToArray()));
                }

                public static IndexResolver Load(BinaryReader reader, ref int remainingBytes)
                {
                    int parsedMin = ParseNumber(reader, ref remainingBytes);
                    int parsedMax = ParseNumber(reader, ref remainingBytes);

                    // discard padding
                    if (remainingBytes > 0)
                    {
                        reader.ReadBytes(remainingBytes);
                    }

                    if (parsedMax == -1)
                    {
                        return new IndexResolver(parsedMin);
                    }
                    else
                    {
                        return new IndexResolver(parsedMin, parsedMax);
                    }
                }
            }

            private static readonly byte[] HOLE_DEFINITION_KEYS = { 0x0a, 0x0b, 0x0c};
            private const int MAXIMUM_CONTENT_FLAGS  = 6;
            private const int SWITCHORPEPPER_DEFINITIONS = 16;
            private const int MOVEMENTPATTERN_DEFINITIONS = 16;
            private const int FRUIT_ORDER_DEFINITIONS = 16;
            private const int FRUIT_ASSOCIATIONS = 32;

            // this seems to determine which textboxes are shown after map start
            // before a player gains control over the level
            private byte[] tutorialText1;                       // < 2 bytes
            private byte[] tutorialText2;                       // < 2 bytes
            // potential buffer for tutorial texts
            private byte[] tutorialTextbuffer;                  // < 2 bytes
            // this is always set to [0xC3, 0x50] in default levels
            // @TODO VM play with this value a bit
            private byte[] unknownCP;                           // < 2 bytes
            // this is always set to [0x00, 0x00] in default levels
            // @TODO VM play with this value a bit
            private byte[] unknownPostCP;                       // < 3 bytes
            private byte ID;                                    // < 1 byte

            // loaded from Ysi_Cmn.pack / Common/Model/Ysi_Field.szs / gsys.bfres / Textures -> Field*
            // @TODO VM build renderer/selector for this
            // @TODO VM check if a range could be used here (only direct values, but structure supports more)
            private const int BackgroundID_LENGTH = 8;
            private IndexResolver BackgroundID;                                                                                        
            // unknown - definitively unrelated to the door                                                                         
            private const int NotDoor_LENGTH = 8;
            private IndexResolver NotDoor;                                                                                          
            // BUFFER < 6 bytes
            // single or range of potential teleports?                                                                              
            private const int TeleportIndices_LENGTH = 8;
            private IndexResolver TeleportIndices;                                                                                  
            private const int HoleDefinitions_LENGTH = 4;
            private Dictionary<byte, tileeditor.TileTypes.Hole.Size> HoleDefinitions = new Dictionary<byte, TileTypes.Hole.Size>(3);
            private const int ContentFlag_LENGTH = 2;
            private ContentFlag[] ContentFlags = new ContentFlag[MAXIMUM_CONTENT_FLAGS];                                            
            private const int SwitchOrPepperDefinitions_LENGTH = 8;
            private SwitchOrPepper[] SwitchOrPepperDefinitions = new SwitchOrPepper[SWITCHORPEPPER_DEFINITIONS];                    
            private const int MovementPattern_LENGTH = 12;
            private IndexResolver[] MovementPatterns = new IndexResolver[MOVEMENTPATTERN_DEFINITIONS];                              
            // related to fruit order?                                                                                              
            private const int FDefinition_LENGTH = 12;
            private IndexResolver[] FDefinition = new IndexResolver[FRUIT_ORDER_DEFINITIONS];                                       
            // resolved an index to a specific fruit (See FruitData)                                                                
            private const int FruitAssociations_LENGTH = 12;
            private IndexResolver[] FruitAssociations = new IndexResolver[FRUIT_ASSOCIATIONS];                                      

            public static Level Load(BinaryReader reader)
            {
                Level level = new Level();
                level.tutorialText1 = reader.ReadBytes(2);
                level.tutorialText2 = reader.ReadBytes(2);
                level.tutorialTextbuffer = reader.ReadBytes(2);

                level.unknownCP = reader.ReadBytes(2);
                level.unknownPostCP = reader.ReadBytes(3);
                level.ID = reader.ReadByte();

                {
                    int remainingBytes = BackgroundID_LENGTH;
                    level.BackgroundID = IndexResolver.Load(reader, ref remainingBytes);
                    reader.ReadBytes(remainingBytes);
                }

                {
                    int remainingBytes = NotDoor_LENGTH;
                    level.NotDoor = IndexResolver.Load(reader, ref remainingBytes);
                    reader.ReadBytes(remainingBytes);
                }

                {
                    int remainingBytes = TeleportIndices_LENGTH;
                    level.TeleportIndices = IndexResolver.Load(reader, ref remainingBytes);
                    reader.ReadBytes(remainingBytes);
                }

                foreach (byte key in HOLE_DEFINITION_KEYS)
                {
                    level.HoleDefinitions.Add(key, (tileeditor.TileTypes.Hole.Size)reader.ReadByte());
                    reader.ReadBytes(HoleDefinitions_LENGTH - 1);
                }

                for (int contentFlagIndex = 0; contentFlagIndex < MAXIMUM_CONTENT_FLAGS; contentFlagIndex++)
                {
                    level.ContentFlags[contentFlagIndex] = (ContentFlag)reader.ReadByte();                    
                    reader.ReadBytes(ContentFlag_LENGTH - 1);
                }

                for (int switchOrPepperIndex = 0; switchOrPepperIndex < SWITCHORPEPPER_DEFINITIONS; switchOrPepperIndex++)
                {
                    level.SwitchOrPepperDefinitions[switchOrPepperIndex] = (SwitchOrPepper)reader.ReadByte(); // < SWITCHORPEPPER_DEFINITIONS * (1 byte (value) + 7 bytes (padding))
                    reader.ReadBytes(SwitchOrPepperDefinitions_LENGTH - 1);
                }

                for (int patternIndex = 0; patternIndex < MOVEMENTPATTERN_DEFINITIONS; patternIndex++)
                {
                    int bytes = MovementPattern_LENGTH;
                    level.MovementPatterns[patternIndex] = IndexResolver.Load(reader, ref bytes);
                    if (bytes > 0)
                    {
                        reader.ReadBytes(bytes);
                    }
                }

                for (int FDefinitionIndex = 0; FDefinitionIndex < FRUIT_ORDER_DEFINITIONS; FDefinitionIndex++)  // < FRUIT_ORDER_DEFINITIONS * (1 or 2 bytes (value) + 10 or 11 bytes (padding))
                {                                                                                               // related to fruit order? actual usage unknown
                    int bytes = FDefinition_LENGTH;
                    level.FDefinition[FDefinitionIndex] = IndexResolver.Load(reader, ref bytes);
                    if (bytes > 0)
                    {
                        reader.ReadBytes(bytes);
                    }
                }

                for (int fruitAssociationIndex = 0; fruitAssociationIndex < FRUIT_ASSOCIATIONS; fruitAssociationIndex++)  // < FRUIT_ASSOCIATIONS * (2 bytes (min) + 1 byte (delimiter) + 2 bytes (max)) + 7 bytes (padding))
                {                                                                                               // related to fruit order? actual usage unknown
                    int bytes = FruitAssociations_LENGTH;
                    level.FruitAssociations[fruitAssociationIndex] = IndexResolver.Load(reader, ref bytes);
                    if (bytes > 0)
                    {
                        reader.ReadBytes(bytes);
                    }
                }

                return level;
            }
        };

        #region File description
        private byte[] headerUnknown; // < 20 bytes
        private Level[] payload;
        #endregion

        public static StageData Load(string fileName)
        {
            StageData stageData = new StageData();

            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                stageData.headerUnknown = reader.ReadBytes(20);
                for (int levelIndex = 0; levelIndex < LEVEL_AMOUNT; levelIndex++)
                {
                    stageData.payload[levelIndex] = Level.Load(reader);
                }
            }

            return stageData;
        }
    }
}