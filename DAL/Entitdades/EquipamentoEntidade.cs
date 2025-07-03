using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DAL.Models;

public partial class EquipamentoEntidade : BaseEntity
{
    public ModeloEquipamentoEntidade Modelo { get; set; }
    public string NroPatrimonio { get; set; }      
    public string CertificadoCalibracao { get; set; }
    public string NroSerie { get; set; }
    public DateTime DataEntrada { get; set; }
    public int CodLaboratorio { get; set; }
    public MetrologiaEntidade Metrica { get; set; } 
}