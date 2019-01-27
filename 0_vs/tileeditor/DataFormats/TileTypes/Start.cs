using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Start : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'S';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Start";
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
