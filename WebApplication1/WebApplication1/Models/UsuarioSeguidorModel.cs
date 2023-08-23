using Postgrest.Attributes;
using Postgrest.Models;

namespace WebApplication1.Models;

[Table("USUARIO_SEGUIDOR")]
public class UsuarioSeguidorModel : BaseModel
{
    [PrimaryKey("ID_USUARIO_SEGUIDOR", false)]
    public long IdUsuarioSeguidor { get; set; }
    
    [Column("ID_USUARIO")]
    public long IdUsuario { get; set; }
    
    [Column("ID_SEGUIDOR")]
    public long IdSeguidor { get; set; }
}