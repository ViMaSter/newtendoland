using System.Windows.Controls;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    class Switch : BaseObject
    {
        public override string DisplayName => "Switch";

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
            return tileType is PepperOrSwitch switchObject
                   && stage.switchOrPepperDefinitions[switchObject.Index-1] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Switch;
        }

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            return new Switch();
        }
        #endregion
    }
}
