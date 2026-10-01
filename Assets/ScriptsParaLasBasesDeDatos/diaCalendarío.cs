using System;
using System.Collections;
using System.Collections.Generic;
using ScriptsParaLasBasesDeDatos;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class diaCalendarío : MonoBehaviour
{
    public TextMeshProUGUI dia;
    public string id_paciente;
    public string id_cuidador;
    public DateTime mydate;
    public DetalleDelDia detalleDiario;
    
    public void makeInvisible()
    {
        var Imagen = GetComponent<Image>();
        Imagen.color = new Color(0, 0, 0, 0);
        dia.text = "";
        var myButton = GetComponent<Button>();
        myButton.onClick.RemoveAllListeners();
    }
    
    public void putDay(DateTime MyDay,string idPaciente,string idcuidador, DetalleDelDia bitacoraDiario)
    {
        mydate = MyDay;
        var Imagen = GetComponent<Image>();
        Imagen.color = new Color(224, 220, 204, 255);
        dia.text = MyDay.Date.Day.ToString();
        id_paciente = idPaciente;
        id_cuidador = idcuidador;
        detalleDiario = bitacoraDiario;
        var myButton = GetComponent<Button>();
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(displayDayDatail);
    }
    

    public void displayDayDatail()
    {
        detalleDiario.generate(id_paciente,id_cuidador,mydate);
        Debug.Log($"Mostrando el detalle del día: {mydate.ToShortDateString()} del paciente: {id_paciente}");
    }
    


}
