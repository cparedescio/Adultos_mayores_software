using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlIndicacionesMedicas : MonoBehaviour
{

    public GameObject template;
    
    [Header("Configuraciones paso I")]
    private string sqlConsulta = "Select id,nombre from 'indicaciones medicas' order by nombre asc";

    public GameObject panelRegistros;
    public GameObject ubicacionRegistros;
    public Button btnAgregar;
    

    public Canvas actual;
    public Canvas anterior;
    
    string DatabaseName = "manual.s3db";
//    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    private string tabla = "'indicaciones medicas'";

    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    public RegistroDeIndicacionesMedicas registroWindow;

    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    
    private UnityEvent eventRegistrar = new UnityEvent();
    private UnityEvent<string> eventEditar = new MyStringEvent();
    private UnityEvent<string> eventAskEliminar = new MyStringEvent();
    private UnityEvent eventEliminar = new UnityEvent(); 
    
    string activeID = "";
    private void OnEnable()
    {
        Reload();
    }
    private void Editar(string id)
    {
        activeID = id;
        registroWindow.editar(id);
    }

    private void Registrar()
    {
        //registroWindow.nuevo();
    }

    void AskForElimination(string id)
    {
        string cmd = $"Select nombre FROM {tabla} WHERE id='{id}'";
        var Tabla=sqlcon.makeAnSqlConsult(cmd);
        if (Tabla.Rows.Count == 0)
        {
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "No se ha encontrado la informacion de la enfermedad", eventReturn2Menu);
            return;
        }
        Debug.Log("Iniciando la pregunta");
        actual.gameObject.SetActive(false);
        var nombredb = Tabla.Rows[0]["nombre"].ToString();
        activeID = id;
        askWindow.Preguntar($"¿Aceptas eliminar la indicación médica {nombredb}?",eventEliminar,eventDontDoNothing);
    }

    private void Eliminar()
    {
        actual.gameObject.SetActive(true);
        string cmd=$"DELETE FROM {tabla} WHERE id='{activeID}'";
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        }
        if (sqlcon.ExecuteACommand(cmd))
        {
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Exito", "Se han eliminado con éxito", eventDontDoNothing);
        }
        else
        {
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "Ha habido un error al eliminar el campo", eventReturn2Menu);
        }
    }

    
    private void Reload()
    {
        eventReturn2Menu.AddListener(Return2Menu);
        eventDontDoNothing.AddListener(NotDoNothing);
        
        eventRegistrar.AddListener(Registrar);
        eventEditar.AddListener(Editar);
        eventAskEliminar.AddListener(AskForElimination);
        eventEliminar.AddListener(Eliminar);
        

        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        };
        //Habrá pacientes registrados...
        var resConsulta=sqlcon.makeAnSqlConsult(sqlConsulta);
        if (resConsulta.Rows.Count == 0)
        {
            ubicacionRegistros.gameObject.SetActive(false);
        }
        else
        {
            ubicacionRegistros.gameObject.SetActive(true);
            var NumeroDeHijos = panelRegistros.transform.childCount;
            for (int i = 0; i < NumeroDeHijos; i++)
            {
                if (panelRegistros.transform.GetChild(i).name != "template")
                {
                    Destroy(panelRegistros.transform.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < resConsulta.Rows.Count; i++)
            {
                var id = resConsulta.Rows[i][0].ToString();
                var Nombre = resConsulta.Rows[i][1].ToString();
                var Childen=Instantiate(template, panelRegistros.transform);
                var Info=Childen.GetComponent<campoInfo>();
                Info.Initialize(id,Nombre,eventAskEliminar,eventEditar);
                Info.gameObject.SetActive(true);
            }
        }
    }
    void NotDoNothing()
    {
        actual.gameObject.SetActive(true);
    }
    private void Return2Menu()
    {
        actual.gameObject.SetActive(false);
        anterior.gameObject.SetActive(true);
    }
}
