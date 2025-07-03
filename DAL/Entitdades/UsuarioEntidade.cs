using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class UsuarioEntidade : BaseEntity
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool IsAdmin { get; set; }
        public int? PessoaId { get; set; }
        public PessoaEntidade Pessoa { get; set; }
    }
} 