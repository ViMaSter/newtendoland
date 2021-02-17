using System;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Spike : BaseObject
    {
        public override string DisplayName => "Spike";

        public override string IconFileName => DisplayName;

        public override string DisplayData => state.ToString();

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

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Spike;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            NintendoLand.TileTypes.Spike spike = tileType as NintendoLand.TileTypes.Spike;
            return new Spike() {state = (SpikeStartState) spike.state, selector = new ComboBox()};
        }
        #endregion
    }
}
