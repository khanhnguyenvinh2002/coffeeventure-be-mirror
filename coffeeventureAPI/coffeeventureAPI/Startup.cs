//using coffeeventureAPI.Core.Base;
using coffeeventureAPI.Data;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Review;
using coffeeventureAPI.Service.Review;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using coffeeventureAPI.Service.Image;
using coffeeventureAPI.Repository.Image;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using coffeeventureAPI.Core.API;
using Microsoft.AspNetCore.Identity;
using IdentityModel;
using coffeeventureAPI.Model;
using Azure.Storage.Blobs;
using coffeeventureAPI.Service.Blob;
using Microsoft.Extensions.Azure;
using System.Net.Http;

namespace coffeeventureAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
            //Add dependency injection for repositories and services
            try
            {
                services.Scan(scan => scan
                    .FromApplicationDependencies()
                        .AddClasses(classes => classes.AssignableTo<IScoped>())
                            .AsImplementedInterfaces()
                            .WithScopedLifetime());
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
            }
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            //// ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default Claims
                                                                        // get options
            var jwtKey = Configuration["JwtIssuerOptions:JwtKey"];
            var jwtIssuer = Configuration["JwtIssuerOptions:JwtIssuer"];
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
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ValidateLifetime = true,
                        //ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            
            // config claims for jwt
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });
            
            // add dependency injections
            services.AddScoped(x => new BlobServiceClient(Configuration.GetValue<string>("AzureBlobStorage")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IImageRepository, ImageRepository>();
            
            
            // add db context
            services.AddDbContext<CVDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            
            // add singleton dependency injection
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // config cors policy
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {

                builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
                builder.WithOrigins("https://coffee-venture.web.app").AllowAnyMethod().AllowAnyHeader();
                builder.WithOrigins("https://coffee-venture.firebaseapp.com").AllowAnyMethod().AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // config app at runtime
            // authenticate
            app.UseAuthentication();
            
            // cors
            app.UseCors("ApiCorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // config static files
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            // config middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
   
}
