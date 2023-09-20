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
            Descricao = usuario.Descricao,
            FotoPerfil = usuario.FotoPerfil,
            DataCriacao = usuario.DataCriacao
        };

        return Results.Ok(usuarioResponse);
    });

app.MapPost("/lista", async (
    CreateListaRequest request,
    Supabase.Client client) =>
{
    //var conteudoJson = JsonConvert.SerializeObject(request.Conteudo);
    
    //var tagsJson = JsonConvert.DeserializeObject(request.Tags!);
    //Console.Write(tagsJson!.ToString());
    List<string>? tags = request.Tags;
    //List<Conteudo> conteudo = request.Conteudo;
    var conteudo = new List<Conteudo>();

    if (request.Conteudo != null)
    {
        conteudo = request.Conteudo;
        // foreach (var item in request.Conteudo)
        // {
        //     //Console.WriteLine($"{item.NomeItem}: {item.DescricaoItem}");
        //     var conteudoTemp = new Conteudo();
        //     conteudoTemp.NomeItem = item.NomeItem;
        //     conteudoTemp.DescricaoItem = item.DescricaoItem;
        //     conteudo.Add(conteudoTemp);
        // }
    }
        
        
        
    //Console.Write(request.Tags);
    
    var lista = new ListaModel
    {
        Titulo = request.Titulo,
        //Conteudo = conteudoJson,
        IdUsuario = request.IdUsuario,
        //Tags = tags
    };

    if (conteudo.Any())
    {
        lista.Conteudo = JsonConvert.SerializeObject(conteudo);

        // JObject fromObject = JObject.FromObject(lista.Conteudo);
        //
        // foreach (var pair in fromObject)
        // {
        //     Console.WriteLine($"{pair.Key}: {pair.Value}");
        // }
    }

    if (tags != null && tags.Any())
    {
        //lista.Tags = new List<string>();

        lista.Tags = JsonConvert.SerializeObject(tags);

        // foreach (var elemento in tags)
        // {
        //     lista.Tags.Add(elemento);
        // }
    }
    
    var response = await client.From<ListaModel>().Insert(lista);

    var novaLista = response.Models.First();

    return Results.Ok(novaLista.IdLista);
});

app.MapGet("/lista/{id}", async (Guid idLista, Supabase.Client client) =>
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
    
    //object jsonConteudo = JsonConvert.DeserializeObject(lista.Conteudo)

    var listaResponse = new Lista
    {
        IdLista = lista.IdLista,
        IdUsuario = lista.IdUsuario,
        Titulo = lista.Titulo,
        //Conteudo = lista.Conteudo,
        NumLikes = lista.NumLikes,
        DataCriacao = lista.DataCriacao
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

app.MapGet("/", async (Supabase.Client client) =>
{
    var response = await client
        .From<ListaModel>()
        .Get();

    var listas = response.Models;

    var listaResponse = new List<Lista>();

    foreach (var lista in listas)
    {
        var lst = new Lista();
        lst.IdLista = lista.IdLista;
        lst.IdUsuario = lista.IdUsuario;
        lst.DataCriacao = lista.DataCriacao;
        //lst.Conteudo = lista.Conteudo;
        lst.Titulo = lista.Titulo;
        lst.NumLikes = lista.NumLikes;
        
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
