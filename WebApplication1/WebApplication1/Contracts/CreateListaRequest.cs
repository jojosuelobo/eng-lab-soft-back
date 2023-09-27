using WebApplication1.Models;

namespace WebApplication1.Contracts;

public class CreateListaRequest
{
    public string Titulo { get; set; } = "";

    public List<Conteudo>? Conteudo { get; set; }
    
    public Guid IdUsuario { get; set; }
    public List<string>? Tags { get; set; }

    public string Descricao { get; set; } = "";
}

public class CreateListaResponse
{
    public string Titulo { get; set; }
    
    public List<object>? Conteudo { get; set; }
    
    public Guid IdUsuario { get; set; }
    public object Tags { get; set; }
}