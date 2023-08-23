using Supabase;
using WebApplication1.Contracts;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Client do banco de dandos
builder.Services.AddScoped<Supabase.Client>(_ => 
    new Supabase.Client(
        builder.Configuration["SupabaseUrl"],
        builder.Configuration["SupabaseKey"],
        new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/cadastro", async (
    CreateUsuarioRequest request,
    Supabase.Client client) =>
    {
        var usuario = new UsuarioModel
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Senha = request.Senha,
            Email = request.Email,
            FotoPerfil = request.FotoPerfil
        };

        var response = await client.From<UsuarioModel>().Insert(usuario);

        var novoUsuario = response.Models.First();

        return Results.Ok(novoUsuario.IdUsuario);
    });

app.MapGet("/usuarios/{id}", async (long IdUsuario, Supabase.Client client) =>
    {
        var response = await client
            .From<UsuarioModel>()
            .Where(n => n.IdUsuario == IdUsuario)
            .Get();

        var usuario = response.Models.FirstOrDefault();

        if (usuario is null)
        {
            return Results.NotFound();
        }

        var usuarioResponse = new UsuarioResponse
        {
            IdUsuario = usuario.IdUsuario,
            Nome = usuario.Nome,
            //Senha = usuario.Senha
            Descricao = usuario.Descricao,
            //Email = usuario.Email
            FotoPerfil = usuario.FotoPerfil,
            DataCriacao = usuario.DataCriacao
        };

        return Results.Ok(usuarioResponse);
    });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
