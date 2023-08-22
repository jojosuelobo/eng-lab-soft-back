using Postgrest.Models;

namespace WebApplication1.Models;

public class UsuarioSeguidorModel : BaseModel
{
    public long IdUsuarioSeguidor { get; set; }
    
    public long IdUsuario { get; set; }
    
    public long IdSeguidor { get; set; }
}