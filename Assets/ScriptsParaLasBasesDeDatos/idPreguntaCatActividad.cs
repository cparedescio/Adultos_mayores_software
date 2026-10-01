using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace ScriptsParaLasBasesDeDatos
{
    public class idPreguntaCatActividad
    {
        public Int64 id { get; set; }
        public string pregunta { get; set; }
        public string categoria { get; set; }

        private bool value=false;
        public void setValue(bool actualValue)
        {
            value = actualValue;
        }
    }
    
}