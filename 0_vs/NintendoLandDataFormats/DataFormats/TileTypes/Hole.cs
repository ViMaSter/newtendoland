using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public class Hole : BaseType
    {
        public override char MemoryIdentifier => 'O';

        public enum Size
        {
            NONE = 'N',
            Small = 'S',
            Medium = 'M',
            Large = 'L'
        }

        public byte definitionIndex;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count == 1, "Hole size", "Hole type requires a size index");

            definitionIndex = parsableBytes[0];
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] stateStringified = System.Text.Encoding.ASCII.GetBytes(((char)definitionIndex).ToString());
            Debug.Assert(bytesLeft >= stateStringified.Length, "No bytes left in buffer for hole size qualifier");
            target.AddRange(stateStringified);
            bytesLeft -= stateStringified.Length;

            return true;
        }
    }
}
