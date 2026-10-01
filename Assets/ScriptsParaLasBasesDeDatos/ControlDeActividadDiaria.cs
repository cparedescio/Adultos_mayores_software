using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ScriptsParaLasBasesDeDatos
{
    public class ControlDeActividadDiaria : MonoBehaviour
    {
        public Toggle infoActividad;
        private EventCambioActividadDiaria eventoAlCambio;
        private RequeriedActivities actividad = new RequeriedActivities();
        public void SetData(RequeriedActivities acitivdadRequerida, EventCambioActividadDiaria EventoAlCambio)
        {
            this.actividad = acitivdadRequerida;
            infoActividad.GetComponentInChildren<Text>().text = actividad.actividad;
            eventoAlCambio = EventoAlCambio;
            infoActividad.onValueChanged.RemoveAllListeners();
            infoActividad.isOn = actividad.getState();
            infoActividad.onValueChanged.AddListener(Cambio);
        }

        private void Cambio(bool arg0)
        {
            eventoAlCambio.Invoke(new Toggle2Activity(){actividad = actividad,newValue = arg0});
        }
    }

    public class Toggle2Activity
    {
        public RequeriedActivities actividad { get; set; }
        public bool newValue { get; set; }
    }
    
    public class EventCambioActividadDiaria : UnityEvent<Toggle2Activity>
    {
        
    }
}