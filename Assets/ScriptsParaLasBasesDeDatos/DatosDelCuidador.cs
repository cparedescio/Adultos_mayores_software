using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DatosDelCuidador : MonoBehaviour
{
    [Header("Datos para el ajuste de la jerarquía del menú")]
    public Canvas Menu;
    public Canvas thisCanvas;
    
    [Header("Configuraciones para la carga del nombre y la edad")]
    public TMP_InputField Nombre;
    public TMP_InputField Edad;
    
    public Toggle SexoMujer;
    public Toggle SexoHombre;

    public TMP_InputField correo;

    [Header("Configuraciones de la informacion del cuidador")]
    public string TablaSql = "cuidador";
    public int EdadinicialDelEdades = 20;
    
    private string identificador = "";
    private string campoID = "id";
    private string campoNombre = "nombre";
    private string campoEdad = "edad";
    private string campoSexo = "sexo";
    private string campoCorreo="correo";

    public string Consulta = "Select * from cuidador";
    
    private string conn, sqlQuery;
    string DatabaseName = "manual.s3db";

    public bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    public Button btnUpdate;

    public VentanaDeInformacion infoWindows;
    public ControlDePregunta askWindows;

    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    private UnityEvent eventRegister = new UnityEvent();

    private string sexoActual = "0";
    
    void Start()
    {
        eventReturn2Menu.AddListener(return2menu);
        eventDontDoNothing.AddListener(notDoNothing);
        eventRegister.AddListener(Registrar);
        btnUpdate.onClick.AddListener(validation);
        RefreshValues();
    }

    private void OnEnable()
    {
        RefreshValues();
    }

    void RefreshValues()
    {
        Debug.Log("Refrescando valores");
        Edad.text = "";
        Nombre.text = "";
        SexoMujer.isOn = true;
        SexoHombre.isOn = false;
        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindows.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        };
        var consultaCuidador=sqlcon.makeAnSqlConsult(Consulta);
        Debug.Log("IniciandoLaConsulta");
        if (consultaCuidador.Rows.Count <= 0) return;
        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        identificador = consultaCuidador.Rows[0][campoID].ToString();
        Nombre.text = consultaCuidador.Rows[0][campoNombre].ToString();
        if (Convert.ToInt32(consultaCuidador.Rows[0][campoSexo])==1)
        {
            SexoHombre.isOn = true;
            SexoMujer.isOn = false;
        }
        Edad.text=consultaCuidador.Rows[0][campoEdad].ToString();
        correo.text = consultaCuidador.Rows[0][campoCorreo].ToString();
    }
    

    void validation()
    {
        Debug.Log("Iniciando la revision de datos");
        Edad.text = Edad.text.Trim();
        int intEdad = 0;
        if (!int.TryParse(Edad.text, out intEdad))
        {
            Edad.text = "";
            Debug.Log("Registra correctamente la edad");
            this.gameObject.SetActive(false);
            infoWindows.ShowInfo("error","Por favor introduce una edad válida",ref eventDontDoNothing,ref thisCanvas);
            return;
        }
        Nombre.text = Nombre.text.Trim();
        Nombre.text = sqlcon.RemoveSpecialCharacters(Nombre.text);
        if (Nombre.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindows.ShowInfo("error","Por favor introduce el nombre del cuidador",ref eventDontDoNothing,ref thisCanvas);
            return;
        }

        if (SexoMujer.isOn == false && SexoHombre.isOn == false)
        {
            this.gameObject.SetActive(false);
            infoWindows.ShowInfo("error","No se ha seleccionado unicamente un genero",ref eventDontDoNothing, ref thisCanvas);
            return;
        };

        sexoActual = "0";
        var mensajeSexo = "mujer";
        if (SexoHombre.isOn)
        {
            mensajeSexo = "hombre";
            sexoActual = "1";
        }

        correo.text = correo.text.Trim();
        correo.text = sqlcon.RemoveSpecialCharacters(correo.text);
        if (correo.text.Length == 0)
        {
            this.gameObject.SetActive(false);
            infoWindows.ShowInfo("error","Por favor introduce el nombre del cuidador",ref eventDontDoNothing,ref thisCanvas);
            return;
        }
        string mensajeDePregunta = $"Deseas guardar el cuidador: {Nombre.text}, sexo: {mensajeSexo} y de edad: {Edad.text}.";
        thisCanvas.gameObject.SetActive(false);
        askWindows.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);

    }
    

    void notDoNothing()
    {
        thisCanvas.gameObject.SetActive(true);
    }

    void Registrar()
    {
        //"Update cuidador SET nombre = @nombre, edad = @edad,sexo=@sexo where id=1";
        string cmd = $"Insert into cuidador({campoNombre},{campoEdad},{campoSexo},{campoCorreo}) values('@nombre','@edad','@sexo','@email')";

        if (Registrado)
        {
            cmd=$"Update cuidador set {campoNombre}='@nombre',{campoEdad}='@edad',{campoSexo}='@sexo',{campoCorreo}='@email' where id='@id'";
            cmd = cmd.Replace("@id", identificador);
        }
        Debug.Log($"Comando original: {cmd}");
        cmd = cmd.Replace("@nombre", Nombre.text);
        cmd = cmd.Replace("@edad", Edad.text);
        cmd = cmd.Replace("@sexo", sexoActual);
        cmd = cmd.Replace("@email", correo.text);
        if (sqlcon.GoodInit)
        {
            Debug.Log($"Comando para SQL: {cmd}");
            if (sqlcon.ExecuteACommand(cmd))
            {
                thisCanvas.gameObject.SetActive(false);
                infoWindows.ShowInfo("Exito", "Se ha guardado el cuidador con exito", eventReturn2Menu);
            }
            else
            {
                thisCanvas.gameObject.SetActive(false);
                infoWindows.ShowInfo("Error", "Ha habido un error al actualizar el cuidador", eventReturn2Menu);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
            infoWindows.ShowInfo("Error", "No se ha inicializado correctamente la base de datos", eventReturn2Menu);
            Debug.Log("No se ha inicializado correctamente la base de datos");
        }
    }
    private void return2menu()
    {
        thisCanvas.gameObject.SetActive(false);
        Menu.gameObject.SetActive(true);
    }
}
