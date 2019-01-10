using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class RotatingObject : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'M';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Rotating object";
            }
        }

        #region Form generator
        private TileTypes.TileType pivotObject;
        private TileTypes.TileType[] rotatingObjects;
        private ComboBox pivotInput;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label pivotLabel = new Label();
            pivotLabel.Content = "Pivot:";
            Grid.SetColumn(pivotLabel, 0);
            Grid.SetRow(pivotLabel, 0);

            pivotInput = new ComboBox();
            pivotInput.ItemsSource = TileTypes.TileType.Enumerable;
            pivotInput.SelectedValuePath = "MemoryIdentifier";
            pivotInput.DisplayMemberPath = "DisplayName";
            Grid.SetColumn(pivotInput, 1);
            Grid.SetRow(pivotInput, 0);
            
            // @TODO: + - Button for valid TileTypes
            grid.Children.Add(pivotLabel);
            grid.Children.Add(pivotInput);
            return true;
        }

        public override void ObtainData()
        {
            pivotObject = TileTypes.TileType.GetTypeByMemoryIdentifier((char)pivotInput.SelectedValue);
        }

        private bool ValidateInput(string text)
        {
            return true;
        }

        #endregion
    }
}
