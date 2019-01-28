using System;
using System.IO;
using System.Windows.Controls;

namespace tileeditor.TileTypes
{
    public abstract class BaseType
    {
        protected BaseType() { }
        public abstract char MemoryIdentifier { get; }
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
        public virtual bool IsValid()
        {
            return true;
        }
        protected virtual bool Load(BinaryReader reader, int availablePadding)
        {
            return false;
        }
        public static BaseType Construct(BinaryReader reader, int availablePadding)
        {
            BaseType tileType = (BaseType)Activator.CreateInstance((TileTypes.Registrar.GetTypeByMemoryIdentifier((char)reader.ReadChar())).GetType());
            if (!tileType.Load(reader, availablePadding))
            {
                // skip additional available memory if this type of tile doesn't care about it
                for (int i = 0; i < availablePadding; i++)
                {
                    reader.ReadByte();
                }
            }
            return tileType;
        }
    }
}
