using System;
using System.Collections.Generic;
using ScriptsParaLasBasesDeDatos;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class PreguntasYRegistroDePlanDeCuidados : MonoBehaviour
{
    [Header("Datos de la GUI")]
    public TextMeshProUGUI CategoriaActual;
    public GameObject panelDePreguntas;
    public GameObject template;
    
    public Button btnSiguiente;
    public Button btnVolver;
    public Button btnSalirDelRegistro;  //Para preguntar si se desea salir, y no existan salidas innoportunas
    
    [Header("Datos del menú")]
    public Canvas actual;
    public Canvas anterior;
    
    [Header("Ventanas auxiliares")] 
    public ControlDePregunta askWindow;
    public VentanaDeInformacion infoWindow;

    private UnityEvent eventReturn2Menu=new UnityEvent();
    private UnityEvent eventDontDoNothing=new UnityEvent();

    [Header("Configuraciónes de las bases de datos")]
    string DatabaseName = "manual.s3db";
    private ConnectionManager sqlcon = new ConnectionManager();
    public string id_paciente="";
    
    //ConsultaSql
    private string sqlObtenerCategoriasPreguntas=@"SELECT manual.id as id, pregunta.pregunta, menu.categoria as categoria
    FROM manual
        INNER JOIN menu 
    ON menu.id=manual.menu
        INNER JOIN pregunta
    on pregunta.id=manual.id_pregunta";
    private string sqlObtenerActividadesRegistradas = @"Select * from plan_trabajo where id_paciente='@id' and id_actividad='@actividad'";
    
    private string clearOldPlan = @"Delete from plan_trabajo where id_paciente='@id'";
    private string registerPlan="Insert into plan_trabajo(id_paciente,id_actividad) values('@id','@actividad')";
    
    //Variables de control
    private int indexActive = 0;
    private int maxIndex = 0;
    private string actualCategory = "";
    
    //Eventos para el control de las preguntas
    private IDActivityEvent CambiadoUnToggle = new IDActivityEvent();
    private UnityEvent EventaskForExit = new UnityEvent();
    private UnityEvent EventaskForUpdate = new UnityEvent();
    private UnityEvent event_UpdatePlan = new UnityEvent();
    
    //Structure for simplify the process Category, Question, activity_id, value
    public Dictionary<string, Dictionary<string, Dictionary<Int64, bool>>> actualPlan =
        new Dictionary<string, Dictionary<string, Dictionary<Int64, bool>>>();

    public bool itNeedsToReload = true;
    public void registroDePlanDeTrabajo(string idpaciente)
    {
        id_paciente = idpaciente;
        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);
        indexActive = 0;
    }
    
//Categoria, pregunta, IDs, valores referentes a los IDs.

    public void changePlan(Tuple<string, bool> infoPlan)
    {
        Debug.Log($"Valor a buscar en el diccionario: {actualCategory},{infoPlan.Item1}:{infoPlan.Item2}");
        
        var actividadesACambiar=(actualPlan[actualCategory])[infoPlan.Item1].Keys.ToList();
        foreach (var t in actividadesACambiar)
        {
            (actualPlan[actualCategory])[infoPlan.Item1][t]=
                infoPlan.Item2;
        }
    }

    public void editar(string id_pac)
    {
        //Ajustes para el control de los pacientes
        id_paciente = id_pac;
        anterior.gameObject.SetActive(false);
        actual.gameObject.SetActive(true);
        
        //Eliminiacion de eventos iniciales
        eventReturn2Menu.RemoveAllListeners();
        eventDontDoNothing.RemoveAllListeners();
        event_UpdatePlan.RemoveAllListeners();
        EventaskForUpdate.RemoveAllListeners();
        
        btnSiguiente.onClick.RemoveAllListeners();
        btnVolver.onClick.RemoveAllListeners();
        btnSalirDelRegistro.onClick.RemoveAllListeners();
        CambiadoUnToggle.RemoveAllListeners();
        
        //Eventos del menú
        eventReturn2Menu.AddListener(return2menu);
        eventDontDoNothing.AddListener(notDoNothing);
        CambiadoUnToggle.AddListener(changePlan);
        
        //Botones relativos para el avance de los elementos del menu
        btnSalirDelRegistro.onClick.AddListener(askForExit);
        btnSiguiente.onClick.AddListener(NextWindow);
        btnVolver.onClick.AddListener(BackWindow);
        
        //Evento para la actualizacion del plan de cuidados
        event_UpdatePlan.AddListener(UpdatePlan);
        
        sqlcon.Initialize(DatabaseName, true);
        if (!sqlcon.GoodInit)
        {
            Debug.Log("No se ha inicializado correctamente la base de datos");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        };
        //Si estará bien configurada la tabla del paciente
        var registroCategoriasPreguntas=sqlcon.makeAnSqlConsult(sqlObtenerCategoriasPreguntas);
        if (registroCategoriasPreguntas.Rows.Count == 0)
        {
            Debug.Log("Error apuntando a la tabla de actividades");
            infoWindow.ShowInfo("Error","Error al inicializar la base de datos",eventReturn2Menu);
            return;
        }

        Debug.Log("Conversión");
        var listaDeIDsActividaes = sqlcon.ConvertDataTable<idPreguntaCatActividad>(registroCategoriasPreguntas);
        var categoriasUnicas = (from v in listaDeIDsActividaes select v.categoria).Distinct().ToList();
        maxIndex=categoriasUnicas.Count-1;
        Debug.Log("Conversión finalizada");
        //Map the registers to Dictionary<Category,Dictionary<Question,Dictionary<id,bool>>>
        actualPlan = new Dictionary<string, Dictionary<string, Dictionary<Int64, bool>>>();
        
        foreach (var v in categoriasUnicas) //Separacion de categorias
        {
            var category = v;
            var infoCategoria = listaDeIDsActividaes.Where(u => u.categoria == v).ToList();
            var uniqueQuestionsByCategory = (from u in infoCategoria select u.pregunta).Distinct().ToList();

            var questionsActivities = new Dictionary<string, Dictionary<Int64, bool>>();
            foreach (var w in uniqueQuestionsByCategory)
            {
                var actualQuestion = w;
                var idsByQuestion = infoCategoria.Where(u => u.pregunta == w).Select(u=> u.id).ToList();
                var activityStatus = new Dictionary<Int64, bool>();
                foreach (var x in idsByQuestion)
                {
  
                    var consultaSql = sqlObtenerActividadesRegistradas.Replace("@id", id_paciente);
                    consultaSql = consultaSql.Replace("@actividad", x.ToString());
                    Debug.Log(consultaSql);
                    var data=sqlcon.makeAnSqlConsult(consultaSql);
                    var isRegistered = data.Rows.Count;
                    Debug.Log($"Se han encontrado: {isRegistered}");
                    activityStatus.Add(x,isRegistered> 0);
                }
                questionsActivities.Add(actualQuestion,activityStatus);
            }
            actualPlan.Add(category,questionsActivities);
        }
        indexActive = 0;
        DisplayData();
    }
    
    void NextWindow()
    {
        indexActive++;
        DisplayData();
    }

    void BackWindow()
    {
        indexActive--;
        if (indexActive < 0)
        { 
            indexActive = 0;
        }
        DisplayData();
    }

    void DisplayData()
    {
        actualCategory = actualPlan.Keys.ToList()[indexActive];
        CategoriaActual.text = actualCategory;
        
        //Boton para regresar a la categoria anterior
        btnVolver.onClick.RemoveAllListeners();
        btnVolver.onClick.AddListener(BackWindow);
        btnVolver.gameObject.SetActive(indexActive != 0);
        
        //Boton para la siguiente categoria
        btnSiguiente.onClick.RemoveAllListeners();

        if (indexActive == maxIndex)
        {
            btnSiguiente.onClick.AddListener(askForUpdate);
            btnSiguiente.GetComponentInChildren<Text>().text = "Actualizar";
        }
        else
        {
            btnSiguiente.GetComponentInChildren<Text>().text = "Siguiente";
            btnSiguiente.onClick.AddListener(NextWindow);
        }
        
        //Limpieza de preguntas
        panelDePreguntas.gameObject.SetActive(true);
        var numberOfSons = panelDePreguntas.transform.childCount;
        for (int i = 0; i < numberOfSons; i++)
        {
            if (panelDePreguntas.transform.GetChild(i).name != "template")
            {
                Destroy(panelDePreguntas.transform.GetChild(i).gameObject);
            }
        }
        //Obtenemos la informacion de la categoría actual ... 
        var actualKey = actualPlan.Keys.ToList()[indexActive];
        var infoSelectedCategory = actualPlan[actualKey];

        //Ahora vamos a generar los nuevos componentes que contendrán la información de las preguntas
        for (int i = 0; i < infoSelectedCategory.Keys.Count; i++)
        {
            var question = infoSelectedCategory.Keys.ToList()[i];
            var Ids = infoSelectedCategory[question];
            
            var children=Instantiate(template, panelDePreguntas.transform);
            var info=children.GetComponent<PreguntaID>();
            info.SetInformation(question,Ids,CambiadoUnToggle);
            info.gameObject.SetActive(true);
        }
    }
    void askForUpdate()
    {
        actual.gameObject.SetActive(false);
        askWindow.Preguntar($"¿Deseas actualizar el plan de cuidado?",event_UpdatePlan,eventDontDoNothing);
    }

    void UpdatePlan()
    {
        //Delete the old plan
        var eliminacion = clearOldPlan.Replace("@id", id_paciente);
        if (sqlcon.GoodInit)
        {
            //Eliminación del plan anterior
            Debug.Log($"Comando para SQL: {eliminacion}");
            if (sqlcon.ExecuteACommand(eliminacion))
            {
                //Registro de cada categoria
                foreach (var category in actualPlan.Keys)
                {
                    //Preguntas de cada categoria
                    foreach (var question in actualPlan[category].Keys)
                    {
                        //Actividades relacionadas a cada actividad
                        foreach (var activity in (actualPlan[category])[question])
                        {
                            //Si en el plan de trabajo se ha marcado como buena
                            if (activity.Value)
                            {
                                //Se registra el ID de la actividad de uno por uno
                                var actualRegister = registerPlan.Replace("@id", id_paciente);
                                actualRegister = actualRegister.Replace("@actividad", activity.Key.ToString());
                                Debug.Log("Registrar actividad");
                                if (!sqlcon.ExecuteACommand(actualRegister))
                                {
                                    //Error Sql
                                    actual.gameObject.SetActive(false);
                                    infoWindow.ShowInfo("Error","Error registrando el plan de trabajo",eventReturn2Menu);
                                    return;
                                }
                            }
                        }
                    }
                }
                //Si todo paso bien
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Exito", "Se ha guardado la indicación médica con éxito", eventReturn2Menu);
                return;
            }
            //Si hubo un error al eliminar el registro
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "Ha habido un error", eventReturn2Menu);
            return;
        }
        //Si no se ha inicializado correctamente la base de datos
        infoWindow.ShowInfo("Error", "No se ha inicializado correctamente la base de datos", eventReturn2Menu);
        Debug.Log("No se ha inicializado correctamente la base de datos");
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

    private void askForExit()
    {
        actual.gameObject.SetActive(false);
        askWindow.Preguntar($"¿Deseas salir?",eventReturn2Menu,eventDontDoNothing);
    }

}


