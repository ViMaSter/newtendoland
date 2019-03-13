namespace tileeditor.DataFormats
{
    class MapDescriptor
    {
        public byte mapID;
        public byte[] tutorialText1;
        public byte[] tutorialText2;
        public NintendoLand.DataFormats.IndexResolver backgroundID;
        public GridObjects.BaseObject[,] grid;

        private MapDescriptor() { }

        #region Conversion
        public static NintendoLand.DataFormats.MapData ToGameData(MapDescriptor descriptor)
        {
            throw new System.NotImplementedException();
        }

        public static MapDescriptor FromGameData(string fileName, NintendoLand.DataFormats.GameDataContainer gameDataContainer)
        {
            MapDescriptor mapDescriptor = new MapDescriptor();
            NintendoLand.DataFormats.Level level = gameDataContainer.GetLevelInfo(fileName);
            mapDescriptor.mapID = level.stage.ID;
            mapDescriptor.backgroundID = level.stage.backgroundID;
            mapDescriptor.tutorialText1 = level.stage.tutorialText1;
            mapDescriptor.tutorialText2 = level.stage.tutorialText2;
            for (int row = 0; row < NintendoLand.DataFormats.MapData.ROWS_TOTAL; row++)
            {
                for (int column = 0; column < NintendoLand.DataFormats.MapData.COLUMNS_TOTAL; column++)
                {
                    NintendoLand.TileTypes.BaseType type = level.mapData.GetItem(row, column);
                    GridObjects.BaseObject createdObject = null;
                    GridObjects.Registrar.ForEach((GridObjects.BaseObject baseType) =>
                    {
                        if (baseType.CanConvert(type, level.stage))
                        {
                            createdObject = baseType.FromTileType(type, level.stage);
                        }
                    });
                    System.Diagnostics.Debug.Assert(
                        createdObject == null,
                        "No valid GridObject conversion",
                        "No GridObject could be created from tile [{0}, {1}]: {2}", row, column, type.ToString()
                    );
                    mapDescriptor.grid[row, column] = createdObject;
                }
            }
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
