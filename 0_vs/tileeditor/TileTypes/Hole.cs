﻿using System;
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

        enum HoleSize
        {
            Small = 'a',
            Large = 'b'
        }

        #region Form generator
        private ComboBox selector;
        HoleSize state;
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, System.Windows.GridUnitType.Star);
            Label label = new Label();
            label.Content = "Hole size:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            selector = new ComboBox();
            foreach (HoleSize spikeState in Enum.GetValues(typeof(HoleSize)))
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
            state = (HoleSize)selector.SelectedItem;
        }
        #endregion
    }
}