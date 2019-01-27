using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    // @TODO: 'W1' is also used for switches - investigate what determines what to use
    class Pepper : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'W';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Pepper";
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
    }
}
