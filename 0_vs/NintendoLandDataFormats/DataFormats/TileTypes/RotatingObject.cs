using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public class RotatingObjectHeader
    {
        public const int BYTES_PER_CONTAINER = BYTES_PER_SEGMENT*8;
        public const int BYTES_PER_SEGMENT = 12;

        public static RotatingObjectHeader[] Load(List<byte> parsableBytes)
        {
            List<List<List<byte>>> segments = new List<List<List<byte>>>();
            do
            {
                // ignore empty segments
                if (parsableBytes[0] == 0x30)
                {
                    do
                    {
                        parsableBytes.RemoveAt(0);
                    } while (parsableBytes.Count > 0 && parsableBytes[0] == 0x00);
                    continue;
                }

                List<List<byte>> segment = new List<List<byte>>();
                int parsedBytes = 0;
                do
                {
                    List<byte> item = new List<byte>();
                    do
                    {
                        item.Add(parsableBytes[0]);
                        parsableBytes.RemoveAt(0);
                        parsedBytes++;
                    } while (parsableBytes.Count > 0 && parsableBytes[0] != 0x20 && parsableBytes[0] != 0x00 && parsedBytes < 12);

                    if (parsableBytes[0] == 0x20)
                    {
                        parsableBytes.RemoveAt(0);
                        parsedBytes++;
                    }

                    segment.Add(item);

                    while (parsableBytes.Count > 0 && parsableBytes[0] == 0x00)
                    {
                        parsableBytes.RemoveAt(0);
                        parsedBytes++;
                    }
                } while (parsableBytes.Count > 0 && parsedBytes < 12);
                segments.Add(segment);
            } while (parsableBytes.Count > 0);

            List<RotatingObjectHeader> rotatingObjects = new List<RotatingObjectHeader>();
            foreach (List<List<byte>> segment in segments)
            {
                Debug.Assert(segment.Count >= 1, "Segment needs to have at least a pivot");
                BaseType pivot = BaseType.Construct(segment[0]);
                segment.RemoveAt(0);
                BaseType[] orbiter = new BaseType[segment.Count];

                int i = 0;
                foreach (List<byte> item in segment)
                {
                    orbiter[i] = BaseType.Construct(item);
                    i++;
                }
                rotatingObjects.Add(new RotatingObjectHeader(pivot, orbiter));
            }

            return rotatingObjects.ToArray();
        }

        public bool SerializeExbin(ref List<byte> target, ref int bytesLeft)
        {
            // add pivot
            pivotObject.SerializeExbin(ref target, ref bytesLeft);
            target.Add(0x20);
            --bytesLeft;

            // add all orbiter
            foreach (BaseType orbiter in rotatingObjects)
            {
                orbiter.SerializeExbin(ref target, ref bytesLeft);
                target.Add(0x20);
                --bytesLeft;
            }

            // remove spillover
            target.RemoveAt(target.Count - 1);
            ++bytesLeft;
            return true;
        }

        private BaseType pivotObject;
        private BaseType[] rotatingObjects;
        public RotatingObjectHeader(BaseType pivot, BaseType[] orbiter)
        {
            this.pivotObject = pivot;
            this.rotatingObjects = orbiter;
        }
    }

    /// <summary>
    /// Refers to header information containing a pivot and rotating objects (orbiters)
    /// Up to RotatingObject.AVAILABLE_PIVOTS pivots can be placed inside the map
    /// 
    /// The body only refers to the MemoryIdentifier and an order (i.e. ['M', '2']).
    /// Inside the header the actual objects are defined (pivot and orbiters)
    /// 
    /// Examples:
    /// In DataFormats.MapData10.exbin at [5, 6] we find ['M', '1'] inside the body. (The first orbiter.)
    /// Information about the actual objects, is stored inside the header starting with M1 at offset 0x26.
    /// This data is formatted just like the regular DataFormats.MapData, but with only 2 bytes for an order (instead of the regular 3)
    /// and allowing up to 4 items, including the pivot.
    /// 
    /// Going there, we would find [Oa, P1] (a small spike + a regular fruit).
    /// This tells us, the pivot object is a small spike, with a fruit as orbiter around it.
    /// 
    /// At [9, 12] we find ['M', '2']. At 0x32 (M1 at 0x26 + (4 items * 3 bytes)) we find [Oa, P2].
    /// Another small spike as pivot, with a regular fruit as orbiter around it.
    /// 
    /// Exception: More than 3 objects around one pivot
    /// It is possible to have more than 3 objects around a pivot. This is done, by having the
    /// last orbiter be a RotatingObject with +1 of the current order.
    /// Example in DataFormats.MapData38.exbin:
    /// At [7, 8] we find ['M', '1']. At 0x26 (the header containing pivot + orbitters) we find: [S, P1, P2, M2]
    /// Notice the M2 at the end - looking at 0x32 (the header containing the next pivot + orbitters) we find [P3, P4].
    /// Internally, the second list is appended to the first and M2 is dropped, resulting in [S, P1, P2, P3, P4]; the start point
    /// being the pivot with 4 regular fruits as orbiter around it.
    /// </summary>
    public class RotatingObject : BaseType
    {
        public override char MemoryIdentifier => 'M';

        public int index;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "order of Rotating object", "Rotating objects require an order - none found");
            index = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }

        /// <summary>
        /// Convert an instance of this class into a byte-array representation inside the .exbin-format
        /// </summary>
        /// <param name="bytesLeft">How much bytes we can occupy for this order at max</param>
        public override bool SerializeExbin(ref List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified order
            byte[] indexStringified = System.Text.Encoding.ASCII.GetBytes(index.ToString());
            Debug.Assert(bytesLeft >= indexStringified.Length, "No bytes left in buffer for order");
            target.AddRange(indexStringified);
            bytesLeft -= indexStringified.Length;

            return true;
        }

        // @TODO VM Make private - registar complains if not public, work around it
        public RotatingObject(){}
    }
}
