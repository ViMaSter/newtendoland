using System;
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
        public static TileType GetTypeByMemoryIdentifier(char identifier)
        {
            return TileType.registar[identifier];
        }
        public delegate void ForEachDelegate(TileType type);
        public static void ForEachType(ForEachDelegate callback)
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

        public abstract bool PopulateFields(ref Grid grid);
        public abstract void ObtainData();
    }
}
