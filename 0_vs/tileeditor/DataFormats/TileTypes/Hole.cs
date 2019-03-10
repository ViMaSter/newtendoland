using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Hole : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'O';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Hole";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return MemoryIdentifier.ToString();
            }
        }

        public override string DisplayData
        {
            get
            {
                return state.ToString();
            }
        }

        public enum Size
        {
            NONE = 'N',
            Small = 'S',
            Medium = 'M',
            Large = 'L'
        }

        #region Form generator
        private ComboBox selector;
        Size state;
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, System.Windows.GridUnitType.Star);
            Label label = new Label();
            label.Content = "Hole size:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            selector = new ComboBox();
            foreach (Size spikeState in Enum.GetValues(typeof(Size)))
            {
                selector.Items.Add(spikeState);
            }
            Grid.SetColumn(selector, 1);
            Grid.SetRow(selector, 0);

            grid.Children.Add(label);
            grid.Children.Add(selector);
            return true;
        }

        public override void ObtainData()
        {
            state = (Size)selector.SelectedItem;
        }
        #endregion

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count == 1, "Hole size", "Hole type requires a size parameter");

            state = (Size)parsableBytes[0];
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
