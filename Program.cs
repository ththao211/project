using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWP_BE.Data;
using SWP_BE.Repositories;
using SWP_BE.Services;
using System.Text;

namespace SWP_BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===========================================================
            // 1. [MỚI] CẤU HÌNH CORS
            // ===========================================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ===== DB =====
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // ===== Controllers & Swagger =====
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "SWP-BE", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            builder.Services.AddScoped<ILabelRepository, LabelRepository>();
            builder.Services.AddScoped<ILabelService, LabelService>();

            builder.Services.AddScoped<IProjectLabelRepository, ProjectLabelRepository>();
            builder.Services.AddScoped<IProjectLabelService, ProjectLabelService>();

            builder.Services.AddScoped<ILabelingTaskRepository, LabelingTaskRepository>();
            builder.Services.AddScoped<ILabelingTaskService, LabelingTaskService>();

            // ===== JWT AUTH =====
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            var app = builder.Build();

            // ===========================================================
            // 2. [MỚI] TRẢ VỀ JSON LỖI (401, 404, 405) 
            // Giúp FE luôn nhận được Object thay vì trang HTML lỗi của trình duyệt
            // ===========================================================
            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                var responseObj = new
                {
                    message = "Yêu cầu không hợp lệ hoặc bạn không có quyền truy cập",
                    error = context.HttpContext.Response.StatusCode switch
                    {
                        401 => "Unauthorized",
                        403 => "Forbidden",
                        404 => "Not Found",
                        405 => "Method Not Allowed",
                        _ => "Error"
                    },
                    statusCode = context.HttpContext.Response.StatusCode
                };
                await context.HttpContext.Response.WriteAsJsonAsync(responseObj);
            });

            // ===== Swagger Config =====
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SWP-BE v1");
                options.RoutePrefix = string.Empty; 
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}