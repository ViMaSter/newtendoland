using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    /// <summary>
    /// PepperOrSwitch are resolved inside StageData
    /// 
    /// Similar to how Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the PepperOrSwitch-type can be resolved by using the index (i.e. W2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see tileeditor.DataFormats.StageData.PepperOrSwitchFlag
    /// </summary>
    class PepperOrSwitch : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'W';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "PepperOrSwitch";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return MemoryIdentifier.ToString();
            }
        }

        int index;

        #region Form generator
        public override bool PopulateFields(ref Grid grid)
        {
            return false;
        }

        public override void ObtainData()
        {
        }
        #endregion

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "index of PepperOrSwitch", "PepperOrSwitch-entries require an index - none found");
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
