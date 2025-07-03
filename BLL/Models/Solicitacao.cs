using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public ManutencaoCalibracao TipoMC { get; set; }
        public string IdEquipamento { get; set; }
        public string Descricao { get; set; }
    }
}
