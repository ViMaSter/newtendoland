using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public class Fruit : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'P';
            }
        }

        public int index;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "index of fruit", "fruits require an index - none found");
            index = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] indexStringified = System.Text.Encoding.ASCII.GetBytes(index.ToString());
            Debug.Assert(bytesLeft >= indexStringified.Length, "No bytes left in buffer for index");
            target.AddRange(indexStringified);
            bytesLeft -= indexStringified.Length;

            return true;
        }
    }
}
