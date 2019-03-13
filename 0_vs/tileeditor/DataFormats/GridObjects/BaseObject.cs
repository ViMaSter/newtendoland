using System.Windows.Controls;

namespace tileeditor.GridObjects
{
    public abstract class BaseObject
    {
        protected BaseObject() { }
        public abstract string DisplayName { get; }
        public abstract string GetIconFileName { get; }
        public virtual string DisplayData
        {
            get
            {
                return "";
            }
        }

        public abstract bool PopulateFields(ref Grid grid);
        public abstract void ObtainData();

        public virtual bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return false;
        }

        public abstract BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage);
    }
}
