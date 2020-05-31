/**
 * Source : http://www.sanity-free.com/132/generic_singleton_pattern_in_csharp.html
 **/

using System;
using System.Reflection;

namespace OneDevApp
{
    public class Singleton<T> where T : class
    {
        private static object s_syncRoot = new object();
        private static T s_instance = null;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_syncRoot)
                    {
                        if (s_instance == null)
                        {
                            ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                            if (ci == null) { throw new InvalidOperationException("class must contain a private constructor"); }
                            s_instance = (T)ci.Invoke(null);
                        }
                    }
                }

                return s_instance;
            }
        }
    }
}