using System;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Fruit : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Fruit (Regular)";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName;
            }
        }

        public override string DisplayData
        {
            get
            {
                return "Index: " + index.ToString();
            }
        }

        public static int HighestCurrentlyUsedIndex = 0;
 
        #region Form generator
        private TextBox indexInput;
        int index;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "Index:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            indexInput = new TextBox();
            indexInput.Text = Fruit.HighestCurrentlyUsedIndex.ToString();
            indexInput.PreviewTextInput += Selector_PreviewTextInput;
            Grid.SetColumn(indexInput, 1);
            Grid.SetRow(indexInput, 0);

            grid.Children.Add(label);
            grid.Children.Add(indexInput);
            return true;
        }

        public override void ObtainData()
        {
            index = Int32.Parse(indexInput.Text);
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
            return tileType is NintendoLand.TileTypes.Fruit;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
