using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class Laboratorio
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Endereco { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<Pessoa>? Responsaveis { get; set; }
        public List<EquipamentoLaboratorio>? Equipamentos { get; set; }
    }
}
