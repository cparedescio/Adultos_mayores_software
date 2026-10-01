using System;

namespace ScriptsParaLasBasesDeDatos
{
    public class SignosVitales
    {
        public Int64 id { get; set; }
        public Int64 id_cuidador { get; set; }
        public double FC { get; set; }
        public double FR { get; set; }
        public double TA  { get; set; }
        public double Temp { get; set; }
        public double Glucosa { get; set; }
        public string Nota { get; set; }
    }
}