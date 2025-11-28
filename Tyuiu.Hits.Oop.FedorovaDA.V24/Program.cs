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

            // -------------------------------------

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Упростим настройки для локальной разработки
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // --- ИСПРАВЛЕНИЕ ОШИБКИ С EMAIL SENDER ---
            // Вместо глючного IdentityNoOpEmailSender используем свою простую заглушку
            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, DummyEmailSender>();

            // --- НЕ ЗАБУДЬ ПОДКЛЮЧИТЬ СЕРВИС НОВОСТЕЙ ---
            builder.Services.AddSingleton<NewsBackgroundService>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<NewsBackgroundService>());


            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

            // --- АВТОМАТИЧЕСКАЯ ИНИЦИАЛИЗАЦИЯ БАЗЫ (ОПЦИОНАЛЬНО) ---
            // Чтобы при первом запуске база создалась сама без Add-Migration
            /*
            using (var scope = app.Services.CreateScope())
            {
                 var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                 db.Database.EnsureCreated();
            }
            */

            app.Run();
        }
    }

    // --- КЛАСС-ЗАГЛУШКА ДЛЯ EMAIL (Чтобы не было ошибок типов) ---
    public class DummyEmailSender : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Task.CompletedTask;
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Task.CompletedTask;
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Task.CompletedTask;
    }
}