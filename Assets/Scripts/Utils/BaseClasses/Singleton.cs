namespace Utils.BaseClasses
{
        public class Singleton
        {
        }
    
        /// <summary>
        /// Türetilecek tim sınıflara "Ins" tanımı ekler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Singleton<T> : Singleton where T : new()
        {
            private static T instance;
            public static T Instance => instance != null ? instance : (instance = new T());
        }
}