using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;

namespace tileeditor.TileTypes
{
    public abstract class TileType
    {
        private static Dictionary<char, TileType> registar = new Dictionary<char, TileType>();
        public static IEnumerable<TileType> Enumerable
        {
            get
            {
                return registar.Values;
            }
        }
        public static TileType HasTypeByMemoryIdentifier(char identifier)
        {
            return TileType.registar[identifier];
        }
        public static TileType GetTypeByMemoryIdentifier(char identifier)
        {
            return TileType.registar[identifier];
        }
        public delegate void ForEachDelegate(TileType type);
        public static void ForEach(ForEachDelegate callback)
        {
            foreach(TileType type in registar.Values)
            {
                callback(type);
            }
        }
        public static RoutedEventHandler GenerateOnClickHandler(TileType type)
        {
            return (object sender, RoutedEventArgs e) =>
            {

            };
        }

        private static void Register(TileType type)
        {
            registar.Add(type.MemoryIdentifier, type);
        }
        public static void PopulateRegistar()
        {
            foreach (Type type in
                Assembly.GetAssembly(typeof(TileType)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(TileType))))
            {
                TileType.Register(((TileType)Activator.CreateInstance(type)));
            }
        }

        protected TileType() { }
        public abstract char MemoryIdentifier { get; }
        public abstract string DisplayName { get; }
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
        public static TileType Construct(BinaryReader reader, int availablePadding)
        {
            TileType tileType = (TileType)Activator.CreateInstance((TileTypes.TileType.GetTypeByMemoryIdentifier((char)reader.ReadChar())).GetType());
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
