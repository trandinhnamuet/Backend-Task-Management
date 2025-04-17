using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200") // Cho phép Angular
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HRTaskDatabase")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("http://localhost:5283/swagger\nhttp://localhost:5283/swagger");

var app = builder.Build();
// Sử dụng CORS
app.UseCors("AllowAngular");

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
