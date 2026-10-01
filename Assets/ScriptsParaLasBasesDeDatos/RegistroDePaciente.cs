using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
public class RegistroDePaciente : MonoBehaviour
{
    [Header("Configuracion de campos")]
    public Canvas anterior;
    public Canvas actual;
    public TMP_Dropdown tiposDeSangre;
    public TMP_InputField nombre;
    public TMP_InputField edad;
    public TMP_InputField datosMedico;
    public TMP_InputField datosEmergencia;
    public TMP_InputField datosSeguro;
    public Toggle mujer;
    public Toggle hombre;

    public Button btnRegistrar;

    [Header("Configuración de enlaces a la base de datos")]
    public string labelNombre = "nombre";
    public string labelEdad = "edad";
    public string labelSexo = "sexo";
    public string labelTipoDeSangre = "tipoDeSangre";
    public string labelDatosMedico = "medico";
    public string labelDatosEmergencia = "emergencia";
    public string labelSeguro = "servicio";

    private string idActualizacion = "";

    [Header("Ajustes de la válidación")] 
    private bool obligatorioDatosMedico = true;
    private bool obligatorioDatosServicio = true;
    private bool obligatorioDatosEmergencia = true;
    [SerializeField] private string sexoActual;
    
    
    string DatabaseName = "manual.s3db";
    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();

    public string consultaEdicion = "Select * from paciente where id='@id'";
    
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    
    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    private UnityEvent eventRegister = new UnityEvent();

    

    public void EditarPaciente(string idPaciente)
    {
        idActualizacion = idPaciente;
        Registrado = true;
        cleanAndPrepare();
        fillInfo();
    }

    public void NuevoRegistro()
    {
        Registrado = false;
        cleanAndPrepare();
    }

    public void cleanAndPrepare()
    {

        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);
        
        //Limpiando campos
        Debug.Log("Refrescando valores");
        nombre.text = "";
        edad.text = "";
        mujer.isOn = true;
        hombre.isOn = false;
        datosEmergencia.text = "";
        datosMedico.text = "";
        datosSeguro.text = "";

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
            Debug.Log("No se ha inicializado correctamente la base de datos");
            this.gameObject.SetActive(false);
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
            infoWindow.ShowInfo("Error","No se ha encontrado el paciente a editar", ref eventReturn2Menu,ref actual);
        }
        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        nombre.text = consulta.Rows[0][labelNombre].ToString();
        edad.text = consulta.Rows[0][labelEdad].ToString();
        datosMedico.text = consulta.Rows[0][labelDatosMedico].ToString();
        datosEmergencia.text = consulta.Rows[0][labelDatosEmergencia].ToString();
        datosSeguro.text = consulta.Rows[0][labelSeguro].ToString();
        if (Convert.ToInt32(consulta.Rows[0][labelSexo])==1)
        {
            hombre.isOn = true;
            mujer.isOn = false;
        }
        switch (consulta.Rows[0][labelTipoDeSangre])
        {
            case "A+":
                tiposDeSangre.value = 0;
                break;
            case "A-":
                tiposDeSangre.value = 1;
                break;
            case "B+":
                tiposDeSangre.value = 2;
                break;
            case "B-":
                tiposDeSangre.value = 3;
                break;
            case "AB+":
                tiposDeSangre.value = 4;    
                break;
            case "AB-":
                tiposDeSangre.value = 5;
                break;
            case "O+":
                tiposDeSangre.value = 6;
                break;
            case "O-":
                tiposDeSangre.value = 7;
                break;
            default:
                Debug.Log("No se ha encontrado el tipo de sangre correspondiente");
                break;

        }
    }

    void validation()
    {
        Debug.Log("Iniciando la revision de datos");
        
        nombre.text = sqlcon.RemoveSpecialCharacters(nombre.text);
        if (nombre.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce el nombre del paciente",ref eventDontDoNothing,ref actual);
            return;
        } 
        
        edad.text = edad.text.Trim();
        int intEdad = 0;
        if (!int.TryParse(edad.text, out intEdad))
        {
            edad.text = "";
            Debug.Log("Registra correctamente la edad");
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Por favor introduce una edad válida",ref eventDontDoNothing,ref actual);
            return;
        }
        if (mujer.isOn == false && hombre.isOn == false)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","No se ha seleccionado unicamente un genero",ref eventDontDoNothing, ref actual);
            return;
        }
        
        var cleanText=sqlcon.RemoveSpecialCharacters(datosMedico.text);
        datosMedico.text = cleanText;
        if (obligatorioDatosMedico && cleanText.Length==0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Es necesario indicar los datos de contacto del médico",ref eventDontDoNothing, ref actual);
            return;
        }

        cleanText = sqlcon.RemoveSpecialCharacters(datosEmergencia.text.Trim());
        datosEmergencia.text = cleanText;
        if (obligatorioDatosEmergencia && cleanText.Length==0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Es necesario indicar los datos de contacto de emergencia",ref eventDontDoNothing, ref actual);
            return;
        }

        cleanText=sqlcon.RemoveSpecialCharacters(datosSeguro.text.Trim());
        datosSeguro.text = cleanText;
        if (obligatorioDatosServicio && cleanText.Length==0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("error","Es necesario indicar los datos del seguro",ref eventDontDoNothing, ref actual);
            return;
        }

        sexoActual = "0";
        var mensajeSexo = "mujer";
        if (hombre.isOn)
        {
            mensajeSexo = "hombre";
            sexoActual = "1";
        }

        string mensajeDePregunta =
            $"Deseas guardar el paciente: {nombre.text}, sexo: {mensajeSexo}," +
            $" edad: {edad.text},sangre: {tiposDeSangre.options[tiposDeSangre.value].text}" +
            $" medico {datosMedico.text}, contacto de emergencia: {datosEmergencia.text} y servicio: {datosSeguro.text}";
        actual.gameObject.SetActive(false);
        askWindow.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);

    }

    void Registrar()
    {

        //"Update cuidador SET nombre = @nombre, edad = @edad,sexo=@sexo where id=1";
        string cmd = $"Insert into paciente({labelNombre},{labelEdad},{labelSexo},{labelTipoDeSangre},{labelDatosMedico},{labelDatosEmergencia},{labelSeguro}) values('@nombre','@edad','@sexo','@tipoDeSangre','@medico','@emergencia','@servicio')";

        if (Registrado)
        {
            cmd=$"Update paciente set {labelNombre}='@nombre',{labelEdad}='@edad',{labelSexo}='@sexo',{labelTipoDeSangre}='@tipoDeSangre',{labelDatosMedico}='@medico',{labelDatosEmergencia}='@emergencia',{labelSeguro}='@servicio' where id='@id'";
            cmd = cmd.Replace("@id", idActualizacion);
        }
        cmd = cmd.Replace("@nombre", nombre.text);
        cmd = cmd.Replace("@edad", edad.text);
        cmd = cmd.Replace("@sexo", sexoActual);
        cmd = cmd.Replace("@tipoDeSangre", tiposDeSangre.options[tiposDeSangre.value].text);
        cmd = cmd.Replace("@medico", datosMedico.text);
        cmd = cmd.Replace("@emergencia", datosEmergencia.text);
        cmd = cmd.Replace("@servicio", datosSeguro.text);
        
        if (sqlcon.GoodInit)
        {
            Debug.Log($"Comando para SQL: {cmd}");
            if (sqlcon.ExecuteACommand(cmd))
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Exito", "Se ha guardado el cuidador con exito", eventReturn2Menu);
            }
            else
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error", "Ha habido un error al actualizar el cuidador", eventReturn2Menu);
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
