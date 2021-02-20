using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    public class Pepper : BaseObject
    {
        public override string DisplayName => "Pepper";

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
            return tileType is NintendoLand.TileTypes.PepperOrSwitch pepper && (stage.switchOrPepperDefinitions[pepper.Index-1] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Pepper);
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            NintendoLand.TileTypes.PepperOrSwitch pepperOrSwitch = tileType as NintendoLand.TileTypes.PepperOrSwitch;
            return new Pepper() { index = pepperOrSwitch.Index };
        }
        #endregion
    }
}
