using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace tileeditor.GridObjects
{
    public abstract class BaseObject
    {
        protected BaseObject() { }
        public abstract string DisplayName { get; }
        public virtual string IconFileName => "unknown";
        private int column;
        private Point position;
        public Point Position => position;

        public Image Icon => new Image
        {
            Source = new BitmapImage(new Uri(
                $"pack://application:,,,/tileeditor;component/Resources/TileTypes/{IconFileName}.png",
                UriKind.Absolute))
        };

        public virtual string DisplayData => "";

        public abstract bool PopulateFields(ref Grid grid);
        public abstract void ObtainData();

        public virtual bool CanConvert(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage)
        {
            return false;
        }

        public abstract BaseObject FromTileType(NintendoLand.TileTypes.BaseType tileType, NintendoLand.DataFormats.StageData.Stage stage);

        public void SetPosition(int column, int row)
        {
            this.position = new Point(column, row);
        }
    }
}
