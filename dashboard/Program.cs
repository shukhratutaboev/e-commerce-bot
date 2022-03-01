using dashboard.Entities;
using dashboard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DashboardDbContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DbConnection"));
}, ServiceLifetime.Singleton);
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IService<Category>, CategoryService>();
builder.Services.AddTransient<IService<Item>, ItemService>();
builder.Services.AddCors(options
    => options.AddDefaultPolicy(
        builder => 
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();   
        }
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
