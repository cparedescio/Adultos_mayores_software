using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistroSignosDiario : MonoBehaviour
{
    public Canvas anterior;
    private Canvas actual;
    
    private void Start()
    {
        actual = this.gameObject.GetComponent<Canvas>();
    }

    public void nuevo(int id_paciente, int id_cuidador, DateTime fechaActual)
    {
        anterior.gameObject.SetActive(false);
    }

    public void modificar(int id)
    {
        anterior.gameObject.SetActive(false);
    }

    public void salir()
    {
        anterior.gameObject.SetActive(true);
        actual.gameObject.SetActive(false);
    }
}
