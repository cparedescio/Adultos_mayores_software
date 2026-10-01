using System;

namespace ScriptsParaLasBasesDeDatos
{
    public class Activdad
    {
        public Int64 id { get; set; }
        public Int64 id_paciente { get; set; }
        public Int64 id_cuidador { get; set; }
        public DateTime fecha { get; set; }
    }
}