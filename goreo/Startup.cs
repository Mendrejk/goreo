using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Westwind.AspNetCore.LiveReload;

namespace goreo
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
            services.AddRazorPages();

            services.AddDbContext<postgresContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("goreoDB"))
            );

            services.AddLiveReload();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/Users/Login";
                        options.AccessDeniedPath = "/Users/Login";
                    }
                );

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Goreo API", Version = "v1.0" });
                }
            );

            services.AddSession();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeAdmin",
                    policy => policy.RequireClaim(ClaimTypes.Role, User.Roles.Administrator));
                options.AddPolicy("MustBeUserOrLeader",
                    policy => policy.RequireClaim(ClaimTypes.Role, User.Roles.User, User.Roles.Leader));
                options.AddPolicy("MustBeLeader",
                    policy => policy.RequireClaim(ClaimTypes.Role, User.Roles.Leader));
            });

            // route to the login page by default
            services.AddMvc().AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Users/Login", "");
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseLiveReload();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}