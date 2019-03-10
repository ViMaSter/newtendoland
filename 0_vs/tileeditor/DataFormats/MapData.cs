using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace tileeditor.DataFormats
{
    public class MapData
    {
        #region Constants
        public const int ROWS_VISIBLE = 15;
        public const int ROWS_TOTAL = 20;
        public const int COLUMNS_VISIBLE = 18;
        public const int COLUMNS_TOTAL = 24;
        public const int HEADER_TILE_PADDING = 2;
        public const int CELL_TILE_PADDING = 3;
        #endregion

        #region File description
        #region Header
        private byte[] headerUnknown;                         // < 16 bytes
        private byte[] backgroundUnknown;                     // < 4 bytes
        private byte[] headerUnknown2;                        // < 18 bytes
        private TileTypes.RotatingObjectHeader[] rotatingObjects;   // < 96 bytes
        #endregion

        #region Payload
        private TileTypes.BaseType[,] rows;            // < ROWS_TOTAL * COLUMNS_TOTAL * (1 + CELL_TILE_PADDING) bytes
        #endregion
        #endregion

        public TileTypes.BaseType GetItem(int row, int column)
        {
            return rows[row, column];
        }

        public void SetItem(int row, int column, TileTypes.BaseType tile)
        {
            Debug.Assert(row < rows.GetLength(0), "Row index too high", "Row {0} is higher than {1}, the maximum row index", row, rows.GetLength(0));
            Debug.Assert(row >= 0, "Row index too low", "Row {0} is lower than 0, the minimum row index ", row);
            Debug.Assert(column < rows.GetLength(1), "Column index too high", "Column {0} is higher than {1}, the maximum column index", column, rows.GetLength(1));
            Debug.Assert(column >= 0, "Column index too low", "Column {0} is lower than 0, the minimum column index ", column);

            Debug.Assert(row <= ROWS_VISIBLE, "Row " + row + " is out of visible bounds!");
            Debug.Assert(column <= COLUMNS_VISIBLE, "Column " + column + " is out of visible bounds!");

            rows[row, column] = tile;
        }

        public static MapData Load(string pathToMapData)
        {
            MapData mapData = new MapData();
            FileStream fs = new FileStream(pathToMapData, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (BinaryReader reader = new BinaryReader(fs))
            {
                mapData.headerUnknown = reader.ReadBytes(16);
                mapData.backgroundUnknown = reader.ReadBytes(4);
                mapData.headerUnknown2 = reader.ReadBytes(18);
                mapData.rotatingObjects = TileTypes.RotatingObjectHeader.Load(new List<byte>(reader.ReadBytes(TileTypes.RotatingObjectHeader.BYTES_PER_CONTAINER)));
                mapData.rows = new TileTypes.BaseType[ROWS_TOTAL, COLUMNS_TOTAL];
                for (int row = 0; row < ROWS_TOTAL; row++)
                {
                    for (int column = 0; column < COLUMNS_TOTAL; column++)
                    {
                        List<byte> parsableBytes = new List<byte>();
                        while (reader.PeekChar() != 0x00)
                        {
                            parsableBytes.Add(reader.ReadByte());
                        }
                        mapData.rows[row, column] = TileTypes.BaseType.Construct(parsableBytes);
                        while (reader.PeekChar() == 0x00)
                        {
                            reader.ReadByte();
                        }
                    }
                }
            }
            return mapData;
        }

        /// <summary>
        /// Convert an instance of this class into a byte-array representation inside the .exbin-format
        /// </summary>
        /// <param name="bytesLeft">How much bytes we can occupy for this index at max</param>
        public void SerializeExbin(ref List<byte> target, int bytesLeft)
        {
            Debug.Assert(bytesLeft >= 16, "No bytes left for 'headerUnknown'");
            target.AddRange(this.headerUnknown);
            bytesLeft -= 16;

            Debug.Assert(bytesLeft >= 4, "No bytes left for 'backgroundUnknown'");
            target.AddRange(this.backgroundUnknown);
            bytesLeft -= 4;

            Debug.Assert(bytesLeft >= 18, "No bytes left for 'headerUnknown2'");
            target.AddRange(this.headerUnknown2);
            bytesLeft -= 18;

            {
                int rotatingObjectHeaderBytesLeft = TileTypes.RotatingObjectHeader.BYTES_PER_CONTAINER;
                foreach (TileTypes.RotatingObjectHeader entry in this.rotatingObjects)
                {
                    int segmentBytesLeft = TileTypes.RotatingObjectHeader.BYTES_PER_SEGMENT;
                    entry.SerializeExbin(ref target, ref segmentBytesLeft);
                    for (int i = 0; i < segmentBytesLeft; i++)
                    {
                        target.Add(0x00);
                    }
                    rotatingObjectHeaderBytesLeft -= TileTypes.RotatingObjectHeader.BYTES_PER_SEGMENT;
                }
                for (int i = 0; i < rotatingObjectHeaderBytesLeft; i++)
                {
                    target.Add((byte)(((rotatingObjectHeaderBytesLeft - i) % 12) == 0 ? 0x30 : 0x00));
                }
            }
            bytesLeft -= TileTypes.RotatingObjectHeader.BYTES_PER_CONTAINER;

            for (int row = 0; row < ROWS_TOTAL; row++)
            {
                for (int column = 0; column < COLUMNS_TOTAL; column++)
                {
                    int cellBytesLeft = CELL_TILE_PADDING + 1;
                    this.rows[row, column].SerializeExbin(ref target, ref cellBytesLeft);
                    Debug.Assert(cellBytesLeft >= 0, "Cell data required "+ cellBytesLeft * -1+"more bytes than legal");
                    for (int i = 0; i < cellBytesLeft; i++)
                    {
                        target.Add(0x00);
                    }
                }
            }
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
                    result += this.rows[row, column].MemoryIdentifier;
                }
                result += " \r\n";
            }
            for (int row = ROWS_VISIBLE; row < ROWS_TOTAL; row++)
            {
                result += ('I');
                result += ' ';
                result += ' ';
                for (int column = 0; column < COLUMNS_TOTAL; column++)
                {
                    result += this.rows[row, column].MemoryIdentifier;
                }
                result += " \r\n";
            }

            return result;
        }
    }
}
