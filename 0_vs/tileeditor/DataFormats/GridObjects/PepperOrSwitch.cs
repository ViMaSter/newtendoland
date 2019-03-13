using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    /// <summary>
    /// PepperOrSwitch are resolved inside StageData
    /// 
    /// Similar to how Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the PepperOrSwitch-type can be resolved by using the index (i.e. W2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see tileeditor.DataFormats.StageData.PepperOrSwitchFlag
    /// </summary>
    class PepperOrSwitch : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "PepperOrSwitch";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName;
            }
        }

        int index;

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
