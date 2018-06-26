// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee;
using Coddee.AppBuilder;
using Coddee.AspNet;
using Coddee.AspNet.LinqToSql;
using Coddee.Attributes;
using Coddee.Loggers;
using Coddee.Windows.AppBuilder;
using HR.Data.LinqToSQL;
using HR.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace HR.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "..\\", "..\\", "..\\", "..\\", "DB"));
            AppDomain.CurrentDomain.SetData("DataDirectory", dbLocation);
            services.AddContainer();
            services.AddLogger(new LoggerOptions(LoggerTypes.DebugOutput, LogRecordTypes.Debug, AppDomain.CurrentDomain.BaseDirectory));
            services.AddILObjectMapper();
            services.AddTransientRepositoryManager();
            services.AddLinqRepositories<HRDBManager>(new LinqInitializerConfig(c => @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30", "HR.Data.LinqToSQL"));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidAudience = Configuration["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    ValidateLifetime = true,
                };
            });

            services.AddDynamicApi(config =>
            {
                config.UseLoggingPage = true;
                config.ReturnException = false;
                config.UseErrorPages = true;
                config.CacheRepositoryActionsOnStartup = true;
                config.AuthorizationValidator = new JwtAuthorizationValidator();
                config.GetApiContext = CreateContextObject;
            }, new[]
            {
                typeof(AuthController),
                typeof(CompanyController),
            });

            var temp = new DebugOuputLogger();
            temp.Initialize(LogRecordTypes.Debug);
            temp.Log("WebApp", "DebugAppStarted");
        }

        private int count = 0;
        private object CreateContextObject(DynamicApiRequest arg)
        {
            return ++count;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();
            app.UseCoddeeDynamicApi();
        }
    }

    public class CompanyController
    {
        [ApiAction("Company/GetCompaniesById", HttpMethod.Post)]
        [Authorize]
        public Task<IEnumerable<Company>> GetCompaniesById(int id)
        {
            return Task.Run(() => new[]
            {
                new Company {Name = "Company123"}
            }.AsEnumerable());
        }
    }
}