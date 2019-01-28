using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Start : BaseType
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
