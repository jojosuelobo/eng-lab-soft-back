using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;

namespace WebApplication1.Models;

[Table("LISTA")]
public class ListaModel : BaseModel
{
    [PrimaryKey("ID_LISTA", false)]
    public Guid IdLista { get; set; }
    
    [Column("TITULO")]
    public string Titulo { get; set; }
    
    [Column("CONTEUDO")]
    public object Conteudo { get; set; }
    
    [Column("NUM_LIKES")]
    public long NumLikes { get; set; }
    
    [Column("DATA_CRIACAO")]
    public DateTime DataCriacao { get; set; }
    
    [Column("ID_USUARIO")]
    public Guid IdUsuario { get; set; }
    
    [Column("TAGS")]
    public object Tags { get; set; }
    
    [Column("DESCRICAO")]
    public string Descricao { get; set; }
}

[Serializable]
public class Conteudo
{
    public string NomeItem { get; set; }
    public string DescricaoItem { get; set; }
}