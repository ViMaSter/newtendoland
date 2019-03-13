using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public class Spike : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'A';
            }
        }

        SpikeStartState state;

        enum SpikeStartState
        {
            Closed = '+',
            Open = '-'
        }

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count == 1, "Initial spike state", "Spikes require a parameter denoting initial state");

            state = (SpikeStartState)parsableBytes[0];
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] stateStringified = System.Text.Encoding.ASCII.GetBytes(((char)state).ToString());
            Debug.Assert(bytesLeft >= stateStringified.Length, "No bytes left in buffer for hole size qualifier");
            target.AddRange(stateStringified);
            bytesLeft -= stateStringified.Length;

            return true;
        }
    }
}
