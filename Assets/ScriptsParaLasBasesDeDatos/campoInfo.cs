using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class campoInfo : MonoBehaviour
{
    [SerializeField] public string nombre;
    [SerializeField] public string id;
    [SerializeField] public Button btnModificar;
    [SerializeField] public Button btnEliminar;
    public TextMeshProUGUI labelNombre;

    private UnityEvent<string> _eventModificar;
    private UnityEvent<string> _eventEliminar;
    
    public void Initialize(string id, string name, UnityEvent<string>  eEliminar, UnityEvent<string>  eEditar)
    {
        labelNombre.text = name;
        this.id = id;
        btnModificar.onClick.AddListener(Modificar);
        btnEliminar.onClick.AddListener(Eliminar);
        _eventEliminar = eEliminar;
        _eventModificar = eEditar;
    }

    private void Modificar()
    {
        _eventModificar.Invoke(id);
    }

    private void Eliminar()
    { 
        _eventEliminar.Invoke(id);
    }
}