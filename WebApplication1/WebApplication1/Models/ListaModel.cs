using Postgrest.Attributes;
using Postgrest.Models;

namespace WebApplication1.Models;

[Table("LISTA")]
public class ListaModel : BaseModel
{
    [PrimaryKey("ID_LISTA", false)]
    public long IdLista { get; set; }
    
    [Column("TITULO")]
    public string Titulo { get; set; }
    
    [Column("CONTEUDO")]
    public string? Conteudo { get; set; }
    
    [Column("NUM_LIKES")]
    public long NumLikes { get; set; }
    
    [Column("DATA_CRIACAO")]
    public DateTime DataCriacao { get; set; }
    
    [Column("ID_USUARIO")]
    public long IdUsuario { get; set; }
}