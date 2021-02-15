using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Heart : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Heart";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName.ToString();
            }
        }

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

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
