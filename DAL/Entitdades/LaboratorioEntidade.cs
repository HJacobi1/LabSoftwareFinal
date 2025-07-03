using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class LaboratorioEntidade : BaseEntity
{
    public int Codigo { get; set; }
    public string Endereco { get; set; }
    public string Nome { get; set; }
    public List<PessoaEntidade> Responsaveis { get; set; }
    public List<EquipamentoEntidade> Equipamentos { get; set; }
}