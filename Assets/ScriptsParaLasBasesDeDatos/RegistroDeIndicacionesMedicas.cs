using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class RegistroDeIndicacionesMedicas : MonoBehaviour
{

    [Header("Configuracion de menu")]
    public Canvas anterior;
    public Canvas actual;
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    [Header("Configuración de campos")]
    public TMP_InputField nombre;
    public Button btnRegistrar;

    [Header("Configuración de enlaces a la base de datos")]
    private string labelNombre = "nombre";
//    private string labelid = "id";
    private string labelpaciente="id_paciente";
    private string idActualizacion = "";
    
    string DatabaseName = "manual.s3db";
    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    
    private string consultaEdicion = "Select * from 'indicaciones_medicas' where id='@id'";
    private string tabla = "'indicaciones_medicas'";
    
    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    private UnityEvent eventRegister = new UnityEvent();

    public string id_paciente;
    public void editar(string id)
    {
        idActualizacion = id;
        Registrado = true;
        cleanAndPrepare();
        fillInfo();
    }

    public void nuevo(string paciente)
    {
        id_paciente = paciente;
        Registrado = false;
        cleanAndPrepare();
    }

    public void cleanAndPrepare()
    {
        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);

        nombre.text = "";
        
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
            infoWindow.ShowInfo("Error","No se ha encontrado la indicación médica a editar", ref eventReturn2Menu,ref actual);
        }
        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        nombre.text = consulta.Rows[0][labelNombre].ToString();
        
    }

    void validation()
    {
        Debug.Log("Iniciando la revision de datos");

        nombre.text = nombre.text.Trim();
        nombre.text = sqlcon.RemoveSpecialCharacters(nombre.text);

        if (nombre.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce el nombre de la indicación médica",ref eventDontDoNothing,ref actual);
            return;
        }
        
        //Solo se puede registrar una vez la enfermedad
        var cmd=$"Select * from {tabla} where {labelNombre}='{nombre}'";
        var consulta=sqlcon.makeAnSqlConsult(cmd);
        if (consulta.Rows.Count > 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","Ya se ha registrado esa indicación médica", ref eventReturn2Menu,ref actual);
            return;
        }

        var Mensaje = "guardar";
        if (Registrado)
        {
            Mensaje = "actualizar";
        }
        string mensajeDePregunta = $"Deseas {Mensaje} la indicación médica: {nombre.text}";
        actual.gameObject.SetActive(false);
        askWindow.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);
    }

    void Registrar()
    {
        string cmd=$"Select * from {tabla} where {labelNombre}='{nombre.text}' and {labelpaciente}='{id_paciente}'";
        var consulta=sqlcon.makeAnSqlConsult(cmd);
        if (consulta.Rows.Count > 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Advertencia", "Ya se ha registrado esa indicación médica", eventReturn2Menu);
            //Error for multiple handlings
            return;
        }
        
        cmd = $"Insert into {tabla}({labelNombre},{labelpaciente}) values('@nombre','{id_paciente}')";
        if (Registrado)
        {
            cmd=$"Update {tabla} set {labelNombre}='@nombre' where id='@id'";
            cmd = cmd.Replace("@id", idActualizacion);
        }
        cmd = cmd.Replace("@nombre", nombre.text);
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
