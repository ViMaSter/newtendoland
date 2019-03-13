using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Spike : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Spike";
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
                return state.ToString();
            }
        }

        enum SpikeStartState
        {
            Closed = '+',
            Open = '-'
        }

        #region Form generator
        private ComboBox selector;
        SpikeStartState state;
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(3, System.Windows.GridUnitType.Star);
            Label label = new Label();
            label.Content = "Spike style:";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);

            selector = new ComboBox();
            foreach (SpikeStartState spikeState in Enum.GetValues(typeof(SpikeStartState)))
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
            state = (SpikeStartState)selector.SelectedItem;
        }
        #endregion
    }
}
