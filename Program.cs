using System.Threading.RateLimiting;
using EbayChat.Entities;
using EbayChat.Events;
using EbayChat.Events.Handlers;
using EbayChat.Hubs;
using EbayChat.Services.ServicesImpl;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace EbayChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var redis = ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!);

            // Persist Data Protection keys to /keys (mounted volume)
            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                .SetApplicationName("EbayChatApp");

            // Add DbContext
            builder.Services.AddDbContext<CloneEbayDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, LogLevel.Information));

            // Dependency injection for services
            builder.Services.AddScoped<Services.IUserServices, UserServices>();
            builder.Services.AddScoped<Services.IChatServices, ChatServices>();
            builder.Services.AddScoped<Services.ICategoryService, CategoryService>();
            builder.Services.AddScoped<Services.IProductService, ProductService>();

            // Redis multiplexer singleton (thread-safe by design)
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                return redis;
            });

            builder.Services.AddScoped<Services.IRedisCacheService, RedisCacheService>();

            builder.Services.AddHttpClient(); // for HttpClientFactory

            // Add view engines
            builder.Services.AddControllersWithViews();

            // Add Rate Limiting (Partitioned by User/IP)
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var clientIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        clientIp,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            AutoReplenishment = true,
                            QueueLimit = 0
                        });
                });

                options.RejectionStatusCode = 429;
            });

            // Always add SignalR with Redis backplane for real-time features
            var signalR = builder.Services.AddSignalR();

            signalR.AddStackExchangeRedis(builder.Configuration["Redis:ConnectionString"]!);

            // Add distributed SQL Server cache for sessions
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:ConnectionString"];
                options.InstanceName = "EbayChatSession:";
            });

            // Add session support
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".EbayChat.Session";       // same for all containers
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // if using HTTPS
            });

            builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

            builder.Services.AddScoped<IEventHandler<DisputeCreatedEvent>, DisputeCreatedEventHandler>();
            builder.Services.AddScoped<IEventHandler<ReturnRequestedEvent>, ReturnRequestedEventHandler>();
            builder.Services.AddScoped<IEventHandler<LowRatingDetectedEvent>, LowRatingDetectedEventHandler>();
            builder.Services.AddScoped<IEventHandler<FeedbackSubmittedEvent>, FeedbackSubmittedEventHandler>();
            builder.Services.AddScoped<IEventHandler<SellerReportedEvent>, SellerReportedEventHandler>();
            builder.Services.AddScoped<IAdminEventNotifier, SignalRAdminEventNotifier>();

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            // Add Rate Limiter middleware right after authorization (or before routing depending on needs)
            // Placing it here protects endpoints while allowing static files to load fast
            app.UseRateLimiter();

            app.MapStaticAssets();
            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<EbayChatHub>("/ebayChatHub");
            app.MapHub<AdminNotificationHub>("/adminNotificationHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
