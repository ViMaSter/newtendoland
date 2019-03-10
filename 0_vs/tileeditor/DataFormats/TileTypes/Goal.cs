using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Goal : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'G';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Goal";
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
                return levelTarget == -1 ? "Regular" : "Jumps to " + levelTarget.ToString();
            }
        }

        #region Form generator
        private CheckBox isBonusGoal;
        private TextBox bonusGoalInput;
        int levelTarget;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
            // Optional inputs
            Label jumpToLevel = new Label();
            jumpToLevel.Content = "Jump to level:";
            jumpToLevel.Visibility = System.Windows.Visibility.Hidden;
            Grid.SetColumn(jumpToLevel, 0);
            Grid.SetRow(jumpToLevel, 1);

            bonusGoalInput = new TextBox();
            bonusGoalInput.Text = "1";
            bonusGoalInput.PreviewTextInput += Selector_PreviewTextInput;
            bonusGoalInput.Visibility = System.Windows.Visibility.Hidden;
            Grid.SetColumn(bonusGoalInput, 1);
            Grid.SetRow(bonusGoalInput, 1);
            Grid.SetColumnSpan(bonusGoalInput, 1);

            // Regular input
            isBonusGoal = new CheckBox();
            isBonusGoal.IsChecked = false;
            isBonusGoal.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(isBonusGoal, 0);
            Grid.SetRow(isBonusGoal, 0);
            Grid.SetColumnSpan(isBonusGoal, 2);
            isBonusGoal.Checked += (object sender, System.Windows.RoutedEventArgs e) =>
            {
                bonusGoalInput.Visibility = (sender as CheckBox).IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                jumpToLevel.Visibility = (sender as CheckBox).IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            };
            isBonusGoal.Unchecked += (object sender, System.Windows.RoutedEventArgs e) =>
            {
                bonusGoalInput.Visibility = (sender as CheckBox).IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                jumpToLevel.Visibility = (sender as CheckBox).IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            };
            isBonusGoal.Content = "Is bonus goal";

            grid.Children.Add(isBonusGoal);
            grid.Children.Add(jumpToLevel);
            grid.Children.Add(bonusGoalInput);
            return true;
        }

        public override void ObtainData()
        {
            levelTarget = isBonusGoal.IsChecked == true ? Int32.Parse(bonusGoalInput.Text) : 0;
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

        protected override void Load(List<byte> parsableBytes)
        {
            base.Load(parsableBytes);

            if (parsableBytes.Count == 0)
            {
                levelTarget = -1;
            }
            else
            {
                levelTarget = Int32.Parse(System.Text.Encoding.Default.GetString(parsableBytes.ToArray()));
            }
        }

        public override bool SerializeExbin(ref System.Collections.Generic.List<byte> target, ref int bytesLeft)
        {
            // add memory identifier
            target.Add((byte)MemoryIdentifier);
            bytesLeft--;

            // add stringified index
            if (levelTarget != -1)
            {
                byte[] indexStringified = System.Text.Encoding.ASCII.GetBytes(levelTarget.ToString());
                Debug.Assert(bytesLeft >= indexStringified.Length, "No bytes left in buffer for goal index");
                target.AddRange(indexStringified);
                bytesLeft -= indexStringified.Length;
            }

            return true;
        }
    }
}
