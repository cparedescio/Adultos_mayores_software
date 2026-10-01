using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using  UnityEngine.Events;

public class registroDeAntecedentes : MonoBehaviour
{
    
    [Header("Configuracion de menu")]
    public Canvas anterior;
    public Canvas actual;
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    [Header("Configuración de campos")]
    public TMP_InputField antecedente;
    public TMP_InputField fecha;
    public Button btnRegistrar;
    
    [Header("Configuración de enlaces a la base de datos")]
    private string labelNombre = "nombre";
    private string labelfecha = "fecha";
    private string labelIdPaciente = "id_paciente";
//    private string labelid = "id";
    private string idActualizacion = "";
    private string tabla = "antecedentes";
    
    private string consultaEdicion = $"Select id, cast(fecha as varchar) as fecha, nombre from antecedentes where id='@id'";
    
    private string DatabaseName = "manual.s3db";
    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    
    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    private UnityEvent eventRegister = new UnityEvent();

    public string id_paciente = "";

    public void editar(string id)
    {
        idActualizacion = id;
        Registrado = true;
        cleanAndPrepare();
        fillInfo();
    }
    public void nuevo(string paciente_id)
    {
        this.id_paciente = paciente_id;
        Registrado = false;
        cleanAndPrepare();
    }
    
    public void cleanAndPrepare()
    {
        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);

        antecedente.text = "";
        
        eventReturn2Menu.RemoveAllListeners();
        eventDontDoNothing.RemoveAllListeners();
        eventRegister.RemoveAllListeners();
        btnRegistrar.onClick.RemoveAllListeners();
        
        eventReturn2Menu.AddListener(return2menu);
        eventDontDoNothing.AddListener(notDoNothing);
        eventRegister.AddListener(Registrar);
        btnRegistrar.onClick.AddListener(validation);
        
        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        };
        
    }
    public void fillInfo()
    {
        var cmdParametrized = consultaEdicion.Replace("@id", idActualizacion);
        var consulta=sqlcon.makeAnSqlConsult(cmdParametrized);
        Debug.Log("IniciandoLaConsulta");
        if (consulta.Rows.Count == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","No se ha encontrado la indicación médica a editar", ref eventReturn2Menu,ref actual);
        }
        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        antecedente.text = consulta.Rows[0][labelNombre].ToString();
        fecha.text = consulta.Rows[0][labelfecha].ToString();

    }
    
    void validation()
    {
        Debug.Log("Iniciando la revision de datos");

        antecedente.text = antecedente.text.Trim();
        antecedente.text = sqlcon.RemoveSpecialCharacters(antecedente.text);

        if (antecedente.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce el antecedente",ref eventDontDoNothing,ref actual);
            return;
        }

        DateTime fechaAuxiliar = new DateTime();
        if (!DateTime.TryParse(fecha.text, out fechaAuxiliar))
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce una fecha válida",ref eventDontDoNothing,ref actual);
            fecha.text = "";
            return;
        }

        
        var Mensaje = "guardar";
        if (Registrado)
        {
            Mensaje = "actualizar";
        }
        string mensajeDePregunta = $"Deseas {Mensaje} el antecedente médico: {antecedente.text}";
        actual.gameObject.SetActive(false);
        askWindow.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);
    }
    
    void Registrar()
    {
        var cmd = $"Insert into {tabla}({labelNombre},{labelIdPaciente},{labelfecha}) values('{antecedente.text}','{id_paciente}','{sqlcon.ConvertDate2Sql(DateTime.Parse(fecha.text))}')";
        if (Registrado)
        {
            cmd=$"Update {tabla} set {labelNombre}='{antecedente.text}',{labelfecha}='{DateTime.Parse(fecha.text).ToShortDateString()}' where id='{idActualizacion}'";
        }
        if (sqlcon.GoodInit)
        {
            Debug.Log($"Comando para SQL: {cmd}");
            if (sqlcon.ExecuteACommand(cmd))
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Exito", "Se ha guardado la indicación médica con éxito", eventReturn2Menu);
            }
            else
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error", "Ha habido un error", eventReturn2Menu);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "No se ha inicializado correctamente la base de datos", eventReturn2Menu);
            Debug.Log("No se ha inicializado correctamente la base de datos");
        }
    }
    
    void notDoNothing()
    {
        actual.gameObject.SetActive(true);
    }
    
    private void return2menu()
    {
        actual.gameObject.SetActive(false);
        anterior.gameObject.SetActive(true);
    }

    
}
