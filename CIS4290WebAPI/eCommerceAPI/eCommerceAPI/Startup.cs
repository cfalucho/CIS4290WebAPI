using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using eCommerceAPI.Services;
using eCommerceAPI.Entities;
using eCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace eCommerceAPI
{
    public class Startup
    {

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var conn = Configuration["connectionStrings:sqlConnectionAPI"];

            //SqlDbContext is our connection to the DB using our connection string from secrets.json(conn)
            services.AddDbContext<SqlDbContext>(options =>
                options.UseSqlServer(conn));
            //Add Identity’s Database Context
            services.AddDbContextPool<ApplicationDbContext>(options =>
                            options.UseSqlServer(conn));

            //Implement Identity 
            services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
                           .AddEntityFrameworkStores<ApplicationDbContext>()
                           .AddDefaultTokenProviders();
            //Add JSON Web Token Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        //Recieve JWT config info from secrets.json
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            //Entity Framework allows interaction with DB, Generics allow data types to be assigned at runtime
            services.AddScoped(typeof(IGenericEFRepository), typeof(GenericEFRepository));

            //Using AutoMapper Package to Map Entities to DTOs AND vice versa
            //Entities represent tables in the DB
            //Data Transfer Object is used to turn entity data into a response object OR  convert request data into an entity model
            AutoMapper.Mapper.Initialize(config =>
            {
               
                config.CreateMap<Entities.Cart, Models.CartDTO>();
                config.CreateMap<Models.CartDTO, Entities.Cart>();
                config.CreateMap<Entities.Product, Models.ProductDTO>();
                config.CreateMap<Models.ProductDTO, Entities.Product>();
                config.CreateMap<Entities.Product, Models.ProductUpdateDTO>();
                config.CreateMap<Models.ProductUpdateDTO, Entities.Product>();
                config.CreateMap<Models.CartUpdateDTO, Entities.Cart>();
                config.CreateMap<Entities.Cart, Models.CartUpdateDTO>();
                config.CreateMap<Entities.Review, Models.ReviewDTO>();
                config.CreateMap<Models.ReviewDTO, Entities.Review>();
                config.CreateMap<Entities.Review, Models.ReviewUpdateDTO>();
                config.CreateMap<Models.ReviewUpdateDTO, Entities.Review>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
