using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Laboratorio : BaseEntity
{
    public int Codigo { get; set; }
    public string Endereco { get; set; }
    public string Nome { get; set; }
    public List<Pessoa> Responsaveis { get; set; }
    public List<Equipamento> Equipamentos { get; set; }
}