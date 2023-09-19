namespace WebApplication1.Contracts;

public class CreateListaRequest
{
    public string Titulo { get; set; } = "";

    public string Conteudo { get; set; } = "";
    
    public Guid IdUsuario { get; set; }
    public string Tags { get; set; }
}

public class CreateListaResponse
{
    public string Titulo { get; set; }
    
    public string Conteudo { get; set; }
    
    public Guid IdUsuario { get; set; }
    public string Tags { get; set; }
}