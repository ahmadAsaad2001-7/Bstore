
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using StoreWebapi.Application;
    using StoreWebapi.Domain.Domain;
    using StoreWebapi.Domain.Interfaces;
    using StoreWebapi.Infrastructure;
    using StoreWebapi.Infrastructure.Data;
    using StoreWebapi.Infrastructure.Shared;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddIdentity<user, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 12;
            
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    builder.Services.AddOpenApi();
    builder.Services.AddControllers(); 
    
    builder.Services.AddApplication();
    builder.Services.AddInfraStructure( builder.Configuration);




    var app = builder.Build();


    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();


    app.MapControllers(); 

    app.Run();