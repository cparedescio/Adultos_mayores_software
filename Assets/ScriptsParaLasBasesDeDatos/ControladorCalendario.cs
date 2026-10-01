using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using ScriptsParaLasBasesDeDatos;
using UnityEngine.Events;
using TMPro;

public class ControladorCalendario : MonoBehaviour
{
    public GameObject templateDay;
    
    
    public void CreateCalendar(DateTime fecha, string idpaciente, string idcuidador, DetalleDelDia bitacoraDiario)
    {
        var firstDayOfMonth=fecha.FirstDayOfMonth();
        var daysInMonth = fecha.DaysInMonth();
        
        var sons = this.transform.childCount;
        for (int i = 0; i < sons; i++)
        {
            if (this.transform.GetChild(i).name != "Encabezado" && this.transform.GetChild(i).name != "template")
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        //Generate innactive days
        var innactiveDays = 0;
        switch (firstDayOfMonth.DayOfWeek)
        {
            case DayOfWeek.Monday:
                innactiveDays = 0;
                break;
            case DayOfWeek.Tuesday:
                innactiveDays = 1;
                break;
            case DayOfWeek.Wednesday:
                innactiveDays = 2;
                break;
            case DayOfWeek.Thursday:
                innactiveDays = 3;
                break;
            case DayOfWeek.Friday:
                innactiveDays = 4;
                break;
            case DayOfWeek.Saturday:
                innactiveDays = 5;
                break;
            case DayOfWeek.Sunday:
                innactiveDays = 6;
                break;
        }
        templateDay.SetActive(false);
        for (int i = 0; i < innactiveDays; i++)
        {
            var Childen=Instantiate(templateDay, this.transform);
            var Info=Childen.GetComponent<diaCalendarío>();
            Info.makeInvisible();
            Info.gameObject.SetActive(true);
        }
        for (int i = 0; i < daysInMonth; i++)
        {
            var Childen=Instantiate(templateDay, this.transform);
            var Info=Childen.GetComponent<diaCalendarío>();
            Info.putDay(firstDayOfMonth,idpaciente,idcuidador,bitacoraDiario);
            Info.gameObject.SetActive(true);
            firstDayOfMonth=firstDayOfMonth.AddDays(1);
        }
    }
    
}
