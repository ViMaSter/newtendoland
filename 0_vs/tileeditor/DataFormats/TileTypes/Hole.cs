using System;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Hole : TileType
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

        protected override bool Load(BinaryReader reader, int availablePadding)
        {
            state = (Size)reader.ReadByte();

            // skip potentially remaining memory
            for (int i = 1; i < availablePadding; i++)
            {
                reader.ReadByte();
            }
            return true;
        }
        #endregion
    }
}
