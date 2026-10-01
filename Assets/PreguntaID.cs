using System;
using System.Collections.Generic;
using System.Linq;
using ScriptsParaLasBasesDeDatos;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreguntaID : MonoBehaviour
{
    public string pregunta = "";
    public List<int> ids = new List<int>();
    public Dictionary<string, bool> valoresActuales = new Dictionary<string, bool>();
    
    public UnityEvent<Tuple<string,bool>> eventoDeActualizacion = new IDActivityEvent();
    public Toggle onOff;
    public void SetInformation(string _pregunta, Dictionary<Int64,bool> ids, IDActivityEvent _modEvent)
    {
        valoresActuales.Clear();
        var numberOfTrues = 0;
        foreach (var valuePair in ids)
        {
            if (valuePair.Value)
            {
                numberOfTrues++;
            }
        }
        onOff.onValueChanged.RemoveAllListeners();
        if (numberOfTrues > 0)
        {
            onOff.isOn = true;
        }else
        {
            onOff.isOn = false;
        }
        onOff.onValueChanged.AddListener(ChangeToogle);
        pregunta = _pregunta;
        onOff.GetComponentInChildren<Text>().text = _pregunta;
        eventoDeActualizacion = _modEvent;
    }

    void ChangeToogle(bool state)
    {
        Debug.Log($"Se ha envíado el cambio de estado de la pregunta {pregunta} a {state}");
        eventoDeActualizacion.Invoke(new Tuple<string, bool>(pregunta,state));
    }
    

}
