using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    class OrderedFruit : BaseObject
    {
        public OrderedFruit()
        {
            orderInput = new TextBox();
        }

        public override string DisplayName => "Ordered Fruit";

        public override string IconFileName => DisplayName;

        public override string DisplayData =>
            $"{string.Join(", ", selectedFruitTypes.Where(entry => entry.Value).Select(entry => entry.Key.ToString()))} [Order: {Order}]";

        public int Order => order;
        int order;

        public Dictionary<FruitData.FruitType, bool> selectedFruitTypes;

        #region Form generator
        private TextBox orderInput;
        private CheckBox checkbox;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            int index = -1;
            {
                Label label = new Label();
                label.Content = "Order:";
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, 0);

                orderInput = new TextBox();
                orderInput.Text = Order.ToString();
                orderInput.PreviewTextInput += Selector_PreviewTextInput;
                Grid.SetColumn(orderInput, 1);
                Grid.SetRow(orderInput, ++index);

                grid.Children.Add(label);
                grid.Children.Add(orderInput);
            }



            foreach (FruitData.FruitType fruitType in Enum.GetValues(typeof(FruitData.FruitType)))
            {
                Label label = new Label();
                label.Content = fruitType + ":";
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, ++index);

                checkbox = new CheckBox();
                checkbox.IsChecked = selectedFruitTypes[fruitType];
                Grid.SetColumn(checkbox, 1);
                Grid.SetRow(checkbox, index);

                grid.Children.Add(label);
                grid.Children.Add(checkbox);
            }

            return true;
        }

        public override void ObtainData()
        {
            order = Int32.Parse(orderInput.Text);
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
            return tileType is NintendoLand.TileTypes.OrderedFruit;
        }

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            NintendoLand.TileTypes.OrderedFruit orderedFruit = tileType as NintendoLand.TileTypes.OrderedFruit;
            var orderedFruitMappings = stage.orderedFruitDefinition[orderedFruit.order - 1].Values;
            var orderedFruitTypeMappings = orderedFruitMappings.Select(fruitMapping => fruitData.FruitByID[fruitMapping].FruitType);
            var orderedFruits = new HashSet<FruitData.FruitType>(orderedFruitTypeMappings.SelectMany(mapping => mapping.Values).Select(entry => (FruitData.FruitType)entry));

            return new OrderedFruit()
            {
                order = orderedFruit.order,
                selectedFruitTypes =
                    ((FruitData.FruitType[])Enum.GetValues(typeof(FruitData.FruitType))).ToDictionary(
                        enumValue => enumValue, enumValue => orderedFruits.Contains(enumValue))
            };
        }
        #endregion
    }
}
