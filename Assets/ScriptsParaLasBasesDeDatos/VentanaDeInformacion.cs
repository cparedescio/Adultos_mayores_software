using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class VentanaDeInformacion : MonoBehaviour
{
    public Button btnAceptar;
    public TextMeshProUGUI textoEncabezado;
    public TextMeshProUGUI textoInformacion;
    public Canvas mycanvas;
    private UnityEvent accionAlPresionar;
    
    public void ShowInfo(string Encabezado, string Info, UnityEvent AlPresionar)
    {
        btnAceptar.onClick.RemoveAllListeners();
        btnAceptar.onClick.AddListener(Okpress);
        Debug.Log("Iniciando la ventana de información");
        textoEncabezado.text = Encabezado;
        textoInformacion.text = Info;
        accionAlPresionar = AlPresionar;
        mycanvas.gameObject.SetActive(true);
    }
    public void ShowInfo(string Encabezado, string Info, ref UnityEvent AlPresionar, ref Canvas Adesactivar)
    {
        Debug.Log("Iniciando la ventana de información");
        btnAceptar.onClick.RemoveAllListeners();
        btnAceptar.onClick.AddListener(Okpress);
        textoEncabezado.text = Encabezado;
        textoInformacion.text = Info;
        accionAlPresionar = AlPresionar;
        mycanvas.gameObject.SetActive(true);
        Debug.Log("Activated");
        Adesactivar.gameObject.SetActive(false);
    }
    void Okpress()
    {
        mycanvas.gameObject.SetActive(false);
        accionAlPresionar.Invoke();
    }
    
}
