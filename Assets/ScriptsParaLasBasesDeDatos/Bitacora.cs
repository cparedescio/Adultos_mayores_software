using System;
namespace ScriptsParaLasBasesDeDatos
{
    public class Bitacora
    {
        public Int64 id { get; set; }
        public Int64 id_paciente { get; set; }
        public Int64 id_acitivad { get; set; }
        public Int64 id_cuidador { get; set; }
        public DateTime fechaDeRealizacion { get; set; }
        public string nota { get; set; }
    }
}