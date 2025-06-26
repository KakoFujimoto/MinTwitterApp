using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Data;
using MinTwitterApp.Services;
using MinTwitterApp.Common;
using Microsoft.Extensions.WebEncoders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<MinTwitterApp.Filters.GlobalExceptionFilter>();
});

// builder.Services.Configure<WebEncoderOptions>(options =>
// {
//     options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(
//     System.Text.Unicode.UnicodeRanges.All);
// });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SessionService, SessionService>();
builder.Services.AddScoped<UserErrorService>();
builder.Services.AddScoped<CreatePostService>();
builder.Services.AddScoped<ViewPostService>();
builder.Services.AddScoped<PostErrorService>();
builder.Services.AddScoped<IPostErrorMessages, PostErrorMessages>();
builder.Services.AddScoped<DeletePostService>();
builder.Services.AddScoped<EditPostService>();
builder.Services.AddSingleton<IDateTimeAccessor, DateTimeAccessor>();
builder.Services.AddScoped<LikePostService>();
builder.Services.AddScoped<RePostService>();
builder.Services.AddScoped<ReplyPostService>();
builder.Services.AddScoped<LoginUser>();
builder.Services.AddScoped<ImageFormatDetector>();



builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });

builder.Services.AddAuthorization();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();
