namespace WebApplication1.Contracts;

public class CreateUsuarioRequest
{
    public string Nome { get; set; }
    
    public string? Descricao { get; set; }
    
    public string? FotoPerfil { get; set; }
    
    public string? Pronomes { get; set; }
}

public class UsuarioResponse
{
    public Guid IdUsuario { get; set; }
    
    public string Nome { get; set; }
    
    public string? Descricao { get; set; }
    
    public string? FotoPerfil { get; set; }
    
    public DateTime DataCriacao { get; set; }
    
    public string? Pronomes { get; set; }
}