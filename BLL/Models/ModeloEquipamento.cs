using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class ModeloEquipamento
    {
        public int Id { get; set; }
        public string Identificacao { get; set; }
        public string Descricao { get; set; }
        public AnalogicoDigital TipoAD { get; set; }
        public string Marca { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
