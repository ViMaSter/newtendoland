using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Bee : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'E';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Bee";
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
