using System.Windows.Controls;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    class Pepper : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Pepper";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName;
            }
        }

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
            return tileType is PepperOrSwitch pepper && (stage.switchOrPepperDefinitions[pepper.Index] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Switch);
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
