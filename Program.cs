
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsPage.data;
using NewsPage.helpers;
using NewsPage.repositories;
using NewsPage.repositories.interfaces;
using NewsPage.Repositories;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace NewsPage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var isDevelopment = builder.Environment.IsDevelopment();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NewsPage API", Version = "v1" });

                // 🔹 Cấu hình Swagger để hỗ trợ Authorization bằng JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập token theo định dạng: Bearer {your_token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

            //// connect to Redis // xử lý mã otp 
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
               .Connect(builder.Configuration["Redis:ConnectionString"]));


            // 🔹 Lấy thông tin từ appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !isDevelopment,
                        ValidateAudience = !isDevelopment,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            builder.Services.AddAuthorization();

            // Add services to the container.
            //Auth service
            builder.Services.AddScoped<IUserAccountRepository, UserAccountsRepository>();
            builder.Services.AddScoped<IUserDetailRepository, UserDetailRepository>();
            builder.Services.AddScoped<ITopicRepository, TopicRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IArticleVisitRepository, ArticleVisitRepository>();
            //JWT token
            builder.Services.AddScoped<JwtHelper>();
            //crypt password
            builder.Services.AddTransient<PasswordHelper>();
            //uploads file
            builder.Services.AddScoped<FileHelper>();

            //Send email
            builder.Services.AddSingleton<MailHelper>();

            //Generate OTP
            builder.Services.AddSingleton<OtpHelper>();

            // Convert enum string
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // setup cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            
            // config logging 
            ConfigureLogging ();
            builder.Host.UseSerilog();



            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            //Xử lý lỗi 401 
            app.Use(async (context, next) =>
            {
                await next();

                // Kiểm tra nếu bị 401 thì trả về JSON thay vì trang HTML mặc định
                if (context.Response.StatusCode == 401)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\": \"Bạn chưa đăng nhập hoặc không có quyền truy cập\"}");
                }
            });

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.UseCors("AllowAll");

            app.Run();
        }
        static void ConfigureLogging (){
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{environment}.json", optional: true
                ).Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigurationElasticSink(configuration, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        static  ElasticsearchSinkOptions ConfigurationElasticSink(IConfigurationRoot configuration, string? environment)
        {
            environment ??= "Development";
            var uriString = configuration["ElasticConfiguration:Uri"] ?? "http://localhost:9200";
            var indexName = Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-") ?? "app";
            
            return new ElasticsearchSinkOptions(new Uri(uriString))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{indexName}-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                NumberOfReplicas = 1,
                NumberOfShards = 2
            };
        }
    }
}
