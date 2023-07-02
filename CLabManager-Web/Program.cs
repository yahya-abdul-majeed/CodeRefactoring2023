using CLabManager_Web.Repos;
using ModelsLibrary.Utilities;
using NToastNotify;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISD, SD>();
builder.Services.AddScoped<ILabRepo, LabRepo>();
builder.Services.AddScoped<IComputerRepo, ComputerRepo>();
builder.Services.AddScoped<IIssuesRepo,IssuesRepo>();
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = false,
    PositionClass = ToastPositions.TopRight
}); 

//builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseNToastNotify();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area=User}/{controller=Labs}/{action=Index}/{id?}"
);


app.Run();
