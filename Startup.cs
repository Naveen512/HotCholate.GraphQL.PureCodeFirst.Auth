using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.PureCodeFirst.Auth.Data;
using GraphQL.PureCodeFirst.Auth.Logics;
using GraphQL.PureCodeFirst.Auth.Resolvers;
using GraphQL.PureCodeFirst.Auth.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GraphQL.PureCodeFirst.Auth
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GraphQL.PureCodeFirst.Auth", Version = "v1" });
            });
            services.AddGraphQLServer()
            .AddQueryType<QueryResolver>()
            .AddMutationType<MutationResolver>()
            .AddAuthorization();

            services.AddDbContext<AuthContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AuthContext"));
            });

            services.AddScoped<IAuthLogic, AuthLogic>();
            services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenSettings = Configuration
                .GetSection("TokenSettings").Get<TokenSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenSettings.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = tokenSettings.Audience,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                    ValidateIssuerSigningKey = true,
                    //ClockSkew = TimeSpan.Zero // enable this line to validate the expiration time below 5mins
                };
            });

            services.AddAuthorization(options => {
                options.AddPolicy("roles-policy", policy => {
                    policy.RequireRole(new string[]{"admin","super-admin"});
                });
                options.AddPolicy("claim-policy-1", policy => {
                    policy.RequireClaim("LastName");
                });
                options.AddPolicy("claim-policy-2", policy=>{
                    policy.RequireClaim("LastName",new string[]{"Bommidi","Test"});
                });
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GraphQL.PureCodeFirst.Auth v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
            });
        }
    }
}
