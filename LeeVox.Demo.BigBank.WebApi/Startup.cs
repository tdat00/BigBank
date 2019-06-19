using System.Linq;
using System.Text;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.WebApi.Middleware;
using LeeVox.Sdk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LeeVox.Demo.BigBank.WebApi
{
    public class Startup
    {
        public ILogger<Startup> Logger {get; set;}

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            new DependencyResolver().AddDependencies(services);

            services.AddMvcCore()
                .AddCors()
                .AddAuthorization()
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();

            // added current login user to DI.
            services.AddScoped<CurrentLoginInfo>(provider =>
            {
                var httpContext = provider.GetService<IHttpContextAccessor>();

                var authInfo = httpContext.HttpContext.AuthenticateAsync().WaitAndReturn();
                var userId = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "id")?.Value;
                var session = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "session")?.Value;
                var first_name = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "first_name")?.Value;
                var last_name = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "last_name")?.Value;
                var email = authInfo?.Principal?.Claims?.FirstOrDefault(x => x.Type == "email")?.Value;
                return new CurrentLoginInfo {
                    Session = session,
                    User = new User
                    {
                        Id = userId.ParseToInt(-1),
                        FirstName = first_name,
                        LastName = last_name,
                        Email = email
                    }
                };
            });

            // parse configuration from appSettings.json
            services.Configure<JwtConfig>(Configuration.GetSection("jwtConfig"));
            var jwtConfig = Configuration.GetSection("jwtConfig").Get<JwtConfig>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            //TODO: should implement OAuth2 standard instead of InMemory session.
            app.UseMiddleware<SessionJwtMiddleware>();
            app.UseMvc();
        }
    }
}
