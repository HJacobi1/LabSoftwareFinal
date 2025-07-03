namespace BLL.Models
{
    public class Metrologia
    {
        public int Id { get; set; }
        public string CACalibracao { get; set; }
        public string CAVerificacoes { get; set; }
        public string CapacidadeMedicao { get; set; }
        public string PeriodiciodadeCalibracao { get; set; }
        public string PeriodicidadeVerificacoesIntermediarias { get; set; }
        public string ResolucaoDivisaoEscala { get; set; }
    }
}