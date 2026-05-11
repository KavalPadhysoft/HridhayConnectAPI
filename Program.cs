
using HridhayConnect_API.Infra;
using HridhayConnect_API.ServiceRepository.CategoryRepository;
using HridhayConnect_API.ServiceRepository.ChangePasswordRepository;
using HridhayConnect_API.ServiceRepository.CustomersRepository;
using HridhayConnect_API.ServiceRepository.DashboardRepository;
using HridhayConnect_API.ServiceRepository.DeliveriesRepository;
using HridhayConnect_API.ServiceRepository.ItemRepository;
using HridhayConnect_API.ServiceRepository.LovRepository;
using HridhayConnect_API.ServiceRepository.MenuAccessRepository;
using HridhayConnect_API.ServiceRepository.MenuRepository;
using HridhayConnect_API.ServiceRepository.OrderRepository;
using HridhayConnect_API.ServiceRepository.PaymentCollectionRepository;
using HridhayConnect_API.ServiceRepository.RoleRepository;
using HridhayConnect_API.ServiceRepository.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//for CROS_Po
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               //  builder.WithOrigins("http://localhost:3001")
               .AllowAnyMethod()
             .AllowAnyHeader();
    });
});
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero //For Removie extra 5 Minutes
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HridhayConnectAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: Bearer abcdef12345"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuAccessRepo, MenuAccessRepo>();
builder.Services.AddScoped<IChangePasswordRepository, ChangePasswordRepository>();
builder.Services.AddScoped<ILovRepository, LovRepository>();
builder.Services.AddScoped<ICustomersRepository, CustomersRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDeliveriesRepository, DeliveriesRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IPaymentCollectionRepository, PaymentCollectionRepository>();
builder.Services.AddScoped<ValidationService>();




builder.Services.AddSwaggerGen();

var app = builder.Build();

AppHttpContextAccessor.Configure(
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Services.GetRequiredService<IHostEnvironment>(),
    app.Services.GetRequiredService<IWebHostEnvironment>(),
    app.Services.GetRequiredService<IDataProtectionProvider>(),
    app.Configuration,
    app.Services.GetRequiredService<IHttpClientFactory>()
);


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger(); // for live index
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommmonProject v1");
    c.RoutePrefix = string.Empty; // ?? IMPORTANT
});


app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
//app.UseAuthorization();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("OK")); // new add simple liveness probe
app.MapControllers();

app.Run();
