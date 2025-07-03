namespace DAL.Models
{
    public class MetrologiaEntidade : BaseEntity
    {
        public int TipoMC { get; set; }
        public string CACalibracao { get; set; }
        public string CAVerificacao { get; set; }
        public string CapacidadeMedicao { get; set; }
        public string Periodicidade { get; set; }
        public string DivisaoEscala { get; set; }
    }
}