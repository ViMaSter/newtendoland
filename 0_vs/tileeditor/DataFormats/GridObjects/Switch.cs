﻿using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Switch : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Switch";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName;
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
            return tileType is NintendoLand.TileTypes.PepperOrSwitch
                   && (stage.switchOrPepperDefinitions[((NintendoLand.TileTypes.PepperOrSwitch)tileType).Index] == NintendoLand.DataFormats.StageData.Stage.PepperOrSwitchFlag.Switch);
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}