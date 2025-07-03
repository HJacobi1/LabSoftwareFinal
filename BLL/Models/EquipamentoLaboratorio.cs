using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class EquipamentoLaboratorio
    {
        public int Id { get; set; }
        public string NroPatrimonio { get; set; }
        public string TagIdentificacao { get; set; }
        public string NroSerie { get; set; }
        public ModeloEquipamento Modelo { get; set; }
        public Metrologia DadosMetrologicos { get; set; }
        public int IdLaboratorio { get; set; }
    }
}
