using LocalChachaAdminApi.AutoMapperConfiguration;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Services;
using LocalChachaAdminApi.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using LocalChachaAdminApi.Core.Repositories;
using RestSharp;
using Microsoft.AspNetCore.Http;

namespace LocalChachaAdminApi
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
            services.AddControllers();

            services.Configure<KestrelServerOptions>(
            Configuration.GetSection("Kestrel"));

            //services
            services.AddTransient<IBulkInsertService, BulkInsertService>();
            services.AddTransient<IS3BucketService, S3BucketService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<IMerchantRepository, MerchantRepository>();
            services.AddTransient<IQuickBloxService, QuickBloxService>();
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddHttpClient("localChacha", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("localChachaUrl"));
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<ApiAuthorizationFilter>();
            });

            services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);

            //logging
            services.AddLogging();

            //for removing error possible object cycle was detected which is not supported
            services.AddControllers().AddNewtonsoftJson(options =>
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "LocalChachaAdmin API",
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocalChacha Admin");
            });

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