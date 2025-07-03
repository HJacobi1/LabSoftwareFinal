using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ModeloEquipamentoEntidade : BaseEntity
    {
        public string Identificacao { get; set; }
        public string Descricao { get; set; }
        public AnalogicoDigital TipoAD { get; set; }
        public string Marca { get; set; }
    }
}
