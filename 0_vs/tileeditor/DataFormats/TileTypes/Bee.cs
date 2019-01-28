using System;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Bee : BaseType
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
