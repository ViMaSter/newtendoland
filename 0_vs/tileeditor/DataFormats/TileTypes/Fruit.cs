using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Fruit : BaseType
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

        public override string GetIconFileName
        {
            get
            {
                return MemoryIdentifier.ToString();
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

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            Debug.Assert(parsableBytes.Count >= 1, "index of Regular fruit", "Regular fruits require an index - none found");
            index = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
        }
        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            byte[] indexStringified = System.Text.Encoding.ASCII.GetBytes(index.ToString());
            target.AddRange(indexStringified);
            bytesLeft -= indexStringified.Length;

            return true;
        }
        #endregion
    }
}
