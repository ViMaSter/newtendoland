using System.Collections.Generic;
using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    /// <summary>
    /// Refers to header information containing a pivot and rotating objects (orbiters)
    /// Up to RotatingObject.AVAILABLE_PIVOTS pivots can be placed inside the map
    /// 
    /// The body only refers to the MemoryIdentifier and an order (i.e. ['M', '2']).
    /// Inside the header the actual objects are defined (pivot and orbiters)
    /// 
    /// Examples:
    /// In DataFormats.MapData10.exbin at [5, 6] we find ['M', '1'] inside the body. (The first orbiter.)
    /// Information about the actual objects, is stored inside the header starting with M1 at offset 0x26.
    /// This data is formatted just like the regular DataFormats.MapData, but with only 2 bytes for an order (instead of the regular 3)
    /// and allowing up to 4 items, including the pivot.
    /// 
    /// Going there, we would find [Oa, P1] (a small spike + a regular fruit).
    /// This tells us, the pivot object is a small spike, with a fruit as orbiter around it.
    /// 
    /// At [9, 12] we find ['M', '2']. At 0x32 (M1 at 0x26 + (4 items * 3 bytes)) we find [Oa, P2].
    /// Another small spike as pivot, with a regular fruit as orbiter around it.
    /// 
    /// Exception: More than 3 objects around one pivot
    /// It is possible to have more than 3 objects around a pivot. This is done, by having the
    /// last orbiter be a RotatingObject with +1 of the current order.
    /// Example in DataFormats.MapData38.exbin:
    /// At [7, 8] we find ['M', '1']. At 0x26 (the header containing pivot + orbitters) we find: [S, P1, P2, M2]
    /// Notice the M2 at the end - looking at 0x32 (the header containing the next pivot + orbitters) we find [P3, P4].
    /// Internally, the second list is appended to the first and M2 is dropped, resulting in [S, P1, P2, P3, P4]; the start point
    /// being the pivot with 4 regular fruits as orbiter around it.
    /// </summary>
    public class RotatingObject : BaseObject
    {
        public override string DisplayName
        {
            get
            {
                return "Rotating object";
            }
        }

        public override string GetIconFileName
        {
            get
            {
                return DisplayName.ToString();
            }
        }

        public override string DisplayData
        {
            get
            {
                return "ID: " + index.ToString();
            }
        }

        #region Form generator
        private ComboBox pivotInput;
        private List<BaseObject> orbitters;

        private int index;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label pivotLabel = new Label();
            pivotLabel.Content = "Pivot:";
            Grid.SetColumn(pivotLabel, 0);
            Grid.SetRow(pivotLabel, 0);

            pivotInput = new ComboBox();
            pivotInput.ItemsSource = GridObjects.Registrar.Enumerable;
            pivotInput.SelectedValuePath = "MemoryIdentifier";
            pivotInput.DisplayMemberPath = "DisplayName";
            Grid.SetColumn(pivotInput, 1);
            Grid.SetRow(pivotInput, 0);
            
            // @TODO: + - Button for valid TileTypes
            grid.Children.Add(pivotLabel);
            grid.Children.Add(pivotInput);
            return true;
        }

        public override void ObtainData()
        {
            GridObjects.Registrar.GetTypeByMemoryIdentifier((char)pivotInput.SelectedValue);
        }

        private bool ValidateInput(string text)
        {
            return true;
        }
        #endregion

        #region Conversion
        public override bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return tileType is NintendoLand.TileTypes.RotatingObject;
        }

        public override BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            NintendoLand.TileTypes.RotatingObject rotatingObject = tileType as NintendoLand.TileTypes.RotatingObject;
            return new RotatingObject() {index = rotatingObject.index, orbitters = new List<BaseObject>(), pivotInput = new ComboBox()};
        }
        #endregion
    }
}
