using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using  UnityEngine.Events;

public class registroEnfermedades : MonoBehaviour
{
    
    [Header("Configuracion de menu")]
    public Canvas anterior;
    public Canvas actual;
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    [Header("Configuración de campos")]
    public TMP_InputField nombreEnfermedad;
    public Button btnRegistrar;

    [Header("Configuración de enlaces a la base de datos")]
    public string labelNombre = "nombre";
    public string labelIdPaciente = "id_paciente";
    public string labelid = "id";
    private string idActualizacion = "";
    
    string DatabaseName = "manual.s3db";
    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    
    public string consultaEdicion = "Select * from enfermedades where id='@id'";
    public string tabla = "enfermedades";
    
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
    public void editar(string id,string nombre)
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

        nombreEnfermedad.text = "";
        
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
            infoWindow.ShowInfo("Error","No se ha encontrado la enfermedad a editar", ref eventReturn2Menu,ref actual);
        }
        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        nombreEnfermedad.text = consulta.Rows[0][labelNombre].ToString();
        
    }

    void validation()
    {
        Debug.Log("Iniciando la revision de datos");

        nombreEnfermedad.text = nombreEnfermedad.text.Trim();
        nombreEnfermedad.text = sqlcon.RemoveSpecialCharacters(nombreEnfermedad.text);

        if (nombreEnfermedad.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce el nombre de la enfermedad",ref eventDontDoNothing,ref actual);
            return;
        }
        //Solo se puede registrar una vez la enfermedad
        var cmd=$"Select * from {tabla} where {labelNombre}='{nombreEnfermedad}'";
        var consulta=sqlcon.makeAnSqlConsult(cmd);
        if (consulta.Rows.Count > 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","Ya se ha registrado esa enfermedad", ref eventReturn2Menu,ref actual);
            return;
        }

        var Mensaje = "guardar";
        if (Registrado)
        {
            Mensaje = "actualizar";
        }
        string mensajeDePregunta = $"Deseas {Mensaje} la enfermedad: {nombreEnfermedad.text}";
        actual.gameObject.SetActive(false);
        askWindow.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);
    }

    void Registrar()
    {
        string cmd=$"Select * from {tabla} where {labelNombre}='{nombreEnfermedad}' and id_paciente='{id_paciente}'";
        var consulta=sqlcon.makeAnSqlConsult(cmd);
        if (consulta.Rows.Count > 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Advertencia","Ya se ha registrado esa enfermedad",eventReturn2Menu);
            //Error for multiple handlings
            return;
        }
        
        cmd = $"Insert into {tabla}({labelNombre},{labelIdPaciente}) values('@nombre','{id_paciente}')";
        if (Registrado)
        {
            cmd=$"Update {tabla} set {labelNombre}='@nombre' where id='@id'";
            cmd = cmd.Replace("@id", idActualizacion);
        }
        Debug.Log($"Consulta de registro: {cmd}");
        cmd = cmd.Replace("@nombre", nombreEnfermedad.text);
        if (sqlcon.GoodInit)
        {
            Debug.Log($"Comando para SQL: {cmd}");
            if (sqlcon.ExecuteACommand(cmd))
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Exito", "Se ha guardado la enfermedad con éxito", eventReturn2Menu);
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
