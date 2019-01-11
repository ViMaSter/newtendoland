using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Empty : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '0';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "EMPTY";
            }
        }

        public override bool IsValid()
        {
            return false;
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
    class Nothing : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return ' ';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Nothing";
            }
        }

        public override bool IsValid()
        {
            return false;
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
    class Null : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '\0';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "EMPTY";
            }
        }

        public override bool IsValid()
        {
            return false;
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
