﻿using System.Windows.Controls;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;

namespace tileeditor.GridObjects
{
    public class Pepper : BaseObject
    {
        public override string DisplayName => "Pepper";

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
            return tileType is NintendoLand.TileTypes.PepperOrSwitch pepper && (stage.switchOrPepperDefinitions[pepper.Index-1] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Pepper);
        }

        public override BaseObject FromTileType(BaseType tileType, StageData.Stage stage, FruitData fruitData)
        {
            return new Pepper();
        }
        #endregion
    }
}
