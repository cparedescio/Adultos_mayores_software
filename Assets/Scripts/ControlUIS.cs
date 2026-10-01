using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUIS : MonoBehaviour
{
    public GameObject UI_Portada;

    public GameObject UI_Introduccion;

    public GameObject UI_Modulos;

    public GameObject UI_Generales;

    public GameObject UI_Estimulacion;

    public GameObject UI_Especificos;

    public GameObject UI_Auxilios;

    public GameObject UI_Cuidador;

    public GameObject UI_Bitacora;
    public GameObject UI_VentanaEvento; /**/
    public GameObject UI_VentanaProcedimiento;/**/
    // Start is called before the first frame update
    void Start()
    {
        UI_Portada.SetActive(true);
        UI_Introduccion.SetActive(false);
        UI_Modulos.SetActive(false);
        UI_Generales.SetActive(false);
        UI_Estimulacion.SetActive(false);
        UI_Especificos.SetActive(false);
        UI_Auxilios.SetActive(false);
        UI_Bitacora.SetActive(false);
        UI_VentanaEvento.SetActive(false);/**/
        UI_VentanaProcedimiento.SetActive(false);/**/

    }

    public void Salir()
    {
        Application.Quit();
        UnityEngine.Debug.Log("Salio: ");
    }
}
