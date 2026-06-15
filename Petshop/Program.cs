using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Petshop.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<Petshop.Repositories.Interfaces.IClienteRepository, Petshop.Repositories.ClienteRepository>();
builder.Services.AddScoped<Petshop.Repositories.Interfaces.IPetRepository, Petshop.Repositories.PetRepository>();
builder.Services.AddScoped<Petshop.Services.Interfaces.IClienteService, Petshop.Services.ClienteService>();
builder.Services.AddScoped<Petshop.Services.Interfaces.IPetService, Petshop.Services.PetService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Petshop.Data.AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
