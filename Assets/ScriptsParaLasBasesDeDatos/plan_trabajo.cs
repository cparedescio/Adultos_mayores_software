namespace ScriptsParaLasBasesDeDatos
{
    public class plan_trabajo
    {
        public int id { get; set; }
        public int id_paciente { get; set; }
        public int id_actividad { get; set; }

        public string addRegisterSQL()
        {
            return $"insert into plan_trabajo(id_paciente,id_actividad) values ({id_paciente},{id_actividad}";
        }

        public string deleteRegisterSQL()
        {
            return $"DELETE FROM plan_trabajo WHERE id_paciente ={id_paciente} and id_actividad ={id_actividad}";
        }
    }
}