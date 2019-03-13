namespace tileeditor.DataFormats
{
    class MapDescriptor
    {
        public int mapID;
        public byte[] tutorialText1;
        public byte[] tutorialText2;
        public int backgroundID;
        public GridObjects.BaseObject[,] grid;

        NintendoLand.DataFormats.MapData MapDescriptorToGameData(MapDescriptor descriptor)
        {
            throw new System.NotImplementedException();
        }
    }
}
