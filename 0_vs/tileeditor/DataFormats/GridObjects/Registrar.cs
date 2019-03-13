using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace tileeditor.GridObjects
{
    public abstract class Registrar
    {
        private static List<BaseObject> registar = new List<BaseObject>();
        public static IEnumerable<BaseObject> Enumerable
        {
            get
            {
                return registar;
            }
        }
        public static BaseObject HasTypeByMemoryIdentifier(char identifier)
        {
            return registar[identifier];
        }
        public static BaseObject GetTypeByMemoryIdentifier(char identifier)
        {
            return registar[identifier];
        }
        public delegate void ForEachDelegate(BaseObject type);
        public static void ForEach(ForEachDelegate callback)
        {
            foreach (BaseObject type in registar)
            {
                callback(type);
            }
        }
        public static RoutedEventHandler GenerateOnClickHandler(BaseObject type)
        {
            return (object sender, RoutedEventArgs e) =>
            {

            };
        }

        private static void Register(BaseObject type)
        {
            registar.Add(type);
        }
        public static void Populate()
        {
            foreach (Type type in
                Assembly.GetAssembly(typeof(BaseObject)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseObject))))
            {
                Register(((BaseObject)Activator.CreateInstance(type)));
            }
        }
        public static void TearDown()
        {
            registar.Clear();
        }
    }
}