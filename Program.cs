using Microsoft.EntityFrameworkCore;
using API.Data;  // Ensure you're using the correct namespace for DataContext

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add other services, e.g., controllers, here
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline (e.g., middleware, routing)
app.MapControllers();

app.Run();
