using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
<<<<<<< Updated upstream
=======
using System.Linq.Expressions;

>>>>>>> Stashed changes

namespace ScriptsParaLasBasesDeDatos
{
    public class DetalleDelDia : MonoBehaviour
    {
        [Header("Configuracion de menu")] public Canvas anterior;
        private Canvas actual;
        public ControlSignosVitales signosVitales;
        [Header("Ventanas auxiliares")] public ControlDePregunta askWindow;
        public VentanaDeInformacion infoWindow;
        [Header("Configuración de campos")]
        public TextMeshProUGUI txtNombre;
        public TextMeshProUGUI txtFecha;
        public TMP_Dropdown Categorias;
        public Button btnSalir;
        public Button btnActualizar;
        public Button btnSignosVitales;
        
        [Header("Configuración de enlaces a la base de datos")]
        private string DatabaseName = "manual.s3db";
        private ConnectionManager sqlcon = new ConnectionManager();
        public string id_paciente = "";
        private DateTime fechaRegistrar;

        private UnityEvent eventReturn2Menu = new UnityEvent();
        private UnityEvent eventDontDoNothing = new UnityEvent();
        private UnityEvent event_UpdatePlan = new UnityEvent();
        
        private string obtenerNombre = "Select nombre from paciente where id='@id'";

        private string getRequieredActivities = @"
            Select dt.id, dt.categoria, dt.actividad, plan_trabajo.id_paciente as paciente from plan_trabajo
            INNER JOIN(
	            SELECT manual.id as id, menu.categoria as categoria, manual.Actividad as actividad
                FROM manual
                INNER JOIN menu 
                ON menu.id=manual.menu
            ) dt
            on dt.id=plan_trabajo.id_actividad
            Where paciente='@id'
        ";

        public Dictionary<string, List<RequeriedActivities>> categoryActivity =
            new Dictionary<string, List<RequeriedActivities>>();
        
        public GameObject panel;
        public GameObject template;
        private EventCambioActividadDiaria cambioToggle = new EventCambioActividadDiaria();
        public string cuidadorActivo = "";
        
        public void generate(string idPaciente, string idCuidador, DateTime _fecha)
        {
            //Para la generación de los datos, faltaba referenciar la fecha a la variable
            Debug.Log($"Fecha para el día {_fecha}");
            fechaRegistrar = _fecha;
            
            actual = this.gameObject.GetComponent<Canvas>();
            cuidadorActivo = idCuidador;
            anterior.gameObject.SetActive(false);
            actual.gameObject.SetActive(true);

            eventReturn2Menu.RemoveAllListeners();
            eventDontDoNothing.RemoveAllListeners();
            event_UpdatePlan.RemoveAllListeners();
            btnActualizar.onClick.RemoveAllListeners();
            btnSalir.onClick.RemoveAllListeners();
            btnSignosVitales.onClick.RemoveAllListeners();

            eventReturn2Menu.AddListener(return2menu);
            eventDontDoNothing.AddListener(notDoNothing);
            event_UpdatePlan.AddListener(UpdatePlan);
            btnSignosVitales.onClick.AddListener(registerVitalSigns);
            
            btnActualizar.onClick.AddListener(askForUpdate);
            btnSalir.onClick.AddListener(askForExit);

            id_paciente = idPaciente;

            sqlcon.Initialize(DatabaseName, true);
            if (!sqlcon.GoodInit)
            {
                this.gameObject.SetActive(false);
                Debug.Log("No se ha inicializado correctamente la base de datos");
                infoWindow.ShowInfo("Error", "Error al inicializar la base de datos", eventReturn2Menu);
                return;
            }

            var selectedPatient = obtenerNombre.Replace("@id", idPaciente);
            var data = sqlcon.makeAnSqlConsult(selectedPatient);
            var listaNombres = sqlcon.ConvertDataTable<PacienteNombre>(data);

            //Se ha obtenido la información del paciente y la fecha actual
            txtNombre.text = listaNombres[0].nombre;
            txtFecha.text = _fecha.ToShortDateString();

            //Se obtiene la informacion de las actividades que se deben de realizar el día de hoy
            var requieredSql = getRequieredActivities.Replace("@id", id_paciente);
            data = sqlcon.makeAnSqlConsult(requieredSql);
            var pendingActivities = sqlcon.ConvertDataTable<RequeriedActivities>(data);
            if (pendingActivities.Count == 0)
            {
                Categorias.gameObject.SetActive(false);
                return;
            }

            //Se obtiene el estado inicial del registro
            foreach (var VARIABLE in pendingActivities)
            {
                VARIABLE.getState(sqlcon, idPaciente, fechaRegistrar);
            }

            //Una vez establecida esta relación se genera un diccionario 
            //Estructura String,-List-
            categoryActivity = new Dictionary<string, List<RequeriedActivities>>();
            var categorias = (from v in pendingActivities select v.categoria).Distinct();
            //Limpiamos las categorias referentes
            Categorias.onValueChanged.RemoveAllListeners();
            Categorias.options.Clear();
            foreach (var variable in categorias)
            {
                var actividadesDeCategoria = pendingActivities.Where(v => v.categoria == variable).ToList();
                categoryActivity.Add(variable, actividadesDeCategoria);
                Categorias.options.Add(new TMP_Dropdown.OptionData(variable));
            }

            Categorias.value = 0;
            Categorias.RefreshShownValue();
            Categorias.onValueChanged.AddListener(CambioDeCategoria);
            CambioDeCategoria(0);
        }

        private void registerVitalSigns()
        {
            Debug.Log($"Abriendo el registro de los signos vitales del día {fechaRegistrar.ToShortDateString()}");
            signosVitales.Genera(id_paciente,cuidadorActivo,fechaRegistrar);
        }

        private void CambioDeCategoria(int arg0)
        {
            cambioToggle.RemoveAllListeners();
            cambioToggle.AddListener(cambioDeToogleCambiarDictionary);
            
            Debug.Log($"Se ha seleccionado la categoría: {arg0}");

            //Limpieza de actividades de la categoría
            panel.gameObject.SetActive(true);
            var numberOfSons = panel.transform.childCount;
            for (int i = 0; i < numberOfSons; i++)
            {
                if (panel.transform.GetChild(i).name != "template")
                {
                    Destroy(panel.transform.GetChild(i).gameObject);
                }
            }

            //Obtenemos la informacion de la categoría actual ... 
            var actualKey = categoryActivity.Keys.ToList()[arg0];
            var infoSelectedCategory = categoryActivity[actualKey];

            foreach (var VARIABLE in infoSelectedCategory)
            {
                var children = Instantiate(template, panel.transform);
                var info = children.GetComponent<ControlDeActividadDiaria>();
                info.SetData(VARIABLE, cambioToggle);
                info.gameObject.SetActive(true);
            }
        }

        private void cambioDeToogleCambiarDictionary(Toggle2Activity arg0)
        {
            var Categoria = arg0.actividad.categoria;
            var index = (categoryActivity[Categoria]).IndexOf(arg0.actividad);
            var Actividad = (categoryActivity[arg0.actividad.categoria])[index].actividad;
            Debug.Log($"Se ha cambiando la {Categoria}:{Actividad} a {arg0.newValue}");
            (categoryActivity[arg0.actividad.categoria])[index].setState(arg0.newValue);
        }

        private void  askForUpdate()
        {
            actual.gameObject.SetActive(false);
            askWindow.Preguntar($"¿Deseas actualizar las actividades realizadas este día?", event_UpdatePlan, eventDontDoNothing);
        }
        
        private void UpdatePlan()
        {
            if (sqlcon.GoodInit)
            {
                //Debug.Log($"Las llaves del diccionario son {String.Join(",",categoryActivity.Keys.ToList())}");
                foreach (var category in categoryActivity.Keys)
                {
                    Debug.Log($"Subiendo cambios de la categoria {category}");
                    //Actividades relacionadas a cada actividad
                    foreach (var activity in (categoryActivity[category]))
                    {
                        Debug.Log($"Subiendo los cambios de {activity.actividad}");
                        if (activity.checkAndUpload(cuidadorActivo, sqlcon)) continue;
                        actual.gameObject.SetActive(false);
                        infoWindow.ShowInfo("Error", "No se ha inicializado correctamente la base de datos",
                            eventReturn2Menu);
                        return;
                    }
                }
                actual.gameObject.SetActive(false);
                infoWindow.ShowInfo("Éxito", "Se ha iniciado correctamente la base de datos", eventReturn2Menu);
                return;
            }
            actual.gameObject.SetActive(false);
            infoWindow.ShowInfo("Error", "No se ha inicializado correctamente la base de datos", eventReturn2Menu);
            Debug.Log("No se ha inicializado correctamente la base de datos");
            return;
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
            askWindow.Preguntar($"¿Deseas salir?", eventReturn2Menu, eventDontDoNothing);
        }
    }


}

