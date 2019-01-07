using System;
using System.IO;
using System.Diagnostics;

namespace tileeditor
{
    enum TileType : byte
    {
        Goal = (byte)'G',
        Wall = (byte)'*',
        Empty = (byte)'0',
        Start = (byte)'S',
        Fruit = (byte)'P',
        Unknown = (byte)'F'
    }

    struct Tile
    {
        private TileType type;
        private string index;

        public static Tile Load(BinaryReader reader)
        {
            Tile tile = new Tile();
            tile.type = (TileType)reader.ReadByte();
            tile.index = System.Text.Encoding.Default.GetString(reader.ReadBytes(3));
            return tile;
        }

        public char Visualize()
        {
            return type == TileType.Empty ? ' ' : (char)type;
        }
    }

    struct MapData
    {
        const int ROWS_VISIBLE = 15;
        const int ROWS_TOTAL = 20;
        const int COLUMNS_VISIBLE = 18;
        const int COLUMNS_TOTAL = 24;
        #region Header
        private byte[] headerUnknown;
        private byte[] background;
        private byte[] headerUnknown2;
        #endregion

        #region Payload
        private Tile[,] visibleRows;
        private Tile[,] invisibleRows;
        #endregion

        public static MapData Load(string fileName)
        {
            MapData mapData = new MapData();
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                mapData.headerUnknown = reader.ReadBytes(16);
                mapData.background = reader.ReadBytes(4);
                mapData.headerUnknown2 = reader.ReadBytes(114);
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

        public string Visualize()
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
                    result += this.visibleRows[row, column].Visualize();
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
                    result += this.invisibleRows[row, column].Visualize();
                }
                result += " \r\n";
            }

            return result;
        }
    }
}
