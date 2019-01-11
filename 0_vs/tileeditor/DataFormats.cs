using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace tileeditor
{
    struct MapData
    {
        public const int ROWS_VISIBLE = 15;
        public const int ROWS_TOTAL = 20;
        public const int COLUMNS_VISIBLE = 18;
        public const int COLUMNS_TOTAL = 24;
        public const int HEADER_TILE_PADDING = 2;
        public const int BODY_TILE_PADDING = 3;
        #region Header
        private byte[] headerUnknown;
        private byte[] background;
        private byte[] headerUnknown2;
        private TileTypes.RotatingObject[] rotatingObjects;
        #endregion

        #region Payload
        private TileTypes.TileType[,] visibleRows;
        private TileTypes.TileType[,] invisibleRows;
        public TileTypes.TileType GetItem(int row, int column)
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
                mapData.rotatingObjects = TileTypes.RotatingObject.Load(reader);
                //reader.ReadBytes(96);
                mapData.visibleRows = new TileTypes.TileType[ROWS_VISIBLE, COLUMNS_TOTAL];
                for (int row = 0; row < ROWS_VISIBLE; row++)
                {
                    for (int column = 0; column < COLUMNS_TOTAL; column++)
                    {
                        mapData.visibleRows[row,column] = TileTypes.TileType.Construct(reader, BODY_TILE_PADDING);
                    }
                }
                mapData.invisibleRows = new TileTypes.TileType[ROWS_TOTAL-ROWS_VISIBLE, COLUMNS_TOTAL];
                for (int row = 0; row < ROWS_TOTAL - ROWS_VISIBLE; row++)
                {
                    for (int column = 0; column < COLUMNS_TOTAL; column++)
                    {
                        mapData.invisibleRows[row,column] = TileTypes.TileType.Construct(reader, MapData.BODY_TILE_PADDING);
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
                    result += this.visibleRows[row, column].MemoryIdentifier;
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
                    result += this.invisibleRows[row, column].MemoryIdentifier;
                }
                result += " \r\n";
            }

            return result;
        }
    }
}
