using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Structr.Samples.Navigation.Infrastructure;
using Structr.Samples.Navigation.Resources;
using System.IO;

namespace Structr.Samples.Navigation
{
    public class Startup
    {
        public IWebHostEnvironment Env { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            Env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            var mvcBuilder = services.AddControllersWithViews();
#if DEBUG
            if (Env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif

            var rootPath = Env.ContentRootPath;

            services.AddSingleton<IMenuActivator, MenuActivator>();
            services.AddSingleton<IBreadcrumbActivator, BreadcrumbActivator>();

            services.AddNavigation()
                .AddXml<MenuItem>(Path.Combine(rootPath, "menu.xml"), (serviceProvider, options) =>
                {
                    options.ResourceType = typeof(MenuResource);
                    options.ItemActivator =
                        item => serviceProvider.GetService<IMenuActivator>().Activate(item);
                })
                .AddJson<Breadcrumb>(Path.Combine(rootPath, "breadcrumbs.json"), (serviceProvider, options) =>
                {
                    options.ItemActivator =
                        breadcrumb => serviceProvider.GetService<IBreadcrumbActivator>().Activate(breadcrumb);
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
