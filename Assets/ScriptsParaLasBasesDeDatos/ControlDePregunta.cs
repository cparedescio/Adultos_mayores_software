using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlDePregunta : MonoBehaviour
{
    public TextMeshProUGUI TextoDePregunta;

    public Button Aceptar;
    public Button Rechazar;

    private UnityEvent EventoAlAceptar;
    private UnityEvent EventoAlRechazar;
    
    public Canvas Actual;
    
    void PressOk()
    {
        Actual.gameObject.SetActive(false);
        EventoAlAceptar.Invoke();
    }

    void PressNot()
    {
        Actual.gameObject.SetActive(false);
        EventoAlRechazar.Invoke();
    }

    public void Preguntar(string Pregunta, UnityEvent eventAceptar, UnityEvent eventRechazar)
    {
        EventoAlAceptar = eventAceptar;
        EventoAlRechazar = eventRechazar;
        TextoDePregunta.text = Pregunta;
        
        Actual.gameObject.SetActive(true);
        Aceptar.onClick.RemoveAllListeners();
        Rechazar.onClick.RemoveAllListeners();
        
        Aceptar.onClick.AddListener(PressOk);
        Rechazar.onClick.AddListener(PressNot);
    }
    
    
}
