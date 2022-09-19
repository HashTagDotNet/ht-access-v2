using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace HT.Common.Reflection
{
    
    /// <summary>
    /// Create an instance of a class from an external assembly
    /// </summary>
    /// <typeparam name="T">Type of class to return</typeparam>
    public static partial class ProviderFactory<T>
    {
        private static ConcurrentDictionary<string, Assembly> _asmCache = new ConcurrentDictionary<string, Assembly>();

        /// <summary>
        /// Returns a new instance of a provider based class.  Executes default constructor of class
        /// </summary>
        /// <param name="typeName">Fully qualified type name for class</param>
        /// <param name="assemblyName">Fully qualified assembly name for class</param>
        /// <param name="isExternalReference">True if assemblyName is a fully qualified path name</param>
        /// <returns>Instantiated class</returns>
        public static T GetInstance(string typeName, string assemblyName, bool isExternalReference)
        {
            return GetInstance(typeName, assemblyName, isExternalReference, null);
        }

    

        /// <summary>
        /// Returns a new instance of a provider based class.  Executes the constructor
        /// of the class that contains a signature matching the arguments list.  If
        /// arguments is null, then execute default constructor
        /// </summary>
        /// <param name="typeName">Full.Type.Name to instantiate</param>
        /// <param name="assemblyName">Assembly.Name containing type to instantiate</param>
        /// <param name="arguments">Arguments to pass to type's constructor</param>
        /// <param name="isExternalReference">True if assemblyName is a fully qualified path name</param>
        /// <returns>Instantiated class</returns>
        public static T GetInstance(string typeName, string assemblyName, bool isExternalReference, params object[] arguments)
        {
            T type = (T) GetAssembly(assemblyName,isExternalReference).CreateInstance(typeName,
                true,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                null, arguments, null, null);

            return type;
        }

        /// <summary>
        /// Returns a new instance of a provider based class.  Executes the constructor
        /// of the class that contains a signature matching the arguments list.  If
        /// arguments is null, then execute default constructor
        /// </summary>
        /// <param name="longName">Full.Type.Name, Assembly.Name</param>
        /// <param name="arguments">Arguments to pass to type's constructor</param>
        /// <returns></returns>
        public static T GetInstance(string longName, params object[] arguments)
        {
            string[] s = longName.Split(new char[] { ',' });
            if (s.Length != 2)
                throw new ArgumentException("longName must be of format: <fully qualified type name>,<assembly name>");
            return GetInstance(s[0], s[1], false,arguments);


        }
      
       


        private static string getAssemblyName(string assemblyName)
        {
            string[] s = assemblyName.Split(new char[] { ',' }, StringSplitOptions.None);
            if (s.Length == 1) return s[0];  
            if (s.Length == 2) return s[1];
            throw new ArgumentException("Unable to find assemblyName in '" + assemblyName + "'");
        }
        private static string getTypeName(string typeName)
        {
            
            string[] s = typeName.Split(new char[] { ',' }, StringSplitOptions.None);
            if (s.Length >= 1) return s[0];

            throw new ArgumentException("Unable to find typeName in '" + typeName + "'");
        }

        /// <summary>
        /// Return a loaded assembly.  First from internal cache then from external assembly.  Generally a helper method.
        /// </summary>
        /// <param name="assemblyName">Assembly name for type,assembly combination</param>
        /// <param name="isExternalReference">True if assemblyName is a fully qualified path name</param>
        /// <returns>Loaded assembly.</returns>
        public static Assembly GetAssembly(string assemblyName, bool isExternalReference)
        {
            string asmName = getAssemblyName(assemblyName);
            if (_asmCache.TryGetValue(asmName, out var assembly))
            {
                return assembly;
            }
            else
            {
                try
                {
                    Assembly assm;
                    if (isExternalReference == false) // store in static list
                    {
                        assm = Assembly.Load(asmName);
                    }
                    else
                    {
                        assm = Assembly.LoadFile(asmName);
                    }
                    _asmCache.TryAdd(asmName, assm);
                }
                catch 
                {
                    throw; //for debugging;
                }
                return GetAssembly(assemblyName, isExternalReference); // get assembly from cache
            }
        }
    } //ProviderFactory
} //namespace