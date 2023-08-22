using Postgrest.Models;

namespace WebApplication1.Models;

public class ListaModel : BaseModel
{
    public long IdLista { get; set; }
    
    public string Titulo { get; set; }
    
    public string Conteudo { get; set; }
    
    public long NumLikes { get; set; }
    
    public DateTime DataCriacao { get; set; }
    
    public long IdUsuario { get; set; }
}