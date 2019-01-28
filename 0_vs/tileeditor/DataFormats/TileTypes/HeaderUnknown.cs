using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class HeaderUnknown : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '#';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "HeaderUnknown";
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

        public override bool IsValid()
        {
            return false;
        }
    }
}
