using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LostFamily
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
            services.AddMvc();
            services.AddProgressiveWebApp();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IMobileHelper, MobileHelper>();
            services.Configure<VapidSettings>(Configuration.GetSection("VAPID"));
            services.AddSingleton<IVapidSettings, VapidSettings>();
            services.AddSingleton<IPushSubscription, PushNotification>();

            //services.AddSingleton<IPushNotificationService>();
            //services.AddSingleton<PushNotificationService>();
            services.AddTransient<IPushNotificationService, PushNotificationService>();

            //services.AddSingleton<PushNotificationService>(serviceProvider => new PushNotificationService(serviceProvider.GetService<VapidSettings>(), serviceProvider.GetService<PushNotification>()));

            //services.AddTransient<IPushNotificationService, PushNotificationService>(serviceProvider => new PushNotificationService(serviceProvider.GetRequiredService<IVapidSettings>(), serviceProvider.GetRequiredService<IPushNotification>());
            //services.AddSingleton<IPushNotificationService, PushNotificationService>(serviceProvider => new PushNotificationService(serviceProvider.GetService<VapidSettings>(), serviceProvider.GetService<IPushNotification>()));
            //services.AddSingleton<IPushNotificationService, PushNotificationService>(serviceProvider => new PushNotificationService( new VapidSettings(), new PushNotification()));
            //services.AddSingleton<IPushNotificationService>(serviceProvider => 
            //{
            //    return new PushNotificationService(serviceProvider.GetService<VapidSettings>(), serviceProvider.GetService<IPushNotification>());
            //});
            //services.AddSingleton<IPushNotificationService>(serviceProvider => services.AddSingleton<PushNotificationService>());
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
