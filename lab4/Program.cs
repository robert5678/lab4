using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/Library", async context =>
{
    await context.Response.WriteAsync("Welcome to the Library!");
});


string booksFilePath = "books.json";
app.MapGet("/Library/Books", async context =>
{
    if (File.Exists(booksFilePath))
    {
        var booksContent = File.ReadAllText(booksFilePath);
        var booksList = JsonSerializer.Deserialize<string[]>(booksContent);
        await context.Response.WriteAsync(string.Join("\n", booksList));
    }
    else
    {
        await context.Response.WriteAsync("No books found.");
    }
});


string profilesFilePath = "profiles.json";
app.MapGet("/Library/Profile/{id?}", async context =>
{
    string idValue = context.Request.RouteValues["id"]?.ToString();

    if (int.TryParse(idValue, out int id) && id >= 0 && id <= 5)
    {
        if (File.Exists(profilesFilePath))
        {
            var profilesContent = File.ReadAllText(profilesFilePath);
            var profiles = JsonDocument.Parse(profilesContent);

            
            if (profiles.RootElement.TryGetProperty(id.ToString(), out var userProfile))
            {
                await context.Response.WriteAsync(userProfile.GetString());
            }
            else
            {
                await context.Response.WriteAsync("User not found.");
            }
        }
        else
        {
            await context.Response.WriteAsync("No profiles found.");
        }
    }
    else if (idValue == null)
    {
       
        await context.Response.WriteAsync("Current User: Default User, Age 28");
    }
    else
    {
        await context.Response.WriteAsync("Invalid user ID. Please enter a number between 0 and 5.");
    }
});

app.Run();
