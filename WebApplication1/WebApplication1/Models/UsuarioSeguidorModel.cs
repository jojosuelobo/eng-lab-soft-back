using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WebApplication1.Models;

[Table("USUARIO_SEGUIDOR")]
public class UsuarioSeguidorModel : BaseModel
{
    [PrimaryKey("ID_USUARIO_SEGUIDOR", false)]
    public Guid IdUsuarioSeguidor { get; set; }
    
    [Column("ID_USUARIO")]
    public Guid IdUsuario { get; set; }
    
    [Column("ID_SEGUIDOR")]
    public Guid IdSeguidor { get; set; }
}