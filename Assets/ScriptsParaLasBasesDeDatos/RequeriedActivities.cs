using System;
using System.Data.SqlTypes;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.LowLevel; // Reemplaza a Experimental.PlayerLoop
using UnityEngine.UIElements; // Reemplaza a Experimental.UIElements
namespace ScriptsParaLasBasesDeDatos
{
    public class RequeriedActivities
    {
        public Int64 id { get; set; }
        public string categoria { get; set; }
        public string actividad { get; set; }
        public Int32 paciente { get; set; }
        
        private DateTime aRevisar;
        
        private bool originalState=false;
        private bool actualState = false;
        private Bitacora state = new Bitacora();
        
        private DateTime fechaARegistrar;
        public void getState(ConnectionManager sqlcon, string _paciente, DateTime fechaR)
        {
            //Faltaba referenciar antes las fechas
            aRevisar = fechaR;
            var inicioDia = aRevisar.FirstMomentOfDay();
            var final = aRevisar.LastMomentOfDay();
            string consultaActividad =
                $@"Select id from BitacoraDeActividades 
                    where id_actividad='{id}' and id_paciente='{_paciente}' and fecha BETWEEN '{inicioDia.CreateSqlDate()}' and '{final.CreateSqlDate()}'";
            Debug.Log(consultaActividad);
            var data = sqlcon.makeAnSqlConsult(consultaActividad);
            fechaARegistrar = fechaR;
            if (data.Rows.Count > 0)
            {
                state = sqlcon.ConvertDataTable<Bitacora>(data)[0];
                originalState = true;
            }
            actualState = originalState;
        }

        public bool checkAndUpload(string cuidador, ConnectionManager sqlcon)
        {
            if (actualState == originalState) return true; //Si no ha habído cambios
            if (actualState) //En el caso de que se haya activado la opcion es necesario registrarla de la siguiente manera
            {
                var commandoSql = $@"Insert into BitacoraDeActividades(id_paciente, id_cuidador, id_actividad, fecha) 
                values ('{paciente}','{cuidador}','{id}','{fechaARegistrar.CreateSqlDate()}')";
                //Insertar en la base de datos
                Debug.Log($"Comando registrar {commandoSql}");
                return sqlcon.ExecuteACommand(commandoSql);
            }
            var inicioDia = aRevisar.FirstMomentOfDay();
            var final = aRevisar.LastMomentOfDay();
            var comandoEliminar =
                $@"Delete from BitacoraDeActividades where id_actividad='{id}' and id_paciente='{paciente}' 
                    and fecha between '{inicioDia.CreateSqlDate()}' and '{final.CreateSqlDate()}'";
            Debug.Log($"CMD: {comandoEliminar}");
            return sqlcon.ExecuteACommand(comandoEliminar);
        }
        
        public bool getState()
        {
            return actualState;
        }
        public void setState(bool newValue)
        {
            actualState = newValue;
        }
    }
}