using System;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Start : BaseObject
    {
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
                return DisplayName;
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
