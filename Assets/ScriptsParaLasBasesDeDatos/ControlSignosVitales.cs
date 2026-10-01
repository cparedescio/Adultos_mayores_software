using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace ScriptsParaLasBasesDeDatos
{
    public class ControlSignosVitales : MonoBehaviour
    {

        [Header("Configuracion de menu")]
        public Canvas anterior;
        private Canvas actual;
        [Header("Ventanas auxiliares")] 
        public ControlDePregunta askWindow;
        public VentanaDeInformacion infoWindow;
        [Header("Configuración de campos")] 
        public TMP_InputField txtFC;
        public TMP_InputField txtFR;
        public TMP_InputField txtTA;
        public TMP_InputField txtTemperatura;
        public TMP_InputField txtGlucosa;
        public TMP_InputField txtObservaciones;
        public Button btnRegistrar;
        
        [Header("Configuración de enlaces a la base de datos")]
        private string idActualizacion = "";


        private string DatabaseName = "manual.s3db";
        private bool Registrado = false;
        private ConnectionManager sqlcon = new ConnectionManager();

        private UnityEvent eventReturn2Menu = new UnityEvent();
        private UnityEvent eventDontDoNothing = new UnityEvent();
        private UnityEvent eventRegister = new UnityEvent();

        public string id_paciente = "";
        public string id_cuidador = "";
        public DateTime fecha;

        public void Genera(string paciente,string cuidador, DateTime fechaARevisar)
        {
            id_paciente = paciente;
            id_cuidador = cuidador;
            fecha = fechaARevisar;
            
            actual = gameObject.GetComponent<Canvas>();
            anterior.gameObject.SetActive(false);
            actual.gameObject.SetActive(true);
            
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
                infoWindow.ShowInfo("Error", "Error al inicializar la base de datos", eventReturn2Menu);
                return;
            }
            txtFC.text = "";
            txtFR.text = "";
            txtTA.text = "";
            txtTemperatura.text = "";
            txtGlucosa.text = "";
            txtObservaciones.text = "";
            CheckRegister();
        }

        public void CheckRegister()
        {
            var cmdParametrized = $@"Select id, id_cuidador, FC, FR, TA, Temp, Glucosa, Nota From ChequeoDeSignos 
                WHERE id_paciente='{id_paciente}' and 
                fecha between '{fecha.FirstMomentOfDay().CreateSqlDate()}' and '{fecha.LastMomentOfDay().CreateSqlDate()}'
            ";
            Debug.Log($"Consulta Sql {cmdParametrized}");
            var data = sqlcon.makeAnSqlConsult(cmdParametrized);
            if (data.Rows.Count == 0) return;
            var actualData = sqlcon.ConvertDataTable<SignosVitales>(data)[0];
            txtFC.text = actualData.FC.ToString();
            txtFR.text = actualData.FR.ToString();
            txtTA.text = actualData.TA.ToString();
            txtTemperatura.text = actualData.Temp.ToString();
            txtGlucosa.text = actualData.Glucosa.ToString();
            txtObservaciones.text = actualData.Nota;
            id_cuidador = actualData.id_cuidador.ToString();
            idActualizacion = actualData.id.ToString();
        }

        void validation()
        {
            var TemporalFloat = 0.0f;
            if (!float.TryParse(txtFC.text, out TemporalFloat) || TemporalFloat<0)
            {
                this.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error","Revisa que la frecuencia cardiaca sea correcta",eventDontDoNothing);
                return;
            }
            if (!float.TryParse(txtFR.text, out TemporalFloat) || TemporalFloat<0)
            {
                this.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error","Revisa que la frecuencia respiratoría sea correcta",eventDontDoNothing);
                return;
            }
            if (!float.TryParse(txtTA.text, out TemporalFloat) || TemporalFloat<0)
            {                
                this.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error","Revisa que la frecuencia arterial sea correcta",eventDontDoNothing);
                return;
            }
            if (!float.TryParse(txtTemperatura.text, out TemporalFloat) || TemporalFloat<0)
            {                
                this.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error","Revisa que la temperatura sea correcta",eventDontDoNothing);
                return;
            }
            if (!float.TryParse(txtGlucosa.text, out TemporalFloat) || TemporalFloat<0)
            {                
                this.gameObject.SetActive(false);
                infoWindow.ShowInfo("Error","Revisa que la medición de la glucosa sea correcta",eventDontDoNothing);
                return;
            }
            Debug.Log("Todos los datos han sido registrados correctamente");
            var Mensaje = "guardar";
            if (Registrado)
            {
                Mensaje = "actualizar";
            }
            string mensajeDePregunta = $"Deseas {Mensaje} el registro de signos vitales";
            actual.gameObject.SetActive(false);
            askWindow.Preguntar(mensajeDePregunta, eventRegister, eventDontDoNothing);
        }

        void Registrar()
        {
            var cmd = $@"Insert into ChequeoDeSignos (id_paciente,id_cuidador,fecha, FC, FR, TA, Temp, Glucosa, Nota) "+
                      $"values('{id_paciente}','{id_cuidador}','{fecha.CreateSqlDate()}','{txtFC.text}', "+
                      $"'{txtFR.text}','{txtTA.text}','{txtTemperatura.text}','{txtGlucosa.text}','{txtObservaciones.text}')";
            if (Registrado)
            {
                cmd =
                    $@"Update ChequeoDeSignos set 
                    id_cuidador='{id_cuidador}',
                    FC='{txtFC.text}',
                    FR='{txtFR.text}',
                    TA='{txtTA.text}',
                    Temp='{txtTemperatura.text}',
                    Glucosa='{txtGlucosa.text}',
                    Nota='{sqlcon.RemoveSpecialCharacters(txtObservaciones.text)}'
                    where id='{idActualizacion}'";
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
}