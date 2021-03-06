﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    /// <summary>
    /// PepperOrSwitch are resolved inside StageData
    /// 
    /// Similar to how Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the PepperOrSwitch-type can be resolved by using the order (i.e. W2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see NintendoLand.DataFormats.StageData.PepperOrSwitchFlag
    /// </summary>
    public class PepperOrSwitch : BaseType
    {
        public override char MemoryIdentifier => 'W';

        public int Index => index;
        int index;

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "order of PepperOrSwitch", "PepperOrSwitch-entries require an order - none found");
            index = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified order
            byte[] orderStringified = System.Text.Encoding.ASCII.GetBytes(index.ToString());
            Debug.Assert(bytesLeft >= orderStringified.Length, "No bytes left in buffer for order");
            target.AddRange(orderStringified);
            bytesLeft -= orderStringified.Length;

            return true;
        }
    }
}
