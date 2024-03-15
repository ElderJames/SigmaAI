using AntDesign.ProLayout;
using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.plugins.Functions;
using AntSK.Services;
using AntSK.Services.Auth;
using AntSK.Services.LLamaSharp;
using Coravel;
using LLama.Native;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sigma.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(config =>
{
    //此设定解决JsonResult中文被编码的问题
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

    config.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    config.JsonSerializerOptions.Converters.Add(new DateTimeNullableConvert());
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AntSKAuthProvider>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(sp.GetService<NavigationManager>()!.BaseUri)
});
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton(sp => new FunctionService(sp, [typeof(AntSK.App).Assembly, typeof(AntSK.Domain.Common.AntSkFunctionAttribute).Assembly]));
builder.Services.AddScoped<FunctionTest>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IImportKMSService, ImportKMSService>();
builder.Services.AddScoped<IKernelService, KernelService>();
builder.Services.AddScoped<IKMService, KMService>();
builder.Services.AddScoped<IApis_Repositories, Apis_Repositories>();
builder.Services.AddScoped<IApps_Repositories, Apps_Repositories>();
builder.Services.AddScoped<IKmss_Repositories, Kmss_Repositories>();
builder.Services.AddScoped<IKmsDetails_Repositories, KmsDetails_Repositories>();
builder.Services.AddScoped<IAIModels_Repositories, AIModels_Repositories>();
builder.Services.AddScoped<IUsers_Repositories, Users_Repositories>();
builder.Services.AddSingleton<ILLamaChatService, LLamaChatService>();
builder.Services.AddSingleton<ILLamaEmbeddingService, LLamaEmbeddingService>();
builder.Services.AddScoped<ILLamaSharpService, LLamaSharpService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddQueue();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AntSK.Api", Version = "v1" });
    //添加Api层注释（true表示显示控制器注释）
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    //添加Domain层注释（true表示显示控制器注释）
    var xmlFile1 = $"{Assembly.GetExecutingAssembly().GetName().Name.Replace("Api", "Core")}.xml";
    var xmlPath1 = Path.Combine(AppContext.BaseDirectory, xmlFile1);
    c.IncludeXmlComments(xmlPath1, true);
    c.DocInclusionPredicate((docName, apiDes) =>
    {
        if (!apiDes.TryGetMethodInfo(out MethodInfo method))
            return false;
        var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
        if (docName == "v1" && !version.Any())
            return true;
        var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
        if (actionVersion.Any())
            return actionVersion.Any(v => v == docName);
        return version.Any(v => v == docName);
    });
});
//Mapper

//后台队列任务
//builder.Services.AddBackgroundTaskBroker().AddHandler<ImportKMSTaskReq, BackGroundTaskHandler>("ImportKMSTask");
// 读取连接字符串配置
{
    builder.Configuration.GetSection("DBConnection").Get<DBConnectionOption>();
    builder.Configuration.GetSection("Login").Get<LoginOption>();
    builder.Configuration.GetSection("LLamaSharp").Get<LLamaSharpOption>();
    if (LLamaSharpOption.RunType.ToUpper() == "CPU")
    {
        NativeLibraryConfig
           .Instance
           .WithCuda(false)
           .WithLogs(true);
    }
    else if (LLamaSharpOption.RunType.ToUpper() == "GPU")
    {
        NativeLibraryConfig
        .Instance
        .WithCuda(true)
        .WithLogs(true);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

InitDB(app);

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseSwagger();
//配置Swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntSK API"); //注意中间段v1要和上面SwaggerDoc定义的名字保持一致
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
void InitDB(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        //codefirst 创建表
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
        //_repository.GetDB().DbMaintenance.CreateDatabase();
        //_repository.GetDB().CodeFirst.InitTables(typeof(Apps));
        //_repository.GetDB().CodeFirst.InitTables(typeof(Kmss));
        //_repository.GetDB().CodeFirst.InitTables(typeof(KmsDetails));
        //_repository.GetDB().CodeFirst.InitTables(typeof(Users));
        //_repository.GetDB().CodeFirst.InitTables(typeof(Apis));
        //_repository.GetDB().CodeFirst.InitTables(typeof(AIModels));
        ////创建vector插件如果数据库没有则需要提供支持向量的数据库
        //_repository.GetDB().Ado.ExecuteCommandAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
    }
}