using ContactsSync.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication<ContactsSyncWebModule>();
// 若不添加autofac，无法在容器中获取仓储及dbcontext
builder.Host.UseAutofac();
var app = builder.Build();
app.InitializeApplication();
app.Run();
