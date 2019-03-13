using System;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    class Heart : BaseObject
    {
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
                return DisplayName.ToString();
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
