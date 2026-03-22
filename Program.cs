using System.Threading.RateLimiting;
using EbayChat.Entities;
using EbayChat.Events;
using EbayChat.Events.Handlers;
using EbayChat.Hubs;
using EbayChat.Services.ServicesImpl;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace EbayChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Persist Data Protection keys to /keys (mounted volume)
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
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
                var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });

            builder.Services.AddScoped<Services.IRedisCacheService, RedisCacheService>();

            builder.Services.AddHttpClient(); // for HttpClientFactory

            // Add view engines
            builder.Services.AddControllersWithViews();

            // Add Rate Limiting (Partitioned by User/IP)
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("GlobalLimiter", httpContext =>
                {
                    // Lấy IP của người dùng làm khóa phân loại (Partition Key)
                    // Nếu IP null, dùng chuỗi "anonymous"
                    var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: clientIp,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // Số request tối đa cho MỖI IP
                            Window = TimeSpan.FromMinutes(1), // Trong khoảng thời gian (1 phút)
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0 // Báo lỗi luôn nếu vượt quá, không chờ
                        });
                });

                // Trả về lỗi 429 Too Many Requests khi vượt quá giới hạn
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Optional: Customize the response body when rejected
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "text/plain";
                    await context.HttpContext.Response.WriteAsync("Too many request, please try again in 1 minute", cancellationToken: token);
                };
            });

            // Always add SignalR
            var signalR = builder.Services.AddSignalR();

            // Only use Redis backplane in production
            if (builder.Environment.IsProduction())
            {
                signalR.AddStackExchangeRedis("redis:6379", options =>
                {
                    options.Configuration.ChannelPrefix = "EbayChat";
                });
            }

            // Add distributed SQL Server cache for sessions
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "SessionCache";
            });

            // Add session support
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".EbayChat.Session";       // same for all containers
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // if using HTTPS
            });

            builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

            builder.Services.AddScoped<IEventHandler<DisputeCreatedEvent>, DisputeCreatedEventHandler>();
            builder.Services.AddScoped<IEventHandler<ReturnRequestedEvent>, ReturnRequestedEventHandler>();
            builder.Services.AddScoped<IEventHandler<LowRatingDetectedEvent>, LowRatingDetectedEventHandler>();
            builder.Services.AddScoped<IEventHandler<FeedbackSubmittedEvent>, FeedbackSubmittedEventHandler>();
            builder.Services.AddScoped<IEventHandler<SellerReportedEvent>, SellerReportedEventHandler>();
            builder.Services.AddScoped<IAdminEventNotifier, SignalRAdminEventNotifier>();

            var app = builder.Build();



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
                .WithStaticAssets()
                .RequireRateLimiting("GlobalLimiter");

            app.Run();
        }
    }
}
