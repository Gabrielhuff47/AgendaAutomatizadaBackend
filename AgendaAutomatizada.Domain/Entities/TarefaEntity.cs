using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaAutomatizada.Domain.Entities
{
    public class TarefaEntity
    {
        public int IdTarefa { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioAtualizacao { get; set; } = string.Empty;
        public DateTime DataAtualizacao { get; set; }
    }
}