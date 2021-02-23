using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    class Fruit : BaseObject
    {
        public override string DisplayName => "Unordered Fruit";

        public override string IconFileName => DisplayName;

        public override string DisplayData =>
            string.Join(", ",
                selectedFruitTypes.Where(entry => entry.Value)
                    .Select(entry => entry.Key.ToString()));

        public Dictionary<FruitData.FruitType, bool> selectedFruitTypes;
        private CheckBox checkbox;

        #region Form generator
        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            int index = -1;
            foreach (FruitData.FruitType fruitType in Enum.GetValues(typeof(FruitData.FruitType)))
            {
                Label label = new Label();
                label.Content = fruitType + ":";
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, ++index);

                Image icon = new Image
                {
                    Source = new BitmapImage(new Uri(
                        $"pack://application:,,,/tileeditor;component/Resources/Fruit/{fruitType}.png",
                        UriKind.Absolute))
                };
                Grid.SetColumn(icon, 1);
                Grid.SetRow(icon, index);

                checkbox = new CheckBox();
                checkbox.IsChecked = selectedFruitTypes[fruitType];
                Grid.SetColumn(checkbox, 2);
                Grid.SetRow(checkbox, index);

                grid.Children.Add(label);
                grid.Children.Add(icon);
                grid.Children.Add(checkbox);
            }

            return true;
        }

        public override void ObtainData()
        {
            throw new NotImplementedException("Build UI for selecting types");
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

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            NintendoLand.TileTypes.Fruit fruit = tileType as NintendoLand.TileTypes.Fruit;
            var fruitMappings = stage.fruitAssociations[fruit.index-1].Values;
            var fruitTypeMappings = fruitMappings.Select(fruitMapping => fruitData.FruitByID[fruitMapping].FruitType);
            var fruitTypes = new HashSet<FruitData.FruitType>(fruitTypeMappings.SelectMany(mapping => mapping.Values).Select(entry => (FruitData.FruitType)entry));

            return new Fruit()
            {
                selectedFruitTypes =
                    ((FruitData.FruitType[]) Enum.GetValues(typeof(FruitData.FruitType))).ToDictionary(
                        enumValue => enumValue, enumValue => fruitTypes.Contains(enumValue))
            };
        }
        #endregion
    }
}
