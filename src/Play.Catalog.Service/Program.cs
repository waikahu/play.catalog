using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Play.Catalog.Service;
using Play.Catalog.Service.Entities;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo()
    .AddMongoRepository<Item>("items")
    .AddMassTransitWithMessageBroker(builder.Configuration)
    .AddJwtBearerAuthentication();

builder.Services.AddAuthorization(opt => {
    opt.AddPolicy(Policies.Read, policy => 
    {
        policy.RequireRole("Admin");
        policy.RequireClaim("scope", "catalog.readaccess", "catalog.fullaccess");
    });

    opt.AddPolicy(Policies.Write, policy => 
    {
        policy.RequireRole("Admin");
        policy.RequireClaim("scope", "catalog.writeaccess", "catalog.fullaccess");
    });
});

builder.Services.AddControllers(opt => 
{
    opt.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(opt => {
        opt.WithOrigins(app.Configuration["AllowedOrigin"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
