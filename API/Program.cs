using API.Data;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt => 
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://127.0.0.1:3000","http://localhost:3000");
});

app.UseAuthorization();

app.MapControllers();

//創建資料庫
var scope = app.Services.CreateScope();
//獲得StoreContext
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
//紀錄錯誤
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    /* Applies any pending migrations for the context to the database. 
        Will create the database if it does not already exist. */
    context.Database.Migrate();
    DbInitializer.Initialize(context);
}
//有error時
catch (Exception ex)
{
    logger.LogError(ex,"A problem occurred during migration.");
}

app.Run();
