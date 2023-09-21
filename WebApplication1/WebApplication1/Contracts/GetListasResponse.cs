using WebApplication1.Models;

namespace WebApplication1.Contracts;

public class GetListasResponse
{
    public List<ListaModel> Listas = new List<ListaModel>();
}

public class Lista
{
    public Guid IdLista { get; set; }
    
    public string Titulo { get; set; }
    
    public List<Conteudo>? Conteudo { get; set; }
    
    public long NumLikes { get; set; }
    
    public DateTime DataCriacao { get; set; }
    
    public Guid IdUsuario { get; set; }
    
    public List<string>? Tags { get; set; }
}