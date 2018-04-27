using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StoreBox.Models;

namespace StoreBox
{
    public class Startup
    {
        private string key = "FQ=LTPg6Bxs8.A:}i&do:^!P";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data"))); 
            services.AddMvc();
            services.AddDbContext<FileContext>(options => options.UseSqlite("Data Source=DBFile.db"));
            services.AddDirectoryBrowser();//ajout de du contexte de la base de données

            services.AddAuthentication()
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false; //retire la sécurité https (pour le développement)
                cfg.SaveToken = true; //sauvegarde du token
                cfg.TokenValidationParameters = new TokenValidationParameters //application des paramètres de l'authentification
                {
                    ValidateIssuerSigningKey = true, //authentification requière une clé
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), //application de la clé
                    ValidIssuer = "http://localhost/home",
                    ValidAudience = "http://localhost/home",
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Data")),
                RequestPath = "/Data"
            });

            app.UseMiddleware<Controllers.JWTInHeaderMiddleware>();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "home",
                    template: "home/{directory?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "homeaction",
                    template: "Download",
                    defaults: new { controller = "Home", action = "Download" });

                routes.MapRoute(
                    name: "upload",
                    template: "UploadFile",
                    defaults: new { controller = "Home", action = "UploadFile" });

                /*
                routes.MapRoute(
                    name: "default",
                    template: "home",
                    defaults: new { controller = "Home", action = "Index" }
                );*/
                routes.MapRoute(
                    name: "default",
                        template: "{controller=Login}/{action=Auth}");
            }); 
        }
    }
}
