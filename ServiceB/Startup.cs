using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceB
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServiceB", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceB v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                
                string headerKey = "X-API-KEY";
                string headerApiValue = "123";
                
                if (!context.Request.Headers.ContainsKey(headerKey))
                {
                    // Call the next delegate/middleware in the pipeline
                    context.Response.StatusCode = 400;
                    await next();
                }
                else
                {
                    IEnumerable<string> headerValues;
                    var apiValue = string.Empty;
                    if(context.Request.Headers.TryGetValue(headerKey, out var apiValues))
                    {
                        apiValue = apiValues.FirstOrDefault();
                        if (!apiValue.Equals(headerApiValue))
                        {
                            context.Response.StatusCode = 400;
                            await next();
                        }
                        else
                        {
                            context.Response.StatusCode = 200;
                        }
                    }
                }
                
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("API key is not valid.");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
