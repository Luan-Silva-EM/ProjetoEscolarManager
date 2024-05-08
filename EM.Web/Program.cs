using EM.Domain;
using EM.Domain.Interfaces;
using EM.Repository;
using EM.Web.Controllers.Utilitarios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioAbstrato<Aluno>, RepositorioAluno>();
builder.Services.AddTransient<IRepositorioAbstrato<Cidade>, RepositorioCidade>();
builder.Services.AddTransient<IRepositorioAluno<Aluno>, RepositorioAluno>();
builder.Services.AddTransient<TabelaRelatorio>();

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
