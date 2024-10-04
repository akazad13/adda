using System.Net;
using EasyConnect.API;
using EasyConnect.API.Data;
using EasyConnect.API.Helpers;
using EasyConnect.API.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                context.Response.AddApplicationError(error.Error.Message);
                await context.Response.WriteAsync(error.Error.Message);
            }
        });
    });
}

app.UseCors("_myAllowSpecificOrigins");

//  app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("hubs/chat");

// Initialise and seed database
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<Seed>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

await app.RunAsync();
