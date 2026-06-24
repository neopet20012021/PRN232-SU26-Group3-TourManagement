using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using TourManagement.Business.Mappings;
using TourManagement.Business.Services;
using TourManagement.Data.Context;
using TourManagement.Data.Models;
using TourManagement.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// OData Edm Model builder
static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Booking>("Bookings");
    builder.EntitySet<Tour>("Tours");
    builder.EntitySet<User>("Users");
    return builder.GetEdmModel();
}

builder.Services.AddControllers()
    .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// Register DbContext
builder.Services.AddDbContext<TourManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();

// Yêu cầu Singleton Pattern: Đổi PromoCodeService thành Singleton
builder.Services.AddSingleton<IPromoCodeService, PromoCodeService>();

// Register AutoMapper
builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
