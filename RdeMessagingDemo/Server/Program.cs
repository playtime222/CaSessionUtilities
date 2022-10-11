using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Server.Models;
using RdeMessagingDemo.Server.Commands;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;
using System.Reflection;

namespace RdeMessagingDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //Identity server
            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.AddApiAuthorization();

            //Website
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

            builder.Services.AddScoped<CreateTokenAsSvgCommand>();
            builder.Services.AddScoped<EnrolDocumentCommand>();
            builder.Services.AddScoped<FindReceiverDocumentsCommand>();
            builder.Services.AddScoped<FindSingleMessageCommand>();
            builder.Services.AddScoped<FindSingleReceiverDocumentCommand>();
            builder.Services.AddScoped<FindUserFromBearerTokenCommand>();
            builder.Services.AddScoped<ListDocumentsByUserCommand>();
            builder.Services.AddScoped<ListMessagesByUserCommand>();
            builder.Services.AddScoped<SendMessageCommand>();
            builder.Services.AddScoped<CreateDownloadAsSvgCommand>();

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

            //var provider = new FileExtensionContentTypeProvider();
            //provider.Mappings[".apk"] = "application/vnd.android.package-archive";
            ////provider.Mappings[".apk"] = "application/text";
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = "/downloads",
            //    ContentTypeProvider = provider,
            //    //ServeUnknownFileTypes = true,
            //});

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