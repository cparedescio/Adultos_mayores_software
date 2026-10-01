using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ScriptsParaLasBasesDeDatos;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Linq;


public class controlDeBitacora : MonoBehaviour
{
    public TMP_Dropdown selectorAño;
    public TMP_Dropdown selectorMes;
    public TMP_Dropdown selectorPaciente;
    public DetalleDelDia registroDiario;

    public Canvas anterior;
    private Canvas actual;

    private DateTime fechaActual;

    private List<string> meses = new List<string>()
    {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre",
        "Diciembre"
    };
    
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;

    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();

    [Header("Configuraciónes de las bases de datos")]
    string DatabaseName = "manual.s3db";
    private ConnectionManager sqlcon = new ConnectionManager();
    private string GetPacientes = "Select id, nombre from paciente";
    private string GetCuidador = "Select id, Nombre from Cuidador";
    
    [Header("Configuraciones del calendario")]
    private int yearsBetween = 30;
    public ControladorCalendario calendario;

    private List<PacienteID> pacientesRegistrados = new List<PacienteID>();
    private Cuidador cuidadorDePacientes = new Cuidador();

    private void AjustesDeInicio()
    {
        actual = this.gameObject.GetComponent<Canvas>();
        fechaActual=DateTime.Now;

        selectorMes.options.Clear();
        foreach (var VARIABLE in meses)
        {
            selectorMes.options.Add(new TMP_Dropdown.OptionData(VARIABLE));
        }

        selectorMes.value = fechaActual.Month - 1;
        selectorMes.RefreshShownValue();
        selectorAño.options.Clear();
        var actualYear = fechaActual.Year;
        for (int i = 0; i <= yearsBetween; i++)
        {
            selectorAño.options.Add(new TMP_Dropdown.OptionData((actualYear-i).ToString()));
        }

        selectorAño.value = 0;
        selectorAño.RefreshShownValue();
    }
    
    public void openBitacora()
    {
        AjustesDeInicio();
        
        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);
        selectorAño.onValueChanged.RemoveAllListeners();
        selectorMes.onValueChanged.RemoveAllListeners();
        selectorPaciente.onValueChanged.RemoveAllListeners();
        
        selectorAño.onValueChanged.AddListener(CambioDeDatos);
        selectorMes.onValueChanged.AddListener(CambioDeDatos);
        selectorPaciente.onValueChanged.AddListener(CambioDeDatos);
        
        eventReturn2Menu.RemoveAllListeners();
        eventReturn2Menu.AddListener(return2menu);
        
        eventDontDoNothing.RemoveAllListeners();
        eventDontDoNothing.AddListener(notDoNothing);
        

        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        }
        //Informacion de pacientes registrados actualmente
        var data = sqlcon.makeAnSqlConsult(GetPacientes);
        if (data.Rows.Count == 0)
        {
            this.gameObject.SetActive(false);
            //No existen pacientes registrados
            infoWindow.ShowInfo("Sin pacientes registrados","Registra a los pacientes antes de acceder a este menú",eventReturn2Menu);
            return;
        }
        Debug.Log(sqlcon.ConvertTable2String(data));
        pacientesRegistrados = new List<PacienteID>();
        for (var i = 0; i < data.Rows.Count; i++)
        {
            var persona = new PacienteID
            {
                id = (long)data.Rows[i]["id"],
                nombre = (string) data.Rows[i]["nombre"]
            };
            pacientesRegistrados.Add(persona);
        }
        selectorPaciente.options.Clear();
        foreach (var t in pacientesRegistrados)
        {
            Debug.Log($"Se ha agregado el paciente: {t.nombre}");
            selectorPaciente.options.Add(new TMP_Dropdown.OptionData(t.nombre));
        }
        selectorPaciente.value = 0;
        selectorPaciente.RefreshShownValue();
        Debug.Log($"Se ha seleccionado el paciente {pacientesRegistrados[0].nombre}");
        //Informacion del cuidador registrado actualmente
        data = sqlcon.makeAnSqlConsult(GetCuidador);
        //Inicializacion directa, existe algun problema con la conversion directa por el momento, se buscara
        //como solucionarlo en otra ocasion.
        var newCuidador = new Cuidador()
        {
            id = (long) data.Rows[0]["id"],
            Nombre = data.Rows[0]["Nombre"].ToString()
        };
        cuidadorDePacientes = newCuidador;
        CambioDeDatos(0);
    }
    
    /// <summary>
    /// El dato de moficiacion se ignora al ser necesaria la recreacion de todo el calendario
    /// </summary>
    /// <param name="CambioDatos"></param>
    void CambioDeDatos(int CambioDatos)
    {
        //Se crea nuevamente la fecha del registro
        var Año = Convert.ToInt32(selectorAño.captionText.text);
        var Mes = selectorMes.value + 1;
        var NewDate = new DateTime(Año, Mes, DateTime.Now.Day);
        //Se obtiene el ID del paciente
        var IdPaciente = pacientesRegistrados[selectorPaciente.value].id.ToString();
        //Se obtiene el registro diario
        var idcuidador = cuidadorDePacientes.id.ToString();
        //Se vuelve a crear la consulta para generar correctamente el calendario
        Debug.Log($"Se ha generado el calendario para {NewDate} del paciente {IdPaciente}, cuidador {cuidadorDePacientes.Nombre}");
        calendario.CreateCalendar(NewDate,IdPaciente,idcuidador,registroDiario);
    }
    
    // Start is called before the first frame update

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





