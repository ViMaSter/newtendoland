using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Spike : TileType
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
            selector = new ComboBox();
            selector.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            foreach (SpikeStartState spikeState in Enum.GetValues(typeof(SpikeStartState)))
            {
                selector.Items.Add(spikeState);
            }

            grid.Children.Add(selector);
            return true;
        }

        public override void ObtainData()
        {
            state = (SpikeStartState)selector.SelectedItem;
        }
        #endregion
    }
}
