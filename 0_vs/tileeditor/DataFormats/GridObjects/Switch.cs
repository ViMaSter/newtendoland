using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Switch : BaseObject
    {
        public override string DisplayName => "Switch";

        public override string IconFileName => DisplayName;

        public int Index => index;
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
            return tileType is NintendoLand.TileTypes.PepperOrSwitch
                   && (stage.switchOrPepperDefinitions[((NintendoLand.TileTypes.PepperOrSwitch)tileType).Index-1] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Switch);
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            NintendoLand.TileTypes.PepperOrSwitch pepperOrSwitch = tileType as NintendoLand.TileTypes.PepperOrSwitch;
            return new Switch() {index = pepperOrSwitch.Index};
        }
        #endregion
    }
}
