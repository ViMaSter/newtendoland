using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    class PepperOrSwitch : BaseType
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
