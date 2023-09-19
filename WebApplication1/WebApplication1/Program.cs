using Supabase;
using WebApplication1.Contracts;
using WebApplication1.Models;
using Newtonsoft.Json;
using Postgrest;

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

// TODO: alterar esta rota para usar coisa do auth
// app.MapPost("/cadastro", async (
//     CreateUsuarioRequest request,
//     Supabase.Client client) =>
//     {
//         var usuario = new UsuarioModel
//         {
//             Nome = request.Nome,
//             Senha = request.Senha,
//             Email = request.Email
//         };
//
//         var response = await client.From<UsuarioModel>().Insert(usuario);
//
//         var novoUsuario = response.Models.First();
//
//         return Results.Ok(novoUsuario.IdUsuario);
//     });

app.MapGet("/usuarios/{id}", async (Guid IdUsuario, Supabase.Client client) =>
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

app.MapPost("/criarlista", async (
    CreateListaRequest request,
    Supabase.Client client) =>
    {
        var lista = new ListaModel
        {
            Titulo = request.Titulo,
            Conteudo = request.Conteudo,
            IdUsuario = request.IdUsuario,
            Tags = request.Tags
            // Nome = request.Nome,
            // Senha = request.Senha,
            // Email = request.Email
        };

        var response = await client.From<ListaModel>().Insert(lista);

        var novaLista = response.Models.First();

        return Results.Ok(novaLista.IdLista);
    });

app.MapGet("/", async (Supabase.Client client) =>
{
    var response = await client
        .From<ListaModel>()
        .Range(9)
        .Order("DataCriacao",Constants.Ordering.Descending)
        .Get();

    var listas = response.Models;

    if (!listas.Any())
    {
        return Results.NotFound();
    }
    // var listasResponse = new GetListasResponse();
    //
    // foreach (ListaModel lista in listas)
    // {
    //     listasResponse.Listas.Add(lista);
    // }

    return Results.Ok(response);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
