using Botsboxadminbot.FileProcessModule;
using Botsboxadminbot.Models;
using Botsboxadminbot.Models.Abstractions;
using Botsboxadminbot.Models.Entities;
using Botsboxadminbot.Models.Repositories;
using Botsboxadminbot.ProcessModule;
using Botsboxadminbot.ProcessModule.Abstractions;
using Botsboxadminbot.SendAdminModule;
using Botsboxadminbot.Services;
using Botsboxadminbot.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsboxadminbot
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
            var connection = Configuration.GetConnectionString("Main");

            services.AddScoped<IDbRepository<BranchEntity>, BranchRepository>();
            services.AddScoped<IDbRepository<BotsBoxMessageEntity>, MessageRepository>();
            services.AddScoped<IDbService<BotsBoxMessageEntity>, BotMessageService>();
            services.AddScoped<IDbService<BranchEntity>, BranchService>();
            services.AddScoped<IMainProcessor, MainProcessor>();
            services.AddScoped<IFileProcessService, FileProcessService>();
            services.AddScoped<ISendAdminamService, SendAdminamService>();

            services.AddDbContext<BotContext>(options =>
            {
                options.UseNpgsql(connection);
            });
            services.AddControllers().AddNewtonsoftJson(); 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Botsboxadminbot", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<BotContext>())
                {
                    context.Database.Migrate();
                }
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Botsboxadminbot v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            Bot.SetBotClientAsync().Wait();
        }
    }
}