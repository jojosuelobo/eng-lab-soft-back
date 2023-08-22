using Postgrest.Models;

namespace WebApplication1.Models;

public class UsuarioModel : BaseModel
{
    public long IdUsuario { get; set; }
    
    public string Nome { get; set; }
    
    public string Senha { get; set; }
    
    public string Descricao { get; set; }
    
    public string Email { get; set; }
    
    public string FotoPerfil { get; set; }
    
    public DateTime DataCriacao { get; set; }
}