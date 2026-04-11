
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Identity;
  
    using StoreWebapi.Application;
    using StoreWebapi.Domain.Domain;
    
    using StoreWebapi.Infrastructure;
    using StoreWebapi.Infrastructure.Data;
   

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddIdentity<user, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 12;
            
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/api/auth/logout";
            options.AccessDeniedPath = "/api/auth/access-denied";
        }
        );
    builder.Services.AddOpenApi();
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }); 
    
    builder.Services.AddApplication();
    builder.Services.AddInfraStructure( builder.Configuration);




    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication(); 
    app.UseAuthorization();

    app.MapControllers(); 

    app.Run();