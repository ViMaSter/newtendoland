using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Spike : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'A';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Spike";
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

        enum SpikeStartState
        {
            Closed = '+',
            Open = '-'
        }

        #region Form generator
        private ComboBox selector;
        SpikeStartState state;
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, System.Windows.GridUnitType.Star);
            Label label = new Label();
            label.Content = "Spike style:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            selector = new ComboBox();
            foreach (SpikeStartState spikeState in Enum.GetValues(typeof(SpikeStartState)))
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
            state = (SpikeStartState)selector.SelectedItem;
        }
        #endregion

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
