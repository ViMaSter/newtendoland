﻿using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class OrderedFruit : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'F';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Fruit (Ordered)";
            }
        }

        public static int HighestCurrentlyUsedorder = 0;

        #region Form generator
        private TextBox orderInput;
        int order;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label label = new Label();
            label.Content = "Order:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            orderInput = new TextBox();
            orderInput.Text = OrderedFruit.HighestCurrentlyUsedorder.ToString();
            orderInput.PreviewTextInput += Selector_PreviewTextInput;
            Grid.SetColumn(orderInput, 1);
            Grid.SetRow(orderInput, 0);

            grid.Children.Add(label);
            grid.Children.Add(orderInput);
            return true;
        }

        public override void ObtainData()
        {
            order = Int32.Parse(orderInput.Text);
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
    }
}