using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    class Goal : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'G';
            }
        }

        int levelTarget;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            if (parsableBytes.Count == 0)
            {
                levelTarget = -1;
            }
            else
            {
                levelTarget = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
            }
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            if (levelTarget != -1)
            {
                byte[] indexStringified = System.Text.Encoding.ASCII.GetBytes(levelTarget.ToString());
                Debug.Assert(bytesLeft >= indexStringified.Length, "No bytes left in buffer for goal index");
                target.AddRange(indexStringified);
                bytesLeft -= indexStringified.Length;
            }

            return true;
        }
    }
}
