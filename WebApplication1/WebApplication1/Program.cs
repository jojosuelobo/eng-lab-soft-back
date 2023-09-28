using Supabase;
using WebApplication1.Contracts;
using WebApplication1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", buildr =>
    {
        buildr.WithOrigins(("http://localhost:5173"))
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/usuarios/id", async (Guid IdUsuario, Supabase.Client client) =>
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
        Descricao = usuario.Descricao,
        FotoPerfil = usuario.FotoPerfil,
        DataCriacao = usuario.DataCriacao
    };

    return Results.Ok(usuarioResponse);
});

app.MapPost("/newpost", async (
    CreateListaRequest request,
    Supabase.Client client) =>
{
    List<string>? tags = request.Tags;
    var conteudo = new List<Conteudo>();

    if (request.Conteudo != null)
    {
        conteudo = request.Conteudo;
    }
    
    var lista = new ListaModel
    {
        Titulo = request.Titulo,
        IdUsuario = request.IdUsuario,
    };

    if (conteudo.Any())
    {
        lista.Conteudo = JsonConvert.SerializeObject(conteudo);
    }

    if (tags != null && tags.Any())
    {
        lista.Tags = JsonConvert.SerializeObject(tags);
    }
    
    var response = await client.From<ListaModel>().Insert(lista);

    var novaLista = response.Models.First();

    return Results.Ok(novaLista.IdLista);
});

app.MapGet("/posts/id", async (Guid idLista, Supabase.Client client) =>
{
    var response = await client
        .From<ListaModel>()
        .Where(n => n.IdLista == idLista)
        .Get();
    
    var lista = response.Models.FirstOrDefault();

    if (lista is null)
    {
        return Results.NotFound();
    }
    
    // TODO: Melhorar esse tanto de condicao horroroso
    
    var conteudoString = lista.Conteudo.ToString();

    List<Conteudo>? conteudo;
    
    if (conteudoString == null)
    {
        conteudo = null;
    }
    else
    {
        conteudo = JsonConvert.DeserializeObject<List<Conteudo>>(conteudoString);
    }

    var tagsString = lista.Tags.ToString();

    List<string>? tags;
    
    if (tagsString == null)
    {
        tags = null;
    }
    else
    {
        tags = JsonConvert.DeserializeObject<List<string>>(tagsString);
    }

    var listaResponse = new Lista
    {
        IdLista = lista.IdLista,
        IdUsuario = lista.IdUsuario,
        Titulo = lista.Titulo,
        Conteudo = conteudo,
        NumLikes = lista.NumLikes,
        DataCriacao = lista.DataCriacao,
        Tags = tags,
        Descricao = lista.Descricao
    };

    return Results.Ok(listaResponse);
});

app.MapGet("/usersTeste", async (Supabase.Client client) =>
{
    var response = await client
        .From<UsuarioModel>()
        .Get();

    var listas = response.Models;

    var usuarioResponse = new List<UsuarioResponse>();

    foreach (var usuario in listas)
    {
        var usr = new UsuarioResponse();
        usr.IdUsuario = usuario.IdUsuario;
        usr.Nome = usuario.Nome;
        usr.Descricao = usuario.Descricao;
        usr.FotoPerfil = usuario.FotoPerfil;
        usr.DataCriacao = usuario.DataCriacao;
        usuarioResponse.Add(usr);
    }
    
    if (!listas.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(usuarioResponse);
});

app.MapGet("/posts", async (Supabase.Client client) =>
{
    var response = await client
        .From<ListaModel>()
        .Limit(10)
        .Get();

    var listas = response.Models;

    var listaResponse = new List<Lista>();

    foreach (var lista in listas)
    {
        var conteudoString = lista.Conteudo.ToString();

        List<Conteudo>? conteudo;
    
        if (conteudoString == null)
        {
            conteudo = null;
        }
        else
        {
            conteudo = JsonConvert.DeserializeObject<List<Conteudo>>(conteudoString);
        }
        
        var tagsString = lista.Tags.ToString();

        List<string>? tags;
    
        if (tagsString == null)
        {
            tags = null;
        }
        else
        {
            tags = JsonConvert.DeserializeObject<List<string>>(tagsString);
        }
        
        var lst = new Lista
        {
            IdLista = lista.IdLista,
            IdUsuario = lista.IdUsuario,
            DataCriacao = lista.DataCriacao,
            Conteudo = conteudo,
            Titulo = lista.Titulo,
            NumLikes = lista.NumLikes,
            Tags = tags,
            Descricao = lista.Descricao
        };

        listaResponse.Add(lst);
    }
    
    if (!listas.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(listaResponse);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
