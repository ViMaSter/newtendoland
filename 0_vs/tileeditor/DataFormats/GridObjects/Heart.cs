using System.Windows.Controls;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    class Heart : BaseObject
    {
        public override string DisplayName => "Heart";

        public override string IconFileName => DisplayName;

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
            return tileType is NintendoLand.TileTypes.Heart;
        }

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            return new Heart();
        }
        #endregion
    }
}
