using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    /// <summary>
    /// Bee enemies are resolved inside StageData
    /// 
    /// Similar to how PepperOrSwitch, Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the Bee-type can be resolved by using the index (i.e. E2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see NintendoLand.DataFormats.StageData.Stage.movementPatterns
    /// </summary>
    public class Bee : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'E';
            }
        }

        int index;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "index of Bee", "Bees require an index - none found");
            index = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] orderStringified = System.Text.Encoding.ASCII.GetBytes(index.ToString());
            Debug.Assert(bytesLeft >= orderStringified.Length, "No bytes left in buffer for index");
            target.AddRange(orderStringified);
            bytesLeft -= orderStringified.Length;

            return true;
        }
    }
}
