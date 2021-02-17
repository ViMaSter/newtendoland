using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    /// <summary>
    /// Bee enemies are resolved inside StageData
    /// 
    /// Similar to how PepperOrSwitch, Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the Bee-type can be resolved by using the index (i.e. E2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see tileeditor.DataFormats.StageData.Stage.movementPatterns
    /// </summary>
    class Bee : BaseObject
    {
        public override string DisplayName => "Bee";

        public override string IconFileName => DisplayName;

        int index;

        #region Form generator
        public override bool PopulateFields(ref Grid grid)
        {
            return false;
        }

        public override void ObtainData()
        {
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.Bee;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            NintendoLand.TileTypes.Bee bee = tileType as NintendoLand.TileTypes.Bee;
            return new Bee() { index = bee.index };
        }
        #endregion
    }
}
