using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace tileeditor.DataFormats
{
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
    [DebuggerDisplay("{IsRange?Min+\" - \"+Max:Value.ToString()}")]
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
                Debug.Assert(IsRange, "Value is not a range - use .Value instead");
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
                Debug.Assert(IsRange, "Value is not a range - this value will be invalid");
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
            if (remainingBytes <= 0)
            {
                // we've got no bytes left - simply return an invalid value
                return -1;
            }

            // peek for a number
            List<byte> buffer = new List<byte> { reader.ReadByte() };
            Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");

            if (buffer[buffer.Count - 1] < '0' || buffer[buffer.Count - 1] > '9')
            {
                // if that's already invalid we can't handle it
                return -1;
            }

            if (remainingBytes <= 0)
            {
                // if there are no more bytes left, simply return the byte we've read as integer
                return System.Byte.Parse(System.Text.Encoding.Default.GetString(buffer.ToArray()));
            }

            // otherwise peek for another number
            buffer.Add(reader.ReadByte());
            Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");

            while (buffer[buffer.Count - 1] >= '0' && buffer[buffer.Count - 1] <= '9')
            {
                buffer.Add(reader.ReadByte());

                // peek for the next number
                Debug.Assert(--remainingBytes >= 0, "No more bytes remaining to continue operation");
            };

            buffer.RemoveAt(buffer.Count - 1);

            // otherwise concatenate both bytes and return the value
            return System.Byte.Parse(System.Text.Encoding.Default.GetString(buffer.ToArray()));
        }

        /// <summary>
        /// Creates an instance of an index based on a memory region.
        /// 
        /// The memory region will start at the current position of reader and will stop parsing at latest after reading as many bytes as provided in remainingBytes.
        /// 
        /// Important: If numbers do not take up all the space provided by remainingBytes, they are discarded and the reader is advanced still.
        /// Basically this method guarantees, that after executing it, reader is at reader.BaseStream.Position (before execution) + remainingBytes.
        /// </summary>
        /// <param name="reader">BinaryReader that is set up to be at the intended start of an index</param>
        /// <param name="remainingBytes">How many bytes can be parsed at maximum - if less are required to create the index, the remaining bytes are skipped and reader is advanced</param>
        /// <returns></returns>
        public static IndexResolver Load(BinaryReader reader, int remainingBytes)
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
}
