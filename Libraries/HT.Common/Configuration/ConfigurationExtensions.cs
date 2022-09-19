﻿using Microsoft.Extensions.Configuration;

namespace HT.Common.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T GetConfig<T>(this IConfiguration config, string section) where T : new()
        {
            var settings = new T();
            config.GetSection(section).Bind(settings);
            return settings;
        }

        public static T GetConfig<T>(this IConfiguration config) where T : new()
        {
            var type = typeof(T);
            return GetConfig<T>(config, type.Name);
        }
    }
}
