using ContactsSync.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication<ContactsSyncWebModule>();
// �������autofac���޷��������л�ȡ�ִ���DbContext
builder.Host.UseAutofac();
var app = builder.Build();
app.InitializeApplication();
app.Run();