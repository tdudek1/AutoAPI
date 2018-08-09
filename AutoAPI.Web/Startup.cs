using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Identity;

namespace AutoAPI.Web
{
	public class Startup
	{
		public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((x, y) => true, true) });

        public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddTransient<DbContext>(x =>
			{
                return new DataContext(new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: "Data").Options);
			});
			services.AddAutoAPI<DataContext>();


            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName:  "Identity");
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;
            });


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbContext context,UserManager<IdentityUser> userManager,IdentityContext identityContext)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
            app.UseAuthentication();

            context.Database.EnsureCreated();
            identityContext.Database.EnsureCreated();

            userManager.CreateAsync(new IdentityUser()
            {
                UserName = "test@test.com",
                Email = "test@test.com",

            },"Password1234!");
		}
	}
}
