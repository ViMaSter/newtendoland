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
