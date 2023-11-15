using Supabase;
using WebApplication1.Contracts;
using WebApplication1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Postgrest;
using Postgrest.Responses;

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
        DataCriacao = usuario.DataCriacao,
        Pronomes = usuario.Pronomes
    };

    return Results.Ok(usuarioResponse);
});

app.MapPut("/usuarios/edit", async (Guid IdUsuario, CreateUsuarioRequest request, Supabase.Client client) =>
{
    // New client
    var usuarioAlterado = new UsuarioModel
    {
        Nome = request.Nome,
        Descricao = request.Descricao,
        FotoPerfil = request.FotoPerfil,
        Pronomes = request.Pronomes
    };

    if (usuarioAlterado.Nome.Length <= 0)
        return Results.Unauthorized();
    
    var responseInsert = await client.From<UsuarioModel>().Where(n => n.IdUsuario == IdUsuario)
        .Set(model => model.Nome ,usuarioAlterado.Nome)
        .Set(model => model.Descricao, usuarioAlterado.Descricao)
        .Set(model => model.FotoPerfil, usuarioAlterado.FotoPerfil)
        .Set(model => model.Pronomes, usuarioAlterado.Pronomes)
        .Update();
    
    if (!responseInsert.Models.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(responseInsert.Models.FirstOrDefault());
});

app.MapPut("/edit/id", async (Guid idLista, CreateListaRequest request, Supabase.Client client) =>
{
    // Newpost
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
        Descricao = request.Descricao,
    };

    if (conteudo.Any())
    {
        lista.Conteudo = JsonConvert.SerializeObject(conteudo);
    }

    if (tags != null && tags.Any())
    {
        lista.Tags = JsonConvert.SerializeObject(tags);
    }
    
    var get = await client
        .From<ListaModel>()
        .Where(n => n.IdLista == idLista)
        .Get();
    
    var lista1 = get.Models.FirstOrDefault();

    if (lista1 is null)
    {
        return Results.NotFound();
    }
    
    // TODO: Melhorar esse tanto de condicao horroroso
    
    var conteudoString = lista1.Conteudo.ToString();

    List<Conteudo>? conteudo1;
    
    if (conteudoString == null)
    {
        conteudo1 = null;
    }
    else
    {
        conteudo1 = JsonConvert.DeserializeObject<List<Conteudo>>(conteudoString);
    }

    var tagsString = lista.Tags.ToString();

    List<string>? tags1;
    
    if (tagsString == null)
    {
        tags1 = null;
    }
    else
    {
        tags1 = JsonConvert.DeserializeObject<List<string>>(tagsString);
    }

    var listaResponse = new Lista
    {
        IdLista = lista1.IdLista,
        IdUsuario = lista1.IdUsuario,
        Titulo = lista1.Titulo,
        Conteudo = conteudo1,
        NumLikes = lista1.NumLikes,
        DataCriacao = lista1.DataCriacao,
        Tags = tags1,
        Descricao = lista1.Descricao
    };

    if (listaResponse.Titulo != request.Titulo && request.Titulo != "")
    {
        listaResponse.Titulo = request.Titulo;
    }

    if (listaResponse.Conteudo != request.Conteudo && request.Conteudo != null)
    {
        listaResponse.Conteudo = request.Conteudo;
    }

    if (listaResponse.Tags != request.Tags && request.Tags != null)
    {
        listaResponse.Tags = request.Tags;
    }

    if (listaResponse.Descricao != request.Descricao && request.Descricao != "")
    {
        listaResponse.Descricao = request.Descricao;
    }

    var novoModel = new ListaModel
    {
        IdLista = listaResponse.IdLista,
        Titulo = listaResponse.Titulo,
        IdUsuario = listaResponse.IdUsuario,
        Descricao = listaResponse.Descricao,
        Conteudo = listaResponse.Conteudo,
        Tags = listaResponse.Tags,
        DataCriacao = listaResponse.DataCriacao,
        NumLikes = listaResponse.NumLikes
        
    };

    var responseInsert = await client.From<ListaModel>().Where(n => n.IdLista == idLista)
        .Set(model => model.Titulo ,novoModel.Titulo)
        .Set(model => model.Descricao, novoModel.Descricao)
        .Set(model => model.Conteudo, novoModel.Conteudo)
        .Set(model => model.Tags, novoModel.Tags)
        .Update();

    if (!responseInsert.Models.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok();
});

app.MapDelete("/delete/id", async (Guid idLista, Supabase.Client client) =>
{
    // TODO: adicionar validacao talvez
    await client.From<ListaModel>().Where(n => n.IdLista == idLista).Delete();
    return Results.Ok();
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
        Descricao = request.Descricao,
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

app.MapGet("/posts/usuario", async (Guid idUsuario, Supabase.Client client) =>
{
    var response = await client
        .From<ListaModel>()
        .Where(n=>n.IdUsuario == idUsuario)
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

// app.MapGet("/posts/busca", async (Supabase.Client client) =>
// {
//     //var queryValues = httpContext.Request.Query["tag"];
//     
//     //Console.WriteLine(queryValues.Count);
//     //Console.WriteLine(queryValues);
//
//     //ModeledResponse<ListaModel>? response;
//     
//     // if (queryValues.Count == 0)
//     // {
//     //     response = await client
//     //         .From<ListaModel>()
//     //         .Get();
//     // }
//     // else
//     // {
//     //     var tags = queryValues.Select(tg => tg).ToList();
//     //     
//     //     response = await client
//     //         .From<ListaModel>()
//     //         .Where(n => tags.Contains(n.Tags))
//     //         .Get();
//     // }
//
//     // var response = await client
//     // .From<ListaModel>()
//     // .Where(n => tags.Contains(n.Tags))
//     // .Get();
//     //
//     var listas = response.Models;
//
//     var listaResponse = new List<Lista>();
//
//     foreach (var lista in listas)
//     {
//         var conteudoString = lista.Conteudo.ToString();
//
//         List<Conteudo>? conteudo;
//     
//         if (conteudoString == null)
//         {
//             conteudo = null;
//         }
//         else
//         {
//             conteudo = JsonConvert.DeserializeObject<List<Conteudo>>(conteudoString);
//         }
//         
//         var tagsString = lista.Tags.ToString();
//
//         List<string>? tags;
//     
//         if (tagsString == null)
//         {
//             tags = null;
//         }
//         else
//         {
//             tags = JsonConvert.DeserializeObject<List<string>>(tagsString);
//         }
//         
//         var lst = new Lista
//         {
//             IdLista = lista.IdLista,
//             IdUsuario = lista.IdUsuario,
//             DataCriacao = lista.DataCriacao,
//             Conteudo = conteudo,
//             Titulo = lista.Titulo,
//             NumLikes = lista.NumLikes,
//             Tags = tags,
//             Descricao = lista.Descricao
//         };
//
//         listaResponse.Add(lst);
//     }
//     
//     if (!listas.Any())
//     {
//         return Results.NotFound();
//     }
//
//     return Results.Ok(listaResponse);
// });

//app.MapGet("/posts/teste", async (string tags, Supabase.Client client) => { return Results.Ok();});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
