using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Fruit : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'P';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Fruit (Regular)";
            }
        }

        public static int HighestCurrentlyUsedIndex = -1;

        enum SpikeStartState
        {
            Closed = '+',
            Open = '-'
        }

        #region Form generator
        private TextBox indexInput;
        int index;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(5, System.Windows.GridUnitType.Star) });

            Label label = new Label();
            label.Content = "Index:";
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            label.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);

            indexInput = new TextBox();
            indexInput.Text = Fruit.HighestCurrentlyUsedIndex.ToString();
            indexInput.PreviewTextInput += Selector_PreviewTextInput;
            indexInput.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            indexInput.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Grid.SetRow(label, 0);
            Grid.SetColumn(indexInput, 1);

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
    }
}
