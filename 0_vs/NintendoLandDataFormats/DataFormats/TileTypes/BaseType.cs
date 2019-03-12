using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NintendoLand.TileTypes
{
    public abstract class BaseType
    {
        #region Instance
        protected BaseType() { }

        // how this type of object is represented in .exbin files
        public abstract char MemoryIdentifier { get; }

        // whether or not this is game-impacting type
        public virtual bool IsValid()
        {
            return true;
        }

        // base implementation of loading a byte-stream into an instance of this class 
        protected virtual void Load(List<byte> parsableBytes)
        {
            Debug.Assert(parsableBytes[0] == MemoryIdentifier);
            parsableBytes.RemoveAt(0);
        }

        // base implementation of writing an instance of this class to memory
        public virtual bool SerializeExbin(ref List<byte> target, ref int bytesLeft)
        {
            target.Add((byte)MemoryIdentifier);
            --bytesLeft;

            return false;
        }
        #endregion

        #region Static
        // construct an instance of a base type form a byte-buffer
        public static BaseType Construct(List<byte> parsableBytes)
        {
            BaseType tileType = (BaseType)Activator.CreateInstance((TileTypes.Registrar.GetTypeByMemoryIdentifier((char)parsableBytes[0])).GetType());
            tileType.Load(parsableBytes);

            return tileType;
        }
        #endregion
    }
}
