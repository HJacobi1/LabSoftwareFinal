using DAL.Enums;

namespace DAL.Models
{
    public class Metrologia : BaseEntity
    {
        public ManutencaoCalibracao TipoMC { get; set; }
        public string CACalibracao { get; set; }
        public string CAVerificacao { get; set; }
        public string CapacidadeMedicao { get; set; }
        public string Periodicidade { get; set; }
        public string DivisaoEscala { get; set; }
    }
}