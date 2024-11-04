using System.Text;
using KhumaloCraft.Web;
using KhumaloCraft.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container
builder.Services.AddRazorPages();

// Add HttpClient for API calls
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtAuthorizationHandler>();

var apiHost = Environment.GetEnvironmentVariable("API_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("ApiHost");

builder.Services.AddHttpClient("BusinessAPI", client =>
{
  client.BaseAddress = new Uri(apiHost);
}).AddHttpMessageHandler<JwtAuthorizationHandler>();

// Configure JWT Bearer authentication
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
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    // Read the JWT from the cookies
    options.Events = new JwtBearerEvents
    {
      OnMessageReceived = context =>
      {
        var token = context.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
          context.Token = token;
        }
        return Task.CompletedTask;
      },
      OnChallenge = context =>
      {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
          context.Response.Redirect("/Auth/Login");
        }
        return Task.CompletedTask;
      },
      OnForbidden = context =>
      {
        context.Response.Redirect("/Auth/AccessDenied");
        return Task.CompletedTask;
      }
    };
  });

// Authorization policy
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Apply Admin-only authorization globally to the /Admin folder
builder.Services.AddRazorPages(options =>
{
  options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");
app.MapRazorPages();

app.Run();
