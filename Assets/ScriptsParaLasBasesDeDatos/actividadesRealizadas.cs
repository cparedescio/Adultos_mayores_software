using System;
using System.Linq;
using System.Text;
using TMPro;

namespace ScriptsParaLasBasesDeDatos
{
    public class actividadesRealizadas
    {
        public int id { get; set; }
        public DateTime FechaDeRealizacion { get; set; }
        public string Actividad { get; set; }
        public string Cuidador { get; set; }
        
        public string GeneraDescripción()
        {
            var ConstructorDeTexto = new StringBuilder();
            ConstructorDeTexto.Append(FechaDeRealizacion.ToShortTimeString());
            ConstructorDeTexto.Append("\t");
            ConstructorDeTexto.Append(Actividad);
            return ConstructorDeTexto.ToString();
        }
    }


}