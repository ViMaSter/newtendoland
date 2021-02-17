using System;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Empty : BaseObject
    {
        public override string DisplayName => "Empty";

        public override string IconFileName => "empty";

        public override string DisplayData => "";

        #region Form generator

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "EMPTY";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            grid.Children.Add(label);
            return true;
        }

        public override void ObtainData()
        {
        }

        private void Selector_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !ValidateInput(e.Text);
        }

        private bool ValidateInput(string text)
        {
            int input = -1;
            return Int32.TryParse(text, out input) && input >= 0 && input <= 99;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Empty;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return new Empty();
        }
        #endregion
    }

    class Nothing : BaseObject
    {
        public override string DisplayName => "Nothing";

        public override string DisplayData => "";

        #region Form generator

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "Nothing";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            grid.Children.Add(label);
            return true;
        }

        public override void ObtainData()
        {
        }

        private void Selector_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !ValidateInput(e.Text);
        }

        private bool ValidateInput(string text)
        {
            int input = -1;
            return Int32.TryParse(text, out input) && input >= 0 && input <= 99;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Nothing;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return new Nothing();
        }
        #endregion
    }

    class Null : BaseObject
    {
        public override string DisplayName => "Null";

        public override string DisplayData => "";

        #region Form generator

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "Null";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            grid.Children.Add(label);
            return true;
        }

        public override void ObtainData()
        {
        }

        private void Selector_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !ValidateInput(e.Text);
        }

        private bool ValidateInput(string text)
        {
            int input = -1;
            return Int32.TryParse(text, out input) && input >= 0 && input <= 99;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Null;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return new Null();
        }
        #endregion
    }

    class Wall : BaseObject
    {
        public override string DisplayName => "Wall";

        public override string DisplayData => "";

        #region Form generator

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "Wall";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            grid.Children.Add(label);
            return true;
        }

        public override void ObtainData()
        {
        }

        private void Selector_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !ValidateInput(e.Text);
        }

        private bool ValidateInput(string text)
        {
            int input = -1;
            return Int32.TryParse(text, out input) && input >= 0 && input <= 99;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Wall;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return new Wall();
        }
        #endregion
    }
}
