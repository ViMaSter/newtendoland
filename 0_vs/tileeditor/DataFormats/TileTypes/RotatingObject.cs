using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    /// <summary>
    /// Refers to header information containing a pivot and rotating objects (orbiters)
    /// Up to RotatingObject.AVAILABLE_PIVOTS pivots can be placed inside the map
    /// 
    /// The body only refers to the MemoryIdentifier and an index (i.e. ['M', '2']).
    /// Inside the header the actual objects are defined (pivot and orbiters)
    /// 
    /// Examples:
    /// In DataFormats.MapData10.exbin at [5, 6] we find ['M', '1'] inside the body. (The first orbiter.)
    /// Information about the actual objects, is stored inside the header starting with M1 at offset 0x26.
    /// This data is formatted just like the regular DataFormats.MapData, but with only 2 bytes for an index (instead of the regular 3)
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
    /// last orbiter be a RotatingObject with +1 of the current index.
    /// Example in DataFormats.MapData38.exbin:
    /// At [7, 8] we find ['M', '1']. At 0x26 (the header containing pivot + orbitters) we find: [S, P1, P2, M2]
    /// Notice the M2 at the end - looking at 0x32 (the header containing the next pivot + orbitters) we find [P3, P4].
    /// Internally, the second list is appended to the first and M2 is dropped, resulting in [S, P1, P2, P3, P4]; the start point
    /// being the pivot with 4 regular fruits as orbiter around it.
    /// </summary>
    class RotatingObject : TileType
    {
        public override char MemoryIdentifier
        {
            get
            {
                return 'M';
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Rotating object";
            }
        }

        public override string DisplayData
        {
            get
            {
                return "Contains: " + rotatingObjects.ToString();
            }
        }

        private const int AVAILABLE_PIVOTS = 8;
        private const int SLOTS_PER_PIVOT = 3;
        public const int MAXIMUM_ORBITERS = AVAILABLE_PIVOTS * SLOTS_PER_PIVOT;

        public static RotatingObject[] Load(BinaryReader reader)
        {
            List<RotatingObject> rotatingObjects = new List<RotatingObject>();
            for (int i = 0; i < AVAILABLE_PIVOTS; i++)
            {
                reader.ReadBytes(DataFormats.MapData.HEADER_TILE_PADDING + 1);
                reader.ReadBytes(DataFormats.MapData.HEADER_TILE_PADDING + 1);
                reader.ReadBytes(DataFormats.MapData.HEADER_TILE_PADDING + 1);
                reader.ReadBytes(DataFormats.MapData.HEADER_TILE_PADDING + 1);
                // @TODO: Rewrite orbiter-parser similar to IndexResolver
            }
            return rotatingObjects.ToArray();
        }

        #region Form generator
        private TileTypes.TileType pivotObject;
        private TileTypes.TileType[] rotatingObjects;
        private ComboBox pivotInput;

        // implement common methods
        public override bool PopulateFields(ref Grid grid)
        {
            Label pivotLabel = new Label();
            pivotLabel.Content = "Pivot:";
            Grid.SetColumn(pivotLabel, 0);
            Grid.SetRow(pivotLabel, 0);

            pivotInput = new ComboBox();
            pivotInput.ItemsSource = TileTypes.TileType.Enumerable;
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
            pivotObject = TileTypes.TileType.GetTypeByMemoryIdentifier((char)pivotInput.SelectedValue);
        }

        private bool ValidateInput(string text)
        {
            return true;
        }

        protected override bool Load(BinaryReader reader, int availablePadding)
        {
            rotatingObjects = new TileTypes.TileType[0];
            return false;
        }

        #endregion
    }
}
