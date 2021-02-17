using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public class OrderedFruit : BaseType
    {
        public override char MemoryIdentifier => 'F';

        public int order;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "index of Ordered fruit", "Ordered fruits require an index - none found");
            order = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] orderStringified = System.Text.Encoding.ASCII.GetBytes(order.ToString());
            Debug.Assert(bytesLeft >= orderStringified.Length, "No bytes left in buffer for order");
            target.AddRange(orderStringified);
            bytesLeft -= orderStringified.Length;

            return true;
        }
    }
}
