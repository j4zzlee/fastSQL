using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.MsDependencyInjection;
using Castle.MicroKernel.Lifestyle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastSQL.API
{
    public class Startup
    {
        private static IWindsorContainer _container;
        public Startup(IConfiguration configuration)
        {
            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            _container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(() => _container).LifestyleSingleton());
            
            _container.Register(Component.For<DbConnection>().UsingFactoryMethod((p) => {
                var env = p.Resolve<IHostingEnvironment>();
                var conf = p.Resolve<IConfiguration>();
                var connectionString = conf.GetConnectionString("__MigrationDatabase");
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            }).LifestyleCustom<MsScopedLifestyleManager>());
            _container.Register(Component.For<DbTransaction>().UsingFactoryMethod((c) => {
                var conn = c.Resolve<DbConnection>();
                return conn.BeginTransaction();
            }).LifestyleCustom<MsScopedLifestyleManager>());
            _container.Install(FromAssembly.InThisApplication(GetType().Assembly));

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            return WindsorRegistrationHelper.CreateServiceProvider(_container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //_container.Register(Component.For<IConfigurationRoot>().UsingFactoryMethod(p => {
            //    var builder = new ConfigurationBuilder()
            //        .SetBasePath(env.ContentRootPath)
            //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            //        .AddEnvironmentVariables();
            //    return builder.Build();
            //}).LifestyleCustom<MsScopedLifestyleManager>());
            //_container.Register(Component.For<IHostingEnvironment>().UsingFactoryMethod(p => env).LifestyleCustom<MsScopedLifestyleManager>());
            app.UseCors(options => options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            app.UseMvc();
        }
    }
}
