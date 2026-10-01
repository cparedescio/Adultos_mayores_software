using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UI;

public class informacionDePacientes : MonoBehaviour
{
    [Header("Configuraciones paso I")]
    private string sqlObtenerPacientes = "Select id,nombre from paciente order by nombre asc";
    public TMP_Dropdown listaDePacientes;
    public Button btnEliminar;
    public Button btnAgregar;
    public Button btnModificar;
    public Button btnPlanDeTrabajo;
    
    [Header("Datos de la GUI, acerca del paciente")]
    public TextMeshProUGUI infoNombre;
    public TextMeshProUGUI infoSexo;
    public TextMeshProUGUI infoEdad;
    public TextMeshProUGUI infoMedico;
    public TextMeshProUGUI infoEmergencia;
    public TextMeshProUGUI infoSeguro;
    public TextMeshProUGUI infoTipoDeSangre;
    public Image infoPaciente;
    
    [Header("Datos del menú")]
    public Canvas actual;
    public Canvas anterior;
    
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;
    public RegistroDePaciente regPacienteWindow;
    
    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();
    private UnityEvent<int> eventChagedPacient = new MyIntEvent();
    
    
    [Header("Datos adicionales del paciente")]
    public TMP_Dropdown InformacionAdicional;
    public GameObject panelExtraInformation;
    public GameObject templateInfo;
    
    public registroEnfermedades registroEnfermedad;
    public RegistroDeIndicacionesMedicas registroIndicaciones;
    public registroDeAntecedentes registroAntedecentes;
    
    public PreguntasYRegistroDePlanDeCuidados registroDelPlanDeCuidados;
    
    public UnityEvent<int> eventoCambioDeInformacionAdicional = new MyIntEvent();
    public Button btnAddExtraInformation;

    [Header("Eventos para el registro de información adicional")]
    
    private UnityEvent<string> eventAskEliminarEnfermedad = new MyStringEvent();
    private UnityEvent<string> eventAskEliminarAntecedente =new MyStringEvent();
    private UnityEvent<string> eventAskEliminarIndicacion = new MyStringEvent();
    private UnityEvent eventEliminarInfomacionAdicional = new UnityEvent();
    private UnityEvent<string> eventActualizarEnfermedad = new MyStringEvent();
    private UnityEvent<string> eventActualizarAntecedente = new MyStringEvent();
    private UnityEvent<string> eventActualizarIndicacion = new MyStringEvent();
    

    [Header("Datos de modificacion actual para depuración")]
    public Dictionary<string, string> id_Paciente = new Dictionary<string, string>();
    public string ActiveId = "";
    
    //Ediciones y eliminaciones de informacion adicional
    public string activeModification = "";
    public string tablaAModificar = "";
    
    [Header("Configuraciónes de las bases de datos")]
    string DatabaseName = "manual.s3db";
    private ConnectionManager sqlcon = new ConnectionManager();
    private string tablaEnfermedad = "enfermedades";
    private string tablaAntcedentes = "antecedentes";
    private string tablaIndicaciones = "indicaciones_medicas";

    [CanBeNull] private string pacienteAnterior = null;
    //___________________________________________Nuevos registros
    private void nuevaEnfermedad()
    {
        registroEnfermedad.nuevo(ActiveId);
    }

    private void nuevoAntecedente()
    {
        registroAntedecentes.nuevo(ActiveId);
    }

    private void editarPlanDeTrabajo()
    {
        registroDelPlanDeCuidados.editar(ActiveId);
    }

    private void nuevaIndicacion()
    {
        registroIndicaciones.nuevo(ActiveId);
    }
    
    //___________________________________________Ediciones de los registros
    private void EditarEnfermedad(string id)
    {
        activeModification = id;
        registroEnfermedad.editar(id);
    }
    private void EditarAntecedente(string id)
    {
        activeModification = id;
        registroAntedecentes.editar(id);
        // registroWindow.editar(id);
    }

    private void EditarIndicacion(string id)
    {
        activeModification = id;
        registroIndicaciones.editar(id);
    }
    //_______________________________________________Eliminaciones de los registros
    private void askForEnfermedad(string id)
    {
        askForElimination(id,tablaEnfermedad,"la enfermedad");
    }
    private void askForIndication(string id)
    {
        askForElimination(id,tablaIndicaciones,"la indicación médica");
    }

    private void askForAntecedente(string id)
    {
        askForElimination(id,tablaAntcedentes,"el antecedente médico");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">id del registro</param>
    /// <param name="tabla">tabla de donde se borrará el registro</param>
    /// <param name="pregunta">¿Aceptas eliminiar _pregunta_: nombre_registro?</param>
    /// <param name="eliminar">Evento que elimina el registro de la base de datos</param>
    private void askForElimination(string id, string tabla, string pregunta)
    {
        activeModification = id;
        tablaAModificar = tabla;
        string cmd = $"Select nombre FROM {tabla} WHERE id='{id}'";
        var Tabla=sqlcon.makeAnSqlConsult(cmd);
        if (Tabla.Rows.Count == 0)
        {
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", $"No se ha encontrado la informacion, registro {tabla}, {id}", eventReturn2Menu);
            return;
        }
        Debug.Log("Iniciando la pregunta");
        actual.gameObject.SetActive(false);
        var nombre = Tabla.Rows[0]["nombre"].ToString();
        Debug.Log("Preguntado...");
        askWindow.Preguntar($"¿Aceptas eliminar {pregunta}: {nombre}?",eventEliminarInfomacionAdicional,eventDontDoNothing);
    }
    
    void eliminarRegistroAdicional()
    {
        pacienteAnterior = ActiveId;
        deleteRegister(activeModification,tablaAModificar);
    }
    
    
    //Zona de códigos adicionales
    void deleteRegister(string id,string tabla)
    {
        Debug.Log($"Eliminando el registro {id} de la tabla {tabla}");
        string cmd=$"DELETE FROM {tabla} WHERE id='{id.ToString()}'";
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        }
        if (sqlcon.ExecuteACommand(cmd))
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Exito", "Se han eliminado con éxito", eventDontDoNothing);
        }
        else
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "Ha habido un error al eliminar el campo", eventReturn2Menu);
        }
    }
    //_______________________________________________Al volverse activo
    private void OnEnable()
    {
        //Eliminiacion de eventos iniciales
        eventReturn2Menu.RemoveAllListeners();
        eventDontDoNothing.RemoveAllListeners();
        //Eventos de botones
        btnAgregar.onClick.RemoveAllListeners();
        btnModificar.onClick.RemoveAllListeners();
        btnEliminar.onClick.RemoveAllListeners();
        btnPlanDeTrabajo.onClick.RemoveAllListeners();
        btnAddExtraInformation.onClick.RemoveAllListeners();

        //Eventos de actualizacion
        eventChagedPacient.RemoveAllListeners();
        eventAskEliminarAntecedente.RemoveAllListeners();
        eventAskEliminarIndicacion.RemoveAllListeners();
        eventAskEliminarEnfermedad.RemoveAllListeners();
        eventEliminarInfomacionAdicional.RemoveAllListeners();
        eventActualizarAntecedente.RemoveAllListeners();
        eventActualizarEnfermedad.RemoveAllListeners();
        eventActualizarIndicacion.RemoveAllListeners();
        
        //Botones adicionales
        listaDePacientes.onValueChanged.RemoveAllListeners();
        InformacionAdicional.onValueChanged.RemoveAllListeners();
        
        //-----------------------Se agregan los eventos
        //Eventos del menú 2
        eventReturn2Menu.AddListener(return2menu);
        eventDontDoNothing.AddListener(notDoNothing);
        
        //Eventos del registor de un paciente 4
        btnAgregar.onClick.AddListener(registrarUnPaciente);
        btnModificar.onClick.AddListener(ModificarPaciente);
        btnEliminar.onClick.AddListener(DeleteAPacient);
        eventChagedPacient.AddListener(changedPacient);

        //Eventos para la información adicional Pendientes de revisión 5
        eventAskEliminarAntecedente.AddListener(askForAntecedente);
        eventAskEliminarIndicacion.AddListener(askForIndication);
        eventAskEliminarEnfermedad.AddListener(askForEnfermedad);
        eventEliminarInfomacionAdicional.AddListener(eliminarRegistroAdicional);
        
        //Eventos para la modificacion de registros 4
        eventActualizarAntecedente.AddListener(EditarAntecedente);
        eventActualizarEnfermedad.AddListener(EditarEnfermedad);
        eventActualizarIndicacion.AddListener(EditarIndicacion);
        btnPlanDeTrabajo.onClick.AddListener(editarPlanDeTrabajo);
        
        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            this.gameObject.SetActive(false);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        };
        //Habrá pacientes registrados...
        var pacientes=sqlcon.makeAnSqlConsult(sqlObtenerPacientes);
        if (pacientes.Rows.Count == 0)
        {
            pacienteAnterior = null;
            //No existen pacientes registrados
            //infoWindow.ShowInfo("Advertencia","No se han registrado todavía pacientes",ref eventDontDoNothing,ref actual);
            btnEliminar.gameObject.SetActive(false);
            btnModificar.gameObject.SetActive(false);
            btnPlanDeTrabajo.gameObject.SetActive(false);
            btnAddExtraInformation.gameObject.SetActive(false);

            infoPaciente.gameObject.SetActive(false);
            listaDePacientes.gameObject.SetActive(false);
        }
        else
        {
            btnEliminar.gameObject.SetActive(true);
            btnModificar.gameObject.SetActive(true);
            btnPlanDeTrabajo.gameObject.SetActive(true);
            
            infoPaciente.gameObject.SetActive(true);
            listaDePacientes.gameObject.SetActive(true);
            id_Paciente.Clear();
            listaDePacientes.options.Clear();
            for (int i = 0; i < pacientes.Rows.Count; i++)
            {
                var Nombre = pacientes.Rows[i][1].ToString();
                var id = pacientes.Rows[i][0].ToString();
                id_Paciente.Add(id,Nombre);
                listaDePacientes.options.Add(new TMP_Dropdown.OptionData(Nombre));
            }

            var indiceDeInicio = 0;
            if (pacienteAnterior != null)
            {
                if (id_Paciente.Keys.Contains(pacienteAnterior))
                {
                    ActiveId = pacienteAnterior;
                    indiceDeInicio=id_Paciente.Keys.ToList().IndexOf(ActiveId);
                }
            }
            
            listaDePacientes.value = indiceDeInicio;
            listaDePacientes.RefreshShownValue();
            ActiveId = pacientes.Rows[indiceDeInicio][0].ToString();
            listaDePacientes.onValueChanged.AddListener(changedPacient);
            InformacionAdicional.onValueChanged.AddListener(seCambioDeInformacionAdicional);
            btnAddExtraInformation.gameObject.SetActive(true);
            changedPacient(indiceDeInicio);
        }
    }

    public void changedPacient(int index)
    {
        Debug.Log($"Selected patiend: "+index);
        ActiveId = (id_Paciente.Keys.ToList())[index];
        pacienteAnterior = ActiveId;
        reloadInfoWithSelection();
        seCambioDeInformacionAdicional();
    }

    private void seCambioDeInformacionAdicional(int newSelection)
    {
        Debug.Log($"Se ha cambiado a la opción {newSelection}");
        var id_paciente = ActiveId;
        string tablaSeleccionada = tablaEnfermedad;
        
        UnityEvent<string> QueSeActualiza = eventActualizarEnfermedad;
        UnityEvent<string> QueSeElimina = eventAskEliminarEnfermedad;
        
        btnAddExtraInformation.onClick.RemoveAllListeners();
        switch (newSelection)
        {
            case 1:
                tablaSeleccionada = tablaIndicaciones;
                QueSeActualiza = eventActualizarIndicacion;
                QueSeElimina = eventAskEliminarIndicacion;
                btnAddExtraInformation.onClick.AddListener(nuevaIndicacion);
                break;
            case 2:
                tablaSeleccionada = tablaAntcedentes;
                QueSeActualiza = eventActualizarAntecedente;
                QueSeElimina = eventAskEliminarAntecedente;
                btnAddExtraInformation.onClick.AddListener(nuevoAntecedente);
                break;
            default:
                btnAddExtraInformation.onClick.AddListener(nuevaEnfermedad);
                break;
            
        }
        var sqlConsulta = $"Select * from {tablaSeleccionada} where id_paciente={id_paciente}";
        if (newSelection == 2)
        {
            sqlConsulta = $"Select id, cast(fecha as varchar) as fecha, nombre from {tablaSeleccionada} where id_paciente={id_paciente}";
        }
        Debug.Log($"Se ha consultado {sqlConsulta}");
        var resConsulta=sqlcon.makeAnSqlConsult(sqlConsulta);
        if (resConsulta.Rows.Count == 0)
        {
            panelExtraInformation.gameObject.SetActive(false);
        }
        else
        {
            panelExtraInformation.gameObject.SetActive(true);
            var numberOfSons = panelExtraInformation.transform.childCount;
            for (int i = 0; i < numberOfSons; i++)
            {
                if (panelExtraInformation.transform.GetChild(i).name != "template")
                {
                    Destroy(panelExtraInformation.transform.GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < resConsulta.Rows.Count; i++)
            {
                var id = resConsulta.Rows[i]["id"].ToString();
                var nombre = resConsulta.Rows[i]["nombre"].ToString();
                if (newSelection == 2)
                {
                    nombre = resConsulta.Rows[i]["nombre"].ToString() + ": " +
                             resConsulta.Rows[i]["fecha"].ToString();
                }
                var children=Instantiate(templateInfo, panelExtraInformation.transform);
                var info=children.GetComponent<campoInfo>();
                info.Initialize(id,nombre,QueSeElimina,QueSeActualiza);
                info.gameObject.SetActive(true);
            }
        }
    }
    
    private void seCambioDeInformacionAdicional()
    {
        switch (InformacionAdicional.captionText.text)
        {
            case "Antecedentes":
                seCambioDeInformacionAdicional(1);
                break;
            case "indicaciones_medicas":
                seCambioDeInformacionAdicional(2);
                break;
            default:
                seCambioDeInformacionAdicional(0);
                break;
        }
    }
    private void ModificarPaciente()
    {
        var pacienteSelecionado =
            id_Paciente.Where(pair => pair.Value == listaDePacientes.options[listaDePacientes.value].text).Select(pair=>pair.Key).First();
        regPacienteWindow.EditarPaciente(pacienteSelecionado);
    }
    private void registrarUnPaciente()
    {
        regPacienteWindow.NuevoRegistro();
    }
    private void reloadInfoWithSelection()
    {
        var idPaciente = ActiveId;
        string consultaEdicion = regPacienteWindow.consultaEdicion;
        var cmdParametrized = consultaEdicion.Replace("@id", idPaciente);
        var consulta=sqlcon.makeAnSqlConsult(cmdParametrized);
        Debug.Log("IniciandoLaConsulta");
        if (consulta.Rows.Count == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","No se ha encontrado el paciente a editar", ref eventReturn2Menu,ref actual);
            return;
        }
        infoNombre.text = $"Nombre: {consulta.Rows[0][regPacienteWindow.labelNombre].ToString()}";
        infoEdad.text=$"Nombre: {consulta.Rows[0][regPacienteWindow.labelEdad].ToString()}";
        infoTipoDeSangre.text=$"Tipo de sangre: {consulta.Rows[0][regPacienteWindow.labelTipoDeSangre].ToString()}";
        infoSexo.text="Sexo: Mujer";
        if (Convert.ToInt32(consulta.Rows[0][regPacienteWindow.labelSexo])==1)
        {
            infoSexo.text="Sexo: Hombre";
        }
        infoMedico.text=$"Médico: {consulta.Rows[0][regPacienteWindow.labelDatosMedico].ToString()}";
        infoEmergencia.text=$"Emergencia: {consulta.Rows[0][regPacienteWindow.labelDatosEmergencia].ToString()}";
        infoSeguro.text=$"Emergencia: {consulta.Rows[0][regPacienteWindow.labelDatosEmergencia].ToString()}";
        
        //Next round update the SQL requests to end the requesters
    }
    void notDoNothing()
    {
        seCambioDeInformacionAdicional();
        actual.gameObject.SetActive(true);
    }
    private void return2menu()
    {
        actual.gameObject.SetActive(false);
        anterior.gameObject.SetActive(true);
    }

    public void DeleteAPacient()
    {
        var pacienteSelecionado =
            id_Paciente.Where(pair => pair.Value == listaDePacientes.options[listaDePacientes.value].text).Select(pair=>pair.Key).First();
        List<string> cmds = new List<string>();
        
        //Partial SQL process to delete all related
        string cmdDeletePaciente=$"Delete From paciente WHERE id='{pacienteSelecionado}'";
        string cmdDeleteSicksReleated = $"Delete from enfermedades where id_paciente='{pacienteSelecionado}'";
        string cmdDeleteAntedentes=$"Delete from antecedentes where id_paciente='{pacienteSelecionado}'";
        string cmdDeleteIndicaciones=$"Delete from plan_trabajo where id_paciente='{pacienteSelecionado}'";
        string cmdDeleteBitacora = $"Delete from BitacoraDeActividades where id_paciente={pacienteSelecionado}";
        string cmdDeleteSignos=$"Delete from ChequeoDeSignos where id_paciente={pacienteSelecionado}";
        //Chain of elimination, add one if some table is added
        cmds.Add(cmdDeletePaciente);
        cmds.Add(cmdDeleteSicksReleated);
        cmds.Add(cmdDeleteAntedentes);
        cmds.Add(cmdDeleteIndicaciones);
        cmds.Add(cmdDeleteBitacora);
        cmds.Add(cmdDeleteSignos);
        foreach (var cmd in cmds)
        {
            if (sqlcon.GoodInit)
            {
                Debug.Log($"Comando para SQL: {cmd}");
                if (!sqlcon.ExecuteACommand(cmd))
                {
                    actual.gameObject.SetActive(false);
                    infoWindow.ShowInfo("Error", "Ha habido un error al eliminar el paciente", eventDontDoNothing);
                    return;
                }
            }
            else
            {
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error", "No se ha inicializado correctamente la base de datos", eventReturn2Menu);
                Debug.Log("No se ha inicializado correctamente la base de datos");
                return;
            }
        }
        actual.gameObject.SetActive(false);
        infoWindow.ShowInfo("Exito", "Se ha eliminador el paciente con éxito", eventReturn2Menu);
    }
}
