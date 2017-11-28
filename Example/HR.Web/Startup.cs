// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Coddee.AspNet;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Security;
using HR.Data.LinqToSQL;
using HR.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "..\\", "..\\", "..\\", "..\\", "HR.Clients.WPF", "DB"));
            AppDomain.CurrentDomain.SetData("DataDirectory", dbLocation);

            services.AddLogger(LoggerTypes.DebugOutput, LogRecordTypes.Debug);
            services.AddILObjectMapper();
            services.AddLinqRepositoryManager<HRDBManager>(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30", "HR.Data.LinqToSQL");

            services.AddAuthentication();
            services.AddDynamicApiControllers(config =>
            {
                config.RegisterController<AuthController>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidAudience = Configuration["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    ValidateLifetime = true
                }
            });

            app.UseCoddeeDynamicApi();
        }
    }

    public class AuthController
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IUserRepository _userRepository;
        public AuthController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _userRepository = repositoryManager.GetRepository<IUserRepository>();

        }

        [ApiAction("User/AuthenticationUser")]
        public async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            var res = await _userRepository.AuthenticationUser(username, password);
            if (res.Status == AuthenticationStatus.Successfull)
                res.AuthenticationToken = CreateToken(new List<Claim>(), DateTime.Now.AddDays(1));
            return res;
        }

        public static string CreateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                                                 Startup.Configuration["Tokens:Issuer"],
                                                 Startup.Configuration["Tokens:Audience"],
                                                 claims,
                                                 expires: expires,
                                                 signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}