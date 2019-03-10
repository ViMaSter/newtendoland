using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected virtual void Load(List<byte> parsableBytes)
        {
            Debug.Assert(parsableBytes[0] == MemoryIdentifier);
            parsableBytes.RemoveAt(0);
        }
        public virtual bool SerializeExbin(ref List<byte> target, ref int bytesLeft)
        {
            target.Add((byte)MemoryIdentifier);
            --bytesLeft;

            return false;
        }
        public static BaseType Construct(List<byte> parsableBytes)
        {
            BaseType tileType = (BaseType)Activator.CreateInstance((TileTypes.Registrar.GetTypeByMemoryIdentifier((char)parsableBytes[0])).GetType());
            tileType.Load(parsableBytes);

            return tileType;
        }
    }
}
