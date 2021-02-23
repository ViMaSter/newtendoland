using System;
using System.Windows;
using System.Windows.Controls;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;
using static System.Int32;

namespace tileeditor.GridObjects
{
    class Goal : BaseObject
    {
        public override string DisplayName => "Goal";

        public override string IconFileName => DisplayName;

        public override string DisplayData => levelTarget == -1 ? "Next level" : "Jumps to level " + levelTarget;

        #region Form generator
        private TextBox bonusGoalInput;
        int levelTarget;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            grid.ColumnDefinitions[0].Width = new System.Windows.GridLength(2, System.Windows.GridUnitType.Star);
            // Optional inputs
            Label jumpToLevel = new Label();
            jumpToLevel.Content = "Jump to level:";
            Grid.SetColumn(jumpToLevel, 0);
            Grid.SetRow(jumpToLevel, 1);

            bonusGoalInput = new TextBox();
            bonusGoalInput.Text = (levelTarget == -1 ? (Application.Current.MainWindow as MainWindow).NextLevelIndex+1  : levelTarget).ToString();
            bonusGoalInput.PreviewTextInput += Selector_PreviewTextInput;
            Grid.SetColumn(bonusGoalInput, 1);
            Grid.SetRow(bonusGoalInput, 1);
            Grid.SetColumnSpan(bonusGoalInput, 1);

            grid.Children.Add(jumpToLevel);
            grid.Children.Add(bonusGoalInput);
            return true;
        }

        public override void ObtainData()
        {
            levelTarget = Parse(bonusGoalInput.Text);
        }

        private void Selector_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !ValidateInput(e.Text);
        }

        private bool ValidateInput(string text)
        {
            int input = -1;
            return TryParse(text, out input) && input >= 0 && input <= 99;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Goal;
        }

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            NintendoLand.TileTypes.Goal goal = tileType as NintendoLand.TileTypes.Goal;
            return new Goal() { levelTarget = goal.levelTarget, bonusGoalInput = new TextBox() };
        }
        #endregion

    }
}
