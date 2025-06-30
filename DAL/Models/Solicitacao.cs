using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DAL.Models;

public partial class Solicitacao : BaseEntity
{
    public DateTime Data { get; set; }
    public ManutencaoCalibracao TipoMC { get; set; }
    public string IdEquipamento { get; set; }
}