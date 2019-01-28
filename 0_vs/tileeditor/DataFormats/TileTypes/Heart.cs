using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Heart : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'H';
            }
        }

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
                return MemoryIdentifier.ToString();
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
