﻿using HT.Access.Admin.Service.Configuration;
using HT.Access.Admin.Service.Cryptography;
using HT.Access.Admin.Service.Cryptography.Interfaces;
using HT.Access.Admin.Service.LDAP;
using HT.Access.Admin.Service.LDAP.Interfaces;
using HT.Access.Admin.Service.LDAP.Runners;
using HT.Extensions.SqlClient;
using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HT.Access.Admin.Service
{
    public static class RegisterServicesExtensions
    {
        public static void RegisterHTServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<ICryptographyService, CryptographyService>();
            services.TryAddSingleton<ILdifBuilder,LdifBuilder>();
            services.TryAddSingleton<ILdifRunner,LdifSqlRunner>();
        }

        public static void RegisterHTAccessPersisters(this IServiceCollection services, IConfiguration configuration)
        {

        }

        public static void LoadHTAccessConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("HTAccess").Get<HTConfig>();

            var dbOptions = configuration.GetSection("HTAccess:DbOptions").Get<DbConnectorOptions>();
            services.AddSingleton(dbOptions);
            services.AddSingleton(config);
            services.AddSingleton(typeof(IDbConnector), s => new DbConnector(
                s.GetRequiredService<HTConfig>().ConnectionStrings["HTAccess"],
                s.GetRequiredService<DbConnectorOptions>()));
        }
    }
}
