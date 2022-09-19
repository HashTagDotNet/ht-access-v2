using HT.Common.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace HT.Common.Reflection
{
    public static class AssemblyExtensions
    {
        public static string Title(this Assembly assembly)
        {
            return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute), false))?.Title ?? "";
        }
        public static string FileVersion(this Assembly assembly)
        {
            return ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute), false))?.Version ?? "";
        }
        public static string InformationalVersion(this Assembly assembly)
        {
            return ((AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute), false))?.InformationalVersion ?? assembly.FileVersion() ?? "";
        }
        public static string Product(this Assembly assembly)
        {
            return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute), false))?.Product ?? "";
        }

        public static string Configuration(this Assembly assembly)
        {
            return ((AssemblyConfigurationAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyConfigurationAttribute), false))?.Configuration ?? "";
        }
        public static string Company(this Assembly assembly)
        {
            return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute), false))?.Company ?? "";
        }
        public static string TargetFramework(this Assembly assembly)
        {
            return ((TargetFrameworkAttribute)Attribute.GetCustomAttribute(assembly, typeof(TargetFrameworkAttribute), false))?.FrameworkDisplayName ?? "";
        }
        public static string Description(this Assembly assembly)
        {
            return ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute), false))?.Description ?? "";
        }
        public static string ApplicationName(this Assembly assembly)
        {
            return assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
        }
        public static string Title(this Type type)
        {
            return type.Assembly.Title();
        }
        public static string FileVersion(this Type type)
        {
            return type.Assembly.FileVersion();
        }
        public static string InformationalVersion(this Type type)
        {
            return type.Assembly.InformationalVersion();
        }
        public static string Product(this Type type)
        {
            return type.Assembly.Product();
        }

        public static string Configuration(this Type type)
        {
            return type.Assembly.Configuration();
        }
        public static string Company(this Type type)
        {
            return type.Assembly.Company();
        }
        public static string TargetFramework(this Type type)
        {
            return type.Assembly.TargetFramework();
        }
        public static string Description(this Type type)
        {
            return type.Assembly.Description();
        }
        public static string ApplicationName(this Type type)
        {
            return type.Assembly.ApplicationName();
        }
    }
}
