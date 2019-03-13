using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    /// <summary>
    /// Bee enemies are resolved inside StageData
    /// 
    /// Similar to how PepperOrSwitch, Fruit and OrderedFruit derive their type of fruit from another lookup,
    /// the Bee-type can be resolved by using the index (i.e. E2 would be '2')
    /// and looking up it's value inside StageData.
    /// @see tileeditor.DataFormats.StageData.Stage.movementPatterns
    /// </summary>
    class Bee : BaseObject
    {
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
