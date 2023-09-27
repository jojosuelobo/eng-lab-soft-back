using Postgrest.Attributes;
using Postgrest.Models;

namespace WebApplication1.Models;

[Table("USUARIO")]
public class UsuarioModel : BaseModel
{
    [PrimaryKey("ID_USUARIO", false)]
    public Guid IdUsuario { get; set; }
    
    [Column("NOME")]
    public string Nome { get; set; }
    [Column("DESCRICAO")]
    public string? Descricao { get; set; }
    
    [Column("FOTO_PERFIL")]
    public string? FotoPerfil { get; set; }
    
    [Column("DATA_CRIACAO")]
    public DateTime DataCriacao { get; set; }
    
    // TODO: adicionar seguidores aqui
}