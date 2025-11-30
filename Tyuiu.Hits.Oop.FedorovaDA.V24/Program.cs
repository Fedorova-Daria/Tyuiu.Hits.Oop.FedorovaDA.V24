using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tyuiu.Hits.Oop.FedorovaDA.V24.Components;
using Tyuiu.Hits.Oop.FedorovaDA.V24.Components.Account;
using NewsAggregator.Data;
using NewsAggregator.Services;

namespace Tyuiu.Hits.Oop.FedorovaDA.V24
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies();

            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=NewsAggregatorDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<ApplicationDbContext>(p =>
                p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, DummyEmailSender>();

            builder.Services.AddSingleton<NewsBackgroundService>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<NewsBackgroundService>());


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }

    public class DummyEmailSender : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Task.CompletedTask;
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Task.CompletedTask;
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Task.CompletedTask;
    }
}