using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Server.Commands;
using System.Text.Json;

namespace RedMessagingDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<CreateTokenAsSvgCommand>();
            builder.Services.AddScoped<EnrolDocumentCommand>();
            builder.Services.AddScoped<FindReceiverDocumentsCommand>();
            builder.Services.AddScoped<FindSingleMessageCommand>();
            builder.Services.AddScoped<FindSingleReceiverDocumentCommand>();
            builder.Services.AddScoped<FindUserFromBearerTokenCommand>();
            builder.Services.AddScoped<ListDocumentsByUserCommand>();
            builder.Services.AddScoped<ListMessagesByUserCommand>();
            builder.Services.AddScoped<SendMessageCommand>();
            
            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            builder.Services.AddApiAuthorization();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddControllers().AddJsonOptions(j =>
            {
                j.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddSwaggerGen(options => 
                options.AddSecurityDefinition("oauth2", 
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                }
                ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            app.Run();
        }
    }
}