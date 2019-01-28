using System;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class Empty : BaseType
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

        public override string GetIconFileName
        {
            get
            {
                return "unknown";
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

    class Nothing : BaseType
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

        public override string GetIconFileName
        {
            get
            {
                return "unknown";
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

    class Null : BaseType
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

        public override string GetIconFileName
        {
            get
            {
                return "unknown";
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

        public override string GetIconFileName
        {
            get
            {
                return "unknown";
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

    class Wall : BaseType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return '*';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Wall";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return "unknown";
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
