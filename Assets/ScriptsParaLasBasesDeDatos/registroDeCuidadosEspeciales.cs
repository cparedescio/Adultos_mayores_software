using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;


public class registroDeCuidadosEspeciales : MonoBehaviour
{

    [Header("Configuracion de menu")]
    public Canvas anterior;
    public Canvas actual;
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;

    [Header("Configuración de campos")]
    //public TMP_InputField nombre;
    public TMP_Dropdown categorias;
    public TMP_Dropdown actividades;
    
    public Button btnRegistrar;
    
    [Header("Configuración de enlaces a la base de datos")]
//    private string labelNombre = "nombre";
    private string labelIdPaciente = "id_paciente";
    private string labelid_cuidado = "id_cuidado";
    
//    private string labelid = "id";
    private string idActualizacion = "";
    
    string DatabaseName = "manual.s3db";
    private bool Registrado = false;
    private ConnectionManager sqlcon = new ConnectionManager();
    
//    private string consultaEdicion = "Select * from cuidados where id='@id'";

    private string consultaCategoriaActividad = @"SELECT manual.id as id, menu.categoria as categoria, manual.Actividad as actividad
    FROM manual
    INNER JOIN menu 
    ON menu.id=manual.menu;";

    private DataTable CategoriasYActividades;
    public List<string> categoriasSeleccion=new List<string>();
    public Dictionary<string, string> actividadesId = new Dictionary<string, string>();
    public List<string> actividadesSeleccion = new List<string>();
    
    private string tabla = "cuidados";
    
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
        
        //nombre.text = "";
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
        var CategoriasYActividades=sqlcon.makeAnSqlConsult(consultaCategoriaActividad);
        if (CategoriasYActividades.Rows.Count == 0)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","No se ha encontrado la indicación médica a editar", ref eventReturn2Menu,ref actual);
        }

        categoriasSeleccion = new List<string>();
        categorias.options.Clear();
        foreach (DataRow v in (DataRowCollection)CategoriasYActividades.Rows)
        {
            if (categoriasSeleccion.Contains(v["categoria"])) continue;
            categorias.options.Add(new TMP_Dropdown.OptionData(v["categoria"].ToString()));
            categoriasSeleccion.Add(v["categoria"].ToString());
        }

        categorias.value = 0; //Esto permite inicializar el evento generado para la actualizacion de los campos
        categorias.RefreshShownValue();
        cambioDeCategoria(0);
        categorias.onValueChanged.AddListener(cambioDeCategoria);
    }

    void cambioDeCategoria(int selected)
    {
        var Catego = categoriasSeleccion[selected];
        actividadesId.Clear();
        actividadesSeleccion.Clear();
        foreach (DataRow v in CategoriasYActividades.Rows)
        {
            if (v["categoria"].ToString() == Catego)
            {
                actividades.options.Add(new TMP_Dropdown.OptionData(v["actividad"].ToString()));
                actividadesSeleccion.Add(v["actividad"].ToString());
            };
            actividadesId.Add(v["actividad"].ToString(),v["id"].ToString());
        }
        actividades.value = 0;
        actividades.RefreshShownValue();
    }
    
    /// <summary>
    /// Falta actualizar la manera para registrar la informacion
    /// </summary>
    public void fillInfo()
    {
        var SelectedCategory = "";
        var SelectedActivity = "";
        bool encontrada = false;
        foreach (DataRow v in CategoriasYActividades.Rows)
        {
            if (v["id"].ToString() == idActualizacion)
            {
                SelectedActivity = v["actividad"].ToString();
                SelectedCategory = v["categoria"].ToString();
                encontrada = true;
            };
        }
        if (encontrada == false)
        {
            this.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error","No se ha encontrado la indicación médica a editar", ref eventReturn2Menu,ref actual);
        }
        var SeletedIndexCategory = categoriasSeleccion.IndexOf(SelectedCategory);
        //Remake options and data
        categorias.value = SeletedIndexCategory;
        cambioDeCategoria(SeletedIndexCategory);
        //One that is created 
        var IndexOfActivity = actividadesSeleccion.IndexOf(SelectedActivity);
        actividades.value = IndexOfActivity;
        

        //Se ponen los datos en la GUI si esta registrado
        Registrado = true;
        //nombre.text = consulta.Rows[0][labelNombre].ToString();
        
    }
    
    void validation()
    {
        Debug.Log("Iniciando la revision de datos");
        var Mensaje = "guardar";
        if (Registrado)
        {
            Mensaje = "actualizar";
        }
        string mensajeDePregunta = $"Deseas {Mensaje} el cuidado especial: {actividades.captionText.text}";
        actual.gameObject.SetActive(false);
        askWindow.Preguntar(mensajeDePregunta,eventRegister,eventDontDoNothing);
    }
    
    void Registrar()
    {
        var cmd = $"Insert into {tabla}({labelIdPaciente},{labelid_cuidado}) values('{id_paciente}','{actividadesId[actividades.captionText.text]}')";
        if (Registrado)
        {
            cmd=$"Update {tabla} set {labelid_cuidado}='{actividadesId[actividades.captionText.text]}' where id='{idActualizacion}'";
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
