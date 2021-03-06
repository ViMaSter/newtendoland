using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NintendoLand.TileTypes
{
    public abstract class Registrar
    {
        private static Dictionary<char, BaseType> registar = new Dictionary<char, BaseType>();
        public static IEnumerable<BaseType> Enumerable => registar.Values;

        public static BaseType HasTypeByMemoryIdentifier(char identifier)
        {
            return registar[identifier];
        }
        public static BaseType GetTypeByMemoryIdentifier(char identifier)
        {
            return registar[identifier];
        }
        public delegate void ForEachDelegate(BaseType type);
        public static void ForEach(ForEachDelegate callback)
        {
            foreach (BaseType type in registar.Values)
            {
                callback(type);
            }
        }

        // TODO @VM Extension class in tile editor?!
        /*
        public static RoutedEventHandler GenerateOnClickHandler(BaseType type)
        {
            return (object sender, RoutedEventArgs e) =>
            {

            };
        }
        */

        private static void Register(BaseType type)
        {
            registar.Add(type.MemoryIdentifier, type);
        }
        public static void Populate()
        {
            foreach (Type type in
                Assembly.GetAssembly(typeof(BaseType)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseType))))
            {
                Register(((BaseType)Activator.CreateInstance(type)));
            }
        }
        public static void TearDown()
        {
            registar.Clear();
        }
    }
}