using System.Net;
using Adda.API;
using Adda.API.Data;
using Adda.API.Helpers;
using Adda.API.Hubs;
using Microsoft.AspNetCore.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();

    _ = app.UseExceptionHandler(static builder =>
    {
        builder.Run(static async context =>
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

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("hubs/chat");

// Initialise and seed database
using (IServiceScope scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<Seed>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

await app.RunAsync();
