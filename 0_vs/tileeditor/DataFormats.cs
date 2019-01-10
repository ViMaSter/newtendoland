using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace tileeditor
{
    enum TileType : byte
    {
        TogglableSpike = (byte)'A',
        Bee = (byte)'E',
        OrderedFruit = (byte)'F',
        Goal = (byte)'G',
        Heart = (byte)'H',
        RotatingFruitWithBonus = (byte)'M',
        Hole = (byte)'O',
        Fruit = (byte)'P',
        Start = (byte)'S',
        PepperAndSwitch = (byte)'W',
        Wall = (byte)'*',
        Empty = (byte)'0',
        Closed = (byte)'+',
        Opened = (byte)'-',
        NONE = 0x00,
    }

    struct Tile
    {
        public static readonly TileType[] PickableTypes = 
        {
            TileType.TogglableSpike,
            TileType.Bee,
            TileType.OrderedFruit,
            TileType.Goal,
            TileType.Heart,
            TileType.RotatingFruitWithBonus,
            TileType.Hole,
            TileType.Fruit,
            TileType.Start,
            TileType.PepperAndSwitch
        };

        private TileType type;
        private string index;

        public static Tile Load(BinaryReader reader, int indexLength = 3)
        {
            Tile tile = new Tile();
            tile.type = (TileType)reader.ReadByte();
            tile.index = System.Text.Encoding.Default.GetString(reader.ReadBytes(indexLength));
            return tile;
        }

        public char GetTileType()
        {
            return !IsValid() ? ' ' : (char)type;
        }
        public override string ToString()
        {
            return !IsValid() ? "" : ((char)type + GetIndex());
        }

        public bool IsValid()
        {
            return type != TileType.Empty && type != TileType.NONE;
        }

        public string GetIndex()
        {
            return index.Trim('\0');
        }
    }

    struct RotatingObject
    {
        public Tile pivot;
        public Tile[] rotatingObjects;

        public const int LIST_COUNT = 8;
        public static RotatingObject[] Load(BinaryReader reader)
        {
            List<RotatingObject> rotatingObjects = new List<RotatingObject>();
            for (int i = 0; i < LIST_COUNT; i++)
            {
                RotatingObject rotatingObject = new RotatingObject();
                rotatingObject.pivot = Tile.Load(reader, 2);
                if (!rotatingObject.pivot.IsValid())
                {
                    // skip 3 entries so we advance the reader
                    Tile.Load(reader, 2);
                    Tile.Load(reader, 2);
                    Tile.Load(reader, 2);
                    continue;
                }

                // By default a list ends after 3 items, except if the last item is of TileType.RotatingFruitWithBonus
                // then the next 4 items are appended to that same list, instead of treating it like another list
                List<Tile> tileList = new List<Tile>(3);
                for (int j = 0; j < 3; j++)
                {
                    Tile newTile = Tile.Load(reader, 2);
                    if (!newTile.IsValid())
                    {
                        continue;
                    }
                    tileList.Add(newTile);
                }

                while (tileList.Count > 0 && tileList[tileList.Count - 1].GetTileType() == (char)TileType.RotatingFruitWithBonus)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Tile newTile = Tile.Load(reader, 2);
                        if (!newTile.IsValid())
                        {
                            continue;
                        }
                        tileList.Add(newTile);
                    }
                    i++;
                }
                rotatingObject.rotatingObjects = tileList.ToArray();
                rotatingObjects.Add(rotatingObject);
            }
            return rotatingObjects.ToArray();
        }
    }

    struct MapData
    {
        public const int ROWS_VISIBLE = 15;
        public const int ROWS_TOTAL = 20;
        public const int COLUMNS_VISIBLE = 18;
        public const int COLUMNS_TOTAL = 24;
        #region Header
        private byte[] headerUnknown;
        private byte[] background;
        private byte[] headerUnknown2;
        private RotatingObject[] rotatingObjects;
        private byte[] headerUnknown3;
        #endregion

        #region Payload
        private Tile[,] visibleRows;
        private Tile[,] invisibleRows;
        public Tile GetItem(int row, int column)
        {
            return visibleRows[row, column];
        }
        #endregion

        public static MapData Load(string fileName)
        {
            MapData mapData = new MapData();
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                mapData.headerUnknown = reader.ReadBytes(16);
                mapData.background = reader.ReadBytes(4);
                mapData.headerUnknown2 = reader.ReadBytes(18);
                mapData.rotatingObjects = RotatingObject.Load(reader);
                mapData.visibleRows = new Tile[ROWS_VISIBLE, COLUMNS_TOTAL];
                for (int row = 0; row < ROWS_VISIBLE; row++)
                {
                    for (int column = 0; column < COLUMNS_TOTAL; column++)
                    {
                        mapData.visibleRows[row,column] = Tile.Load(reader);
                    }
                }
                mapData.invisibleRows = new Tile[ROWS_TOTAL-ROWS_VISIBLE, COLUMNS_TOTAL];
                for (int row = 0; row < ROWS_TOTAL - ROWS_VISIBLE; row++)
                {
                    for (int column = 0; column < COLUMNS_TOTAL; column++)
                    {
                        mapData.invisibleRows[row,column] = Tile.Load(reader);
                    }
                }
            }
            return mapData;
        }

        public override string ToString()
        {
            string result = "";
            result += ' ';
            result += ' ';
            result += ' ';

            for (int column = 0; column < COLUMNS_TOTAL; column++)
            {
                result += (column < COLUMNS_VISIBLE) ? 'V' : 'I';
            }
            result += " \r\n";
            result += " \r\n";
            result += " \r\n";

            for (int row = 0; row < ROWS_VISIBLE; row++)
            {
                result += 'V';
                result += ' ';
                result += ' ';
                for (int column = 0; column < COLUMNS_TOTAL; column++)
                {
                    result += this.visibleRows[row, column].GetTileType();
                }
                result += " \r\n";
            }
            for (int row = 0; row < ROWS_TOTAL - ROWS_VISIBLE; row++)
            {
                result += ('I');
                result += ' ';
                result += ' ';
                for (int column = 0; column < COLUMNS_TOTAL; column++)
                {
                    result += this.invisibleRows[row, column].GetTileType();
                }
                result += " \r\n";
            }

            return result;
        }
    }
}
