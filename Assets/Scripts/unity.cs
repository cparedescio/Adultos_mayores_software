using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using TextSpeech;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Text.RegularExpressions;

using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

//References

public class unity : MonoBehaviour
{
string query = " ";
    const string LANG_CODE = "es-MX";
    System.Random rnd = new System.Random();
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    List<Manual> resultadosBD = new List<Manual>();
    List<Procedimiento> procedimientosBD = new List<Procedimiento>();
    private IDataReader reader;
public ToggleGroup idioma;
public TMPro.TMP_Dropdown lista1;
public TMPro.TMP_Dropdown lista2;
public TMPro.TMP_Dropdown lista3;
public TMPro.TMP_Dropdown lista4;
public TMPro.TMP_Dropdown lista21;
public TMPro.TMP_Dropdown lista22;
public TMPro.TMP_Dropdown lista23;
public TMPro.TMP_Dropdown lista24;
public Text id;
public Text id2;
public Text id3;
    public Text auxiliar;
    public Text dur_aux;
    public Text mat_aux;
    public TMP_Text mensaje;
    public Text input;
    public TMP_Text  id_pasos;
public TMP_Text pasos_max;
public Text instruccion;
public TMP_Text imagen;
public Text dur;
public Text mat;
public GameObject anterios;
public GameObject siguiente;
public GameObject Final;
public GameObject UI_Portada;
public GameObject UI_Introduccion;
public GameObject UI_Modulos;
public GameObject UI_Generales;
public GameObject UI_Estimulacion;
public GameObject UI_Especificos;
public GameObject UI_Auxilios;
public GameObject UI_Cuidador;
public GameObject UI_Bitacora;
public GameObject UI_Dialog;
public GameObject UI_VentanaProcedimiento;
public GameObject UI_VentanaEvento;
public GameObject UI_PieLogos;
public GameObject UI_PieBtnVoz;
public TMP_Text titulo;
    public Image im;
    private string filepath;
    int idioma_num = 1;
    string DatabaseName = "manual.s3db";


void Start()
{
        var ddL = lista1.GetComponent<TMPro.TMP_Dropdown>();
        ddL.onValueChanged.AddListener(delegate { DropdownValueChanged(ddL); });
        var ddL2 = lista2.GetComponent<TMPro.TMP_Dropdown>();
        ddL2.onValueChanged.AddListener(delegate { DropdownValueChanged2(ddL2); });
        var ddL3 = lista3.GetComponent<TMPro.TMP_Dropdown>();
        ddL3.onValueChanged.AddListener(delegate { DropdownValueChanged3(ddL3); });
        var ddL4 = lista4.GetComponent<TMPro.TMP_Dropdown>();
        ddL4.onValueChanged.AddListener(delegate { DropdownValueChanged4(ddL4); });
        var ddL6 = lista21.GetComponent<TMPro.TMP_Dropdown>();
        ddL6.onValueChanged.AddListener(delegate { DropdownValueChangedsegundo(ddL6); });
        var ddL7 = lista22.GetComponent<TMPro.TMP_Dropdown>();
        ddL7.onValueChanged.AddListener(delegate { DropdownValueChangedsegundo(ddL7); });
        var ddL8 = lista23.GetComponent<TMPro.TMP_Dropdown>();
        ddL8.onValueChanged.AddListener(delegate { DropdownValueChangedsegundo(ddL8); });
        var ddL9 = lista24.GetComponent<TMPro.TMP_Dropdown>();
        ddL9.onValueChanged.AddListener(delegate { DropdownValueChangedsegundo(ddL9); });
       
        lista1.options.Clear();
        lista2.options.Clear();
        lista3.options.Clear();
        lista4.options.Clear();
        List<string> listOptions1 = new List<string>();
        List<string> listOptions2 = new List<string>();
        List<string> listOptions3 = new List<string>();
        List<string> listOptions4 = new List<string>();
        List<string> listOptions5 = new List<string>();
        listOptions1.Add("Seleccione la categoria");
        listOptions2.Add("Seleccione la categoria");
        listOptions3.Add("Seleccione la categoria");
        listOptions4.Add("Seleccione la categoria");
        listOptions5.Add("Seleccione la categoria");

#if (UNITY_EDITOR)
        string filepath = Application.dataPath + "/Plugins/" + DatabaseName;

        //open db connection
        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        Debug.Log(filepath);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

#endif
#if (UNITY_ANDROID)
        filepath = Application.persistentDataPath + "/" + DatabaseName;
        Debug.Log(filepath);
        if (!File.Exists(filepath))
        {
// If not found on android will create Tables and database

Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
 Application.dataPath + "!/manual");
// UNITY_ANDROID
WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/manual.s3db");
while (!loadDB.isDone) {
                titulo.text += "no pudo leer la base de datos";
            }
// then save to Application.persistentDataPath
File.WriteAllBytes(filepath, loadDB.bytes);
}
idioma = GetComponent<ToggleGroup>();

//open db connection
conn = "URI=file:" + filepath;
Debug.Log("Stablishing connection to: " + conn);
dbconn = new SqliteConnection(conn);

dbconn.Open();

        string query = " CREATE TABLE IF NOT EXISTS 'comandos' (id INTEGER NOT NULL,  id_idioma int(11) NOT NULL,  nombre text NOT NULL,  texto text NOT NULL, PRIMARY KEY('id' AUTOINCREMENT)  );   ";
        query += " CREATE TABLE IF NOT EXISTS 'duracion' (id INTEGER NOT NULL,  id_idioma int(11) NOT NULL,  id_manual int(11) NOT NULL,  Texto text NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT)  );  ";
        query += " CREATE TABLE IF NOT EXISTS 'entities' (id INTEGER NOT NULL,  name text NOT NULL,  valor int(11) NOT NULL,  id_manual int(11) NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT)  );  ";
        query += " CREATE TABLE IF NOT EXISTS 'idioma' (id INTEGER NOT NULL,  idioma text NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT)   );   ";
        query += " CREATE TABLE IF NOT EXISTS 'intent' (id INTEGER NOT NULL,  name int(11) NOT NULL,  confidence int(11) NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT)   );";
        query += " CREATE TABLE IF NOT EXISTS 'material' (id INTEGER NOT NULL,  id_idioma int(11) NOT NULL,  id_manual int(11) NOT NULL,  Texto text NOT NULL, PRIMARY KEY('id' AUTOINCREMENT)  );   ";
        query += " CREATE TABLE IF NOT EXISTS 'menu' (id INTEGER, principal INTEGER, categoria TEXT, id_idioma INTEGER, PRIMARY KEY('id' AUTOINCREMENT)   ); ";
        query += " CREATE TABLE IF NOT EXISTS 'paciente' (id INTEGER NOT NULL,  nombre TEXT NOT NULL, edad INTEGER NOT NULL, sexo INTEGER NOT NULL,  tipoDeSangre TEXT NOT NULL, medico TEXT NOT NULL, emergencia TEXT NOT NULL, servicio TEXT NOT NULL, PRIMARY KEY('id' AUTOINCREMENT)   );   ";
        query += " CREATE TABLE IF NOT EXISTS 'cuidados_especiales' (id INTEGER NOT NULL,  id_paciente INTEGER, nombre TEXT NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT)   ); ";
        query += " CREATE TABLE IF NOT EXISTS 'enfermedades' (id INTEGER NOT NULL,  id_paciente INTEGER, nombre TEXT NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT));";
        query += " CREATE TABLE IF NOT EXISTS 'indicaciones_medicas' (id INTEGER NOT NULL,  id_paciente INTEGER, nombre TEXT, PRIMARY KEY('id' AUTOINCREMENT)   );  ";
        query += " CREATE TABLE IF NOT EXISTS 'cuidador' (id INTEGER NOT NULL,  Nombre varchar(100) NOT NULL,  sexo int(11) NOT NULL,  edad INTEGER, correo varchar(100), PRIMARY KEY('id' AUTOINCREMENT)  ); ";
        query += " CREATE TABLE IF NOT EXISTS 'antecedentes' (id INTEGER NOT NULL,  id_paciente INTEGER, nombre TEXT, fecha DATE, PRIMARY KEY('id' AUTOINCREMENT)  ); ";
        query += " CREATE TABLE IF NOT EXISTS 'manual' (id INTEGER NOT NULL,  id_idioma int(11) NOT NULL,  menu INTEGER, Actividad text NOT NULL, Auxiliar text NOT NULL, pasos INTEGER NOT NULL, dur TEXT, mat NUMERIC, id_pregunta INTEGER, PRIMARY KEY('id' AUTOINCREMENT)   ); ";
        query += " CREATE TABLE IF NOT EXISTS 'pregunta' (id INTEGER NOT NULL,  pregunta TEXT, id_si INTEGER, id_no INTEGER, PRIMARY KEY('id' AUTOINCREMENT)   );";
        query += " CREATE TABLE IF NOT EXISTS 'plan_trabajo' (id INTEGER NOT NULL,  id_paciente int(11) NOT NULL, id_actividad int(11) NOT NULL, PRIMARY KEY('id' AUTOINCREMENT)   );  ";
        query += " CREATE TABLE IF NOT EXISTS 'Bitacora' (id INTEGER NOT NULL,  fecha date NOT NULL,  FC float NOT NULL, FR float NOT NULL, TA float NOT NULL, Temp float NOT NULL, Glucosa float NOT NULL, Nota TEXT NOT NULL, id_trabajo INTEGER NOT NULL,  PRIMARY KEY('id' AUTOINCREMENT) );  ";
        query += " CREATE TABLE IF NOT EXISTS 'procedimiento' (id INTEGER NOT NULL, id_idioma int(11) NOT NULL, id_manual int(11) NOT NULL, id_pasos int(11) NOT NULL, Texto text NOT NULL, Imagen text NOT NULL, PRIMARY KEY('id' AUTOINCREMENT)   ); ";
      



        try
        {
dbcmd = dbconn.CreateCommand(); // create empty command
dbcmd.CommandText = query; // fill the command
reader = dbcmd.ExecuteReader(); // execute command which returns a reader
}
catch (Exception e)
{

            titulo.text+=e.Message;

}
        string query2 = "";
        query2 = "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (1,1,'titulo ','CUIDADOS PARA ADULTOS MAYORES DEPENDIENTES');											";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (2,1,'	boton1	','	Inicio	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (3,1,'	boton2	','	Salir	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (4,1,'	Intro	','	Introducción:	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (5,1,'	menu1	','	Cuidados generales	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (6,1,'	menu1	','	Estimulación física y cognositiva	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (7,1,'	menu1	','	Cuidados específicos	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (8,1,'	menu1	','	Primeros auxilios	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (9,1,'	menu1	','	Cuidador	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (10,1,'	menu1	','	Bitácora del cuidador	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (11,1,'	menu1	','	Plan de cuidado	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (12,1,'	general	','	Regresar	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (13,1,'	menu2	','	Higiene General	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (14,1,'	menu2	','	Signos Vitales	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (15,1,'	menu2	','	Transferencia de la persona	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (16,1,'	menu2	','	Estimulación física y cognositiva	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (17,1,'	menu2	','	Eliminacion	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (18,1,'	menu3	','	Ejercicios de estimulacion	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (19,1,'	menu3	','	Masoterapia	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (20,1,'	menu3	','	Actividad Ludica	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (21,1,'	proceso	','	Materiales:	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (22,1,'	proceso	','	Duración:	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (23,1,'	proceso	','	Modulos	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (24,1,'	proceso	','	Procedimiento	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (25,1,'	proceso	','	Materiales y duracion	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (26,1,'	proceso	','	Antes	');							";
        query2 += "	INSERT INTO 'comandos' ('id','id_idioma','nombre','texto') VALUES (27,1,'	proceso	','	Siguiente	');							";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (1,1,1,'40 a 60 segundos.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (2,1,2,'Variable');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (3,1,3,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (4,1,4,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (5,1,5,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (6,1,6,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (7,1,7,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (8,1,8,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (9,1,9,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (10,1,10,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (11,1,11,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (12,1,12,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (13,1,13,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (14,1,14,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (15,1,15,'A consideración de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (16,1,16,'5 minutos');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (17,1,17,'5 minutos');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (18,1,18,'A consideracion de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (19,1,19,'Taquicardia sinusal Bradicardia sinusal Pulso débil');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (20,1,20,'Bradipnea Taquipnea Apnea Disnea Ortopnea');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (21,1,21,'Hipertensión arterial Hipotensión arterial');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (22,1,22,'Ayudar a transferir a la persona de la cama a la silla de ruedas');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (23,1,23,'Ayudar a transferir a la persona de la silla de ruedas a la cama');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (24,1,24,'Promover dignidad, intimidad e independencia, a la persona que recibe la intervención');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (25,1,25,'Subir a la persona que recibe la intervención, a un vehículo para poder transportarse');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (26,1,25,'Subir a la persona que recibe la intervención, a un vehículo para poder transportarse');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (27,1,26,'Subir a la persona que recibe la intervención, a un vehículo para poder transportarse');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (28,1,27,'Elección del cómodo o botella orinal Lleve todo el material a la habitación Proteja la intimidad de la persona Coloque el hule por debajo, entre la cintura y las rodillas de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (29,1,28,'Elección del cómodo o botella orinal Lleve todo el material a la habitación Proteja la intimidad de la persona Coloque el hule por debajo, entre la cintura y las rodillas de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (30,1,29,'Elección del cómodo o botella orinal Lleve todo el material a la habitación Proteja la intimidad de la persona Coloque el hule por debajo, entre la cintura y las rodillas de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (31,1,29,'Elección del cómodo o botella orinal Lleve todo el material a la habitación Proteja la intimidad de la persona Coloque el hule por debajo, entre la cintura y las rodillas de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (32,1,51,'La base de esta maniobra, es la compresión. Técnica utilizada para estimular los músculos, presionando de manera más profunda');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (33,1,52,'se realiza con oscilaciones rítmicas y se ejecutan con suavidad Tiene efecto suavizante y relajante sobre músculos y articulaciones Indicada en personas hipersensibles');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (34,1,53,'Es una técnica de masaje destinada a romper las adherencias en los músculos y los tejidos circundantes (promueve la circulación de estas áreas específicas)');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (35,1,54,'Tiene efectos estimulantes profundos Consiste en golpeteos rítmicos con la palma de las manos, sobre los músculos de la persona alternando ambas manos');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (36,1,55,'Ayuda a combatir el estreñimiento. los trastornos digestivos. se recomienda realizar en posición decúbito supino. Proporciona relajación. Ayuda a esculpir y remodelar la zona del abdomen.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (37,1,56,'Es el deslizamiento superficial. Puede ser superficial o profundo Se utiliza al: Iniciar. En cada cambio de técnica. Al finalizar.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (38,1,57,'Asociación de imágenes. Favorece el trabajo en equipo la música armoniza el ambiente grupal.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (39,1,59,'Mejora la memoria a corto y largo plazo, fomenta los recuerdos y refuerza los conocimientos. Asociación objeto/ sonido. Potencia la discriminación auditiva. Favorece el trabajo en equipo.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (40,1,60,'Esta acción terapéutica se realiza por medio de la expresión corporal y movimiento del cuerpo, apoyado de un tema musical. Potencializa la memoria. fomenta la creatividad. estimula la convivencia y socialización del usuario.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (41,1,61,'Tiene un efecto renovador de acuerdo a las vivencias de Cada persona. estimula el desarrollo perceptivo de las personas en la vida cotidiana. Activa los sentidos. Asociaciones en la retención de memoria/aprendizaje.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (42,1,62,'30 minutos para la ministración de los alimentos .30 minutos de reposo en posición de fowler.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (43,1,63,'30 minutos para la ministración de los alimentos.30 minutos de reposo en posición de fowler.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (44,1,64,'Dependiendo de la prescripción médica');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (45,1,65,'Es una vía de ministración cómoda, segura. Su efecto puede ser local o sistémico. No produce sensación de inseguridad en la persona debido a su facilidad de ingestión. Su dosificación es fácil y cómoda. No se puede ministrar en personas que presenten náuseas, vómito o dificultad para deglutir. No se puede ministrar en pacientes inconscientes. Con cirugía reciente de estómago o abdomen.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (46,1,66,'Es una vía de ministración cómoda, segura. Su efecto puede ser local o sistémico. No produce sensación de inseguridad en la persona debido a su facilidad de ingestión. Su dosificación es fácil y cómoda. No se puede ministrar en personas que presenten náuseas, vómito o dificultad para deglutir. No se puede ministrar en pacientes inconscientes. Con cirugía reciente de estómago o abdomen.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (47,1,67,'El paciente puede ingerir el medicamento por su cuenta. No hay efecto de primer paso. En caso de que la persona no sea cooperadora, el tratamiento tendrá dificultades. El medicamento es absorbido en el plexo bucal e ingresa a la circulación linfática y venosa.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (48,1,68,'Efecto local Pocos efectos secundarios. No produce dolor ni ansiedad. Es cómodo. Su aplicación es sencilla, la persona lo puede hacer por cuenta propia. El fármaco puede penetrar el organismo a través de abrasiones, dando lugar a un efecto sistémico. Limita las actividades de la persona. Puede producir irritación de la piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (49,1,69,'Efecto local Pocos efectos secundarios. No produce dolor ni ansiedad. Es cómodo. Su aplicación es sencilla, la persona lo puede hacer por cuenta propia. El fármaco puede penetrar el organismo a través de abrasiones, dando lugar a un efecto sistémico. Limita las actividades de la persona. Puede producir irritación de la piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (50,1,70,'Efecto local Pocos efectos secundarios. No produce dolor ni ansiedad. Es cómodo. Su aplicación es sencilla, la persona lo puede hacer por cuenta propia. El fármaco puede penetrar el organismo a través de abrasiones, dando lugar a un efecto sistémico. Limita las actividades de la persona. Puede producir irritación de la piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (51,1,71,'Efecto local Pocos efectos secundarios. No produce dolor ni ansiedad. Es cómodo. Su aplicación es sencilla, la persona lo puede hacer por cuenta propia. El fármaco puede penetrar el organismo a través de abrasiones, dando lugar a un efecto sistémico. Limita las actividades de la persona. Puede producir irritación de la piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (52,1,72,'Su absorción es más rápida que la vía oral. Evita de forma parcial el paso por el hígado. Se puede emplear cuando la vía oral no es opción debido a náuseas o vómito. Puede resultar incómoda en algunos casos. La presencia de heces dificulta la absorción. No se puede emplear en caso de diarrea.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (53,1,73,'Su acción es local. Puede resultar incómoda para algunas personas.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (54,1,74,'Rapidez en efecto local y sistémica. Sus efectos secundarios suelen desaparecer con rapidez. Es cómoda para el paciente. Es costosa. El efecto dura poco tiempo. Irritación de la mucosa del tracto respiratorio. Necesita de aparatos para la inhalación de aerosoles.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (55,1,75,'Permite administrar vehículos oleosos, además de soluciones acuosas, implantes depot, etc. Es una vía de administración generalmente molesta, contraindicada para pacientes tratados con anticoagulantes o con trastornos graves de la coagulación.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (56,1,76,'Efecto inmediato del medicamento. Es una vía de administración generalmente molesta, contraindicada para pacientes tratados con anticoagulantes o con trastornos graves de la coagulación. Mayor riesgo de efectos adversos. Solo puede administrar un volumen reducido. Posible riesgo de necrosis en la región de aplicación.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (57,1,77,'Permite administrar medicamento que de otra forma su absorción sería lenta o ineficaz. Es una vía de administración generalmente molesta. Solo se puede administrar un volumen reducido. Posible riesgo de necrosis en la región de aplicación.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (58,1,82,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (59,1,83,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (60,1,84,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (61,1,85,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (62,1,86,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (63,1,87,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (64,1,88,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (65,1,89,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (66,1,90,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (67,1,91,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (68,1,92,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (70,1,97,'Letargia Confusión Apatía Inactividad Somnolencia Desorientación Inatención Inquietud Irritabilidad Euforia Agitación psicomotriz Alucinaciones o ilusiones Agresividad');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (71,1,98,'Esfigmomanómetro Estetoscopio Reloj con segundero Medicamento prescrito por el médico Cobertor');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (72,1,99,'Cefalea Mareo Epistaxis (hemorragia nasal) Palpitaciones rápidas Referencia de vista nublada (fosfenos) o con presencia de lucecitas Referencia de zumbido de oídos (acufenos)');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (73,1,100,'Dolor torácico agudo (opresivo) y duradero, localizado en el centro del pecho y que se irradia hacia hombros, cuello, sobre todo hacia el brazo izquierdo, llegando hasta el dedo meñique. Palidez y diaforesis Mareo Sensación de falta de aire Ritmo cardiaco alterado');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (74,1,101,'Dolor torácico semejante al del ataque cardiaco (opresión en el pecho), puede extenderse a uno de los brazos El dolor rara vez dura más de 10 minutos Palidez y diaforesis (sudoración) Antecedente de haber realizado un esfuerzo o, someterse a alguna actividad estresante Sensación de ansiedad');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (75,1,102,'Enojo Mal humor Hambre Piel pálida y sudorosa Confusión Temblor Convulsiones Ausencia de respuesta Mala coordinación Glicemia casual menor a 60mg/dl');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (76,1,103,'Caída repentina. Ausencia de respuesta. Rigidez corporal y arqueo de la espalda. Movimiento muscular espasmódico. Incontinencia');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (77,1,104,'Respiración anormalmente lenta, ruidosa (como silbido o con flemas), profunda o superficial Labios azulados Palidez y diaforesis (sudoración) Necesidad de hacer pausas mientras habla, para recuperar el aliento');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (78,1,106,'Enrojecimiento Inflamación leve Dolor');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (79,1,107,'Ámpulas o flictenas Inflamación Dolor Escurrimiento de líquido Piel gris o chamuscada');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (80,1,108,'Piel gris a negra o carbonizada No hay dolor');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (81,1,109,'Sangrado excesivo con cada latido del corazón.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (82,1,110,'Sangrado que fluye de manera continua (escurrimiento).');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (83,1,111,'Sangrado de forma continua pero lenta, en forma de puntos.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (84,1,112,'Sangrado poco abundante');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (85,1,114,'Hematoma Área dolorida y sensible. Vómito o tos con sangre Heces negras o con sangre roja brillante');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (86,1,115,'Sangrado abundante Posible objeto incrustado. Orificio de entrada (pequeño). Orificio de salida (más extenso y con hemorragia).');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (87,1,116,'Sangrado abundante.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (88,1,117,'Gran cantidad de sangrado. Colgajos de piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (89,1,118,'Sangrado abundante. Bordes irregulares de piel.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (90,1,119,'Sangrado abundante. Órganos internos que sobresalen de la cavidad.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (91,1,120,'Pérdida de una parte corporal.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (92,1,121,'Dolor Inflamación Limitación de movimiento');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (93,1,122,'Deformidad Dolor Inflamación Limitación de movimiento');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (94,1,123,'Deformidad Herida abierta Sensibilidad y dolor Hinchazón/inflamación (regla DASH) Incapacidad de movimiento.');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (95,1,124,'Inconciencia Ausencia de pulso con o sin ausencia de respiración');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (96,1,125,'Inconciencia Ausencia de pulso con o sin ausencia de respiración');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (97,1,30,'Elección del cómodo o botella orinal .Lleve todo el material a la habitación. Proteja la intimidad de la persona. ');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (98,1,30,'Elección del cómodo o botella orinal .Lleve todo el material a la habitación. Proteja la intimidad de la persona. ');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (99,1,92,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (100,1,78,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (101,1,79,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (102,1,80,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'duracion' ('id','id_idioma','id_manual','Texto') VALUES (103,1,81,'Variable de acuerdo a las necesidades de la persona');											";
        query2 += "	INSERT INTO 'idioma' ('id','idioma') VALUES (1,'español');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (1,1,1,'Agua. Jabón líquido.Toalla desechable. Contenedor de basura.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (2,1,2,'Agua.  Esponja.   Jabón.   Toallas.   Lebrillo.  Protector.  Plástico.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (3,1,3,'Hule protector.  Recipiente plástico.  Champú o jabón  Toalla. secador eléctrico.  Agua tibia . Palangana.  Algodón.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (4,1,4,'Agua. estéril Gasas. Guantes desechables. Toalla. Bolsa o cesto de basura.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (5,1,5,'Algodón. Gasas. Hisopo.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (6,1,6,'Tina Pequeña. Recipiente con agua tibia. Jabon.Toalla. Lubricantes: vaselina,aceite de almendras o crema.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (7,1,7,'Jabón. Recipiente con agua tibia en cantidad suficiente. Cuña o cómodo. Toalla. Esponja. Jabón.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (8,1,8,'Jabón. Esponja. Recipiente con agua tibia. en cantidad suficiente. Cuña o cómodo. Toallas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (9,1,9,'Guantes desechables. Pañal limpio.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (10,1,10,'Guantes desechables. Cepillo dental. Pasta dental. Vaso con agua. Riñonera. Toalla.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (12,1,11,'Corta uñas. Lima de uñas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (13,1,12,'Hilo dental');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (14,1,13,'Cepillo dental. Pasta dental. Agua a chorro. Toalla desechable. Recipiente pequeño.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (15,1,14,'Jabón. Toalla limpia. Silla para la ducha. Alfombra o tapete de baño anti-derrapante.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (16,1,15,'Rastrillo o máquina de afeitar. Jabón o espuma para afeitar. Brocha pequeña para jabón. Recipiente. Toalla de baño. Toalla facial. Loción para después de afeitar (opcional).');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (17,1,16,'Guantes desechables. Sábanas (Cajón y plana). Cobija. Colcha. Funda para almohada. Bolsa para la ropa sucia.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (18,1,17,'Guantes desechables. Sábanas (Cajón y plana). Cobija. Colcha. Funda para almohada. Bolsa para la ropa sucia.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (19,1,18,'Guantes desechables. Sábanas (Cajón y plana). Cobija. Colcha. Funda para almohada. Bolsa para la ropa sucia.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (20,1,19,'Reloj con segundero. Bitácora.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (21,1,20,'Reloj con segundero. Bitácora.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (22,1,21,'Estetoscopio. Esfigmoma nómetro. Bitácora.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (23,1,22,'Silla de ruedas. Sábana (ésta deberá ser doblada en tres partes y colocada debajo de la persona que recibe la intervención, de modo que cubra una distancia de los hombros hasta sus rodillas, sábana clínica).');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (24,1,23,'Silla de ruedas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (25,1,24,'Tubos auxiliares. Silla de ruedas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (26,1,25,'Silla de ruedas. Automóvil.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (27,1,26,'Solución de agua inyectable. Esponja desechable. Guantes. Cesto de basura. Bolsas comunes. Gasas. Jabón neutro. Hule protector. Tijeras. Recipiente. Sistema de bolsa.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (28,1,27,'Solución de agua inyectable. Esponja desechable. Guantes. Cesto de basura. Bolsas comunes. Gasas. Jabón neutro. Hule protector. Tijeras. Recipiente. Sistema de bolsa.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (29,1,28,'Cómodo o botella orinal. Papel higiénico. Guantes. Hule .Toalla. Jabón. Esponja para lavado.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (30,1,29,'Cómodo o botella orinal. Guantes. Papel higiénico. Hule. Jabón. Toalla. Esponja para lavado.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (31,1,30,'Cómodo o botella orinal. Guantes. Jabón. Toalla. Esponja para lavado. Apósito para fijación de la sonda. Tela adhesiva. Benjuí.Gasas no estériles.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (32,1,31,'Silla. Pesas de distintas medidas.   Ropa comoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (33,1,32,'Silla. Pesas de distintas medidas.   Ropa comoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (34,1,33,'Silla. Pesas de distintas medidas.   Ropa comoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (35,1,34,'Silla. Pesas de distintas medidas.   Ropa comoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (36,1,35,'Silla. Pesas de distintas medidas.   Ropa comoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (37,1,36,'Ropa cómoda. Es recomendable que, para este ejercicio, la persona adulta mayor no tenga accesorios en las manos Pesas de distintas medidas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (38,1,37,'Es recomendable que, para la realización de este ejercicio, se evite que la persona adulta mayor tenga accesorios en las manos Pesas de distintas medidas');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (39,1,38,'Peloa de espoja de diferentes tamaños.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (41,1,39,'Ropa cómoda. Barras paralelas o una superficie plana y resistente.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (42,1,40,'Ropa cómoda. Barras paralelas .');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (43,1,41,'Ropa cómoda. Barras paralelas .');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (44,1,42,'Ropa cómoda. Barras paralelas .');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (45,1,43,'Ropa cómoda. Zapato tenis. Silla.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (46,1,44,'Ropa cómoda. Zapato tenis. Silla.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (47,1,45,'Ropa cómoda. Zapato tenis. Silla.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (48,1,46,'Ropa cómoda. Zapato tenis. Barras paralelas o en su defecto,una mesa.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (49,1,47,'Ropa cómoda. Zapato tenis. Barras paralelas o en su defecto,una mesa.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (50,1,48,'Ropa cómoda. Zapato tenis.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (51,1,49,'Ropa cómoda. Zapato tenis.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (52,1,50,'Camilla de masaje. cama o Silla. Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (53,1,51,'Camilla de masaje. cama o Silla. Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (54,1,52,'Camilla de masaje. cama o Silla. Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (55,1,53,'Camilla de masaje. cama o Silla. Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (56,1,54,'Camilla de masaje.  Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (57,1,55,'Camilla de masaje. Sábanas. Toallas. Aceite mineral.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (58,1,56,'Aceite mineral. Toallas desechables.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (59,1,57,'Tablero de Oca (su contenido va de acuerdo a las necesidades del grupo, en este caso será (musical). dados. música para armonizar. Reproductor de CD.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (60,1,59,'Fichas con los nombres de los instrumentos. Instrumentos como: un par de Platillos. un par de Maracas. un Triángulo.un Guitarra. un Tambor. un par de Cascabeles. un Bongó. un par de Panderetas. un Xilófono. un Flauta. un Acordeón .un Violín. un Armónica.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (61,1,60,'Área con espacio para realizar la actividad. equipo de música de buena calidad y sonoridad. CD o USB de las canciones elegidas. N° de participantes 1 a 8 personas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (62,1,61,'Sala amplia con mesas para Cada estación. equipo de sonido, USB. Protectores de ojos o bufandas.Gusto: Alimentos con sabores variados, dulces, salado, amargo, ácido como frutas, galletas, dulces. Tacto: Piedras, objetos con diferente tipo de texturas, semillas, telas, lijas. Olfato: Cosas con diferentes aromas como perfumes y esencias naturales. Oído: Cosas que generen sonidos e instrumentos musicales como llaves, papel, sonajas, canicas, pandero, flauta.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (63,1,62,'Jeringa asepto. Jeringa de 20 ml. Guantes .Gasas. Un vaso de agua. Alimento.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (64,1,63,'Bolsa con equipo de alimentación o frasco y equipo de goteo. Alimentación.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (65,1,64,'Bomba de infusión. Equipo para bomba de infusión. Alimentación.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (66,1,65,'Bandeja de medicamentos. Vaso graduado o vaso de papel. Vaso de agua. Mortero. Jeringa.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (67,1,66,'Bandeja de medicamentos. Vaso graduado, gotero, cuchara, sorbete o jeringa (dependiendo de la prescripción médica). Vaso con agua. Toalla de papel.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (68,1,67,'Bandeja de medicamentos. Vaso graduado o de papel.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (69,1,68,'Medicamento. Guantes. Solución estéril. Gasa o aplicador. Apósito de ojo. solo si es necesario.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (70,1,69,'Medicamento. Guantes desechables. Abatelenguas. Gasas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (71,1,70,'Guantes desechables. Algodón. Aplicadores. Gasa. Medicamento.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (72,1,71,'Medicamento. Recipiente para colocar residuos. Aplicadores. Algodón.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (73,1,72,'Guantes desechables. Bata para paciente. Gasas Vaselina, en caso de ser requerida. Medicamento.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (74,1,73,'Guantes desechables. Bata para paciente. Gasas Vaselina, en caso de ser requerida. Medicamento.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (75,1,74,'Medicamento. Nebulizador. cuando sea indicado.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (76,1,75,'Jeringas. Medicamentos para aplicarse. Agujas intramusculares, estériles. Solución antiséptica o alcohol. Gasas / Algodón. Guantes desechables. Contenedor para material punzante.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (77,1,76,'Jeringas. Medicamentos para aplicarse. Agujas intramusculares, estériles. Solución antiséptica o alcohol. Gasas / Algodón. Guantes desechables. Contenedor para material punzante.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (78,1,77,'Jeringas. Medicamentos para aplicarse. Agujas intramusculares, estériles. Solución antiséptica o alcohol. Gasas / Algodón. Guantes desechables. Contenedor para material punzante.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (79,1,78,'Conciliar el sueño en su horario y permitir un mejor descanso.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (80,1,79,'Proporcionar comodidad y tranquilidad en su habitación.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (81,1,80,'A través de la comunicación, el cuidador puede lograr un vínculo de confianza.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (82,1,81,'Aprovechar los momentos en donde tenga energía para distraerse.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (83,1,82,'Informar a la familia y crear un vínculo entre la persona, la familia y el cuidador.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (84,1,83,'Proporcionar la tranquilidad espiritual en la persona.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (85,1,84,'Silla');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (86,1,85,'Comida preparada. Agua para ingerir. Cubiertos. Plato y vaso.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (87,1,86,'Jabón de pH neutro. Toallas suaves .Compresas .Crema suave (lanolina) o Crema emoliente (vaselina) .Solución fisiológica .Gasas .Talco.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (88,1,87,'Laxantes (solo por prescripción médica) .Alimentos ricos en fibra. Agua para ingerir.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (89,1,88,'En caso de secreciones. Perilla. Guantes. Papel higiénico .Gogles o lentes para protegerse los ojos.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (90,1,89,'Aspirador. Sondas de aspiración .Solución. fisiológica. Gasas. Guantes desechables. Gafas o lentes protectores. Cubrebocas .2 Frascos de vidrio. Bolsa de residuos.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (91,1,90,'Esfigmomanómetro. Estetoscopio. Termómetro. Glucómetro .Tiras reactivas para el glucómetro. Aguja estéril para puncionar. Algodón con alcohol .Reloj con segundero. Medicamento prescrito por el médico. Vendas para realizar una sujeción.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (92,1,91,'Referencia de caída o presenciarla. Causas: Deshidratación Fiebre Cambio de dosis de medicamentos antihipertensivos Administración simultánea de medicamentos .Administración de sedantes o hipnóticos Alteraciones visuales y/o auditivas. Mal uso de ayudas técnicas para deambular Mala iluminación Dificultad para ver Confusión mental Descompensación de enfermedades preexistentes.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (93,1,92,'Esfigmomanómetro. Estetoscopio. Reloj con segundero. Medicamento prescrito por el médico.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (94,1,93,'Esfigmomanómetro. Estetoscopio. Reloj con segundero.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (95,1,94,'Medicamento prescrito por el médico.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (96,1,95,'Glucómetro. Tiras reactivas. Lancetas. Torundas con alcohol. Azúcar. Refresco o jugo.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (97,1,96,'Cobijas o almohadas para amortiguar la caída.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (98,1,97,'Inhalador (sólo si está prescrito con anterioridad)');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (99,1,98,'Agua. Venda estéril.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (100,1,99,'Agua. Venda estéril.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (101,1,100,'Agua. Venda estéril.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (102,1,101,'Apósitos estériles. Vendas Hielo.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (103,1,102,'Apósitos estériles. Vendas Hielo.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (104,1,103,'Apósitos estériles. Vendas Hielo.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (105,1,104,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (106,1,105,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (107,1,106,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (108,1,107,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (109,1,108,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (110,1,109,'Agua. Jabón quirúrgico. Apósitos no adherentes. Vendas.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (111,1,110,'Apósitos estériles. Vendas. Solución fisiológica o estéril.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (112,1,111,'Vendas. Apósitos estériles. Bolsa de plástico o recipiente Hielo. Solución fisiológica o estéril.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (113,1,112,'Férula rígida. Hielo. Vendas. Apósitos.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (114,1,113,'Férula rígida. Hielo. Vendas. Apósitos.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (115,1,114,'Férula rígida. Hielo. Vendas. Apósitos.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (116,1,115,'Si el evento se inicia con un paro cardiaco, la circulación se detiene y todos los órganos vitales quedan instantáneamente privados de oxígeno. La reparación cesa segundos después por hipoxia de los centros bulbares, aunque en algunos casos se mantiene una respiración agónica e ineficaz, que no debe ser considerada como respiración eficaz.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (117,1,116,'DAE: Dispositivo capaz de analizar la frecuencia cardiaca y proporcionar un choque eléctrico, conocido como desfibrilación, al corazón de la persona con paro cardiaco.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (118,1,221,'Silla o barra paralela.Ropa cómoda.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (119,1,222,'Ropa cómoda.Silla o barra paralela.');											";
        query2 += "	INSERT INTO 'material' ('id','id_idioma','id_manual','Texto') VALUES (120,1,227,'Aspirador. Sondas de aspiración .Solución. fisiológica. Gasas. Guantes desechables. Gafas o lentes protectores. Cubrebocas .2 Frascos de vidrio. Bolsa de residuos.');											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (1,1,'Higiene General',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (2,1,'Signos vitales',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (3,1,'Transferencia de la persona',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (4,1,'Eliminación',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (5,2,'Ejercicio terapéutico',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (6,2,'Masoterapia',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (7,2,'Reflexología',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (8,2,'Actividades lúdicas',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (9,3,'Ministración de alimentos',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (10,3,'Ministración de medicamentos',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (11,3,'Atención de personas en fase terminal',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (12,3,'Úlceras por presión',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (13,3,'Aspiración de secreciones',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (14,3,'Delirium',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (15,4,'Herida y fractura',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (16,4,'Hemorragia',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (17,4,'Quemadura',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (18,4,'Crisis asmática',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (19,4,'Convulsiones',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (20,4,'Hipoglucemia',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (21,4,'Angina de pecho',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (22,4,'Infarto agudo al miocardio',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (23,4,'Crisis hipertensiva',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (24,4,'Caídas',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (25,4,'Delirium',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (26,4,'Paro cardiorrespiratorio',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (27,5,'Características del cuidador',1);											";
        query2 += "	INSERT INTO 'menu' ('id','principal','categoria','id_idioma') VALUES (28,5,'Prevención del cuidador',1);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (1,1,1,'Higiene de manos','',12,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (2,1,1,'Baño de esponja','',11,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (3,1,1,'Higiene de cabello','',9,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (4,1,1,'Higiene de ojos','',6,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (5,1,1,'Higiene del pabellón auricular	','',5,'Duración.','Material.',NULL);										";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (6,1,1,'Higiene de pies','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (7,1,1,'Higiene de genitales en la mujer','',6,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (8,1,1,'Higiene de genitales en el hombre','',8,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (9,1,1,'Colocación de pañal','',8,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (10,1,1,'Higiene de la boca','',8,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (11,1,1,'Corte de uñas','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (12,1,1,'Uso del hilo dental','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (13,1,1,'Prótesis dental.','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (14,1,1,'Baño en regadera','		',7,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (15,1,1,'Afeitado facial','		',10,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (16,1,1,'Tendido Cama Cerrada','		',13,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (17,1,1,'Tendido Cama Abierta','		',14,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (18,1,1,'Tendido Cama Ocupada','		',17,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (19,1,2,'Pulso arterial','Es la onda pulsátil de la sangre, originada en la contracción del ventrículo izquierdo del corazón y que resulta en la expansión y contracción regular del calibre de las arterias; representa el rendimiento del latido cardiaco y la adaptación de las arterias Debe tener una fase ascendente y descendente, amplitud de la onda que le permite ser palpado, frecuencia y ritmo',5,'Posibles Alteraciones.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (20,1,2,'Frecuencia respiratoria','Número de veces que una persona inspira y espira por minuto Suele medirse cuando la persona está en reposo (y sin tener consciencia de estar haciéndolo)',4,'Posibles Alteraciones.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (21,1,2,'Presión o tensión arterial','La presión arterial resulta de la fuerza ejercida por la columna de sangre, impulsada por el corazón hacia los vasos sanguíneos. La fuerza de la sangre contra la pared arterial, es la presión sanguínea y la resistencia opuesta por las paredes de las mismas, es la tensión arterial',18,'Posibles Alteraciones.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (22,1,3,'De cama a silla de ruedas','		',9,'Utilidad.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (23,1,3,'De silla de ruedas a cama','		',7,'Utilidad.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (24,1,3,'De silla de ruedas a escusado','si no existen barras auxiliares, se puede utilizar cualquier elemento que se encuentre fijo y sea resistente.',12,'Utilidad.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (25,1,3,'De silla de ruedas a vehículo','		',6,'Utilidad.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (26,1,4,'Colostomía','Exteriorización quirúrgica del intestino grueso (colon), a través de la pared.abdominal, para provocar la salida del contenido fecal al exterior',36,'','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (27,1,4,'Urostomía','Derivación urinaria en la cual la salida de la orina tiene lugar a través de un estoma mucocutáneo en la pared abdominal',36,'','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (28,1,4,'Cómodo','Recipiente para auxiliar a una persona encamada en el proceso de eliminación (materia fecal y orina)',8,'','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (29,1,4,'Botella orinal','Recipiente para auxiliar a una persona encamada en el proceso de eliminación (solo orina)',9,'','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (30,1,4,'Sonda vesical o Foley (Cuidados Generales)','Es un catéter interno, que llega hasta la vejiga a través de la uretra; puede ser temporal o permanente, con el objetivo de drenar la orina fuera del cuerpo Se utiliza en caso de incontinencia, retención urinaria, previo y durante una cirugía, así como otro tipo de problemas de salud',11,'','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (31,1,5,'Hombro: Abducción y aducción','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (32,1,5,'Hombro: Flexión y extensión','',4,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (33,1,5,'Circunducción de Hombro','',2,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (34,1,5,'Codo: Flexión y extensión','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (35,1,5,'Codo: Supinación y pronación','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (36,1,5,'Muñeca: Flexoextensión','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (37,1,5,'Muñeca: Abducción y aducción','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (38,1,5,'Muñeca: Flexión y extensión','',4,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (39,1,5,'Cadera: Extensión y flexión','',6,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (40,1,5,'Cadera: Abducción y aducción','',5,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (41,1,5,'Cadera: Rotaciones','',4,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (42,1,5,'Rodilla: Flexión y extensión','',4,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (43,1,5,'Extremidades superiores:Extensión y flexión','',2,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (44,1,5,'Extremidades superiores: Aducción y abducción','',2,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (45,1,5,'Extremidades inferiores: Flexión y extensión','',2,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (46,1,5,'Extremidad superior e inferior izquierda y derecha:Flexión y extensión','',2,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (47,1,5,'Extremidad superior e inferior izquierda y derecha:Abducción y aducción','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (48,1,5,'Extremidad superior derecha e inferior izquierda:Extensión y flexión','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (49,1,5,'Extremidad superior izquierda e inferior derecha:Aducción y abducción','',3,'Duración.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (50,1,6,'Deslizamiento (Effleurage)','		',3,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (51,1,6,'Amasamiento (Petrissage)','		',3,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (52,1,6,'Vibración','		',2,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (53,1,6,'Fricción','		',1,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (54,1,6,'Golpeteos (Tapottement)','		',3,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (55,1,6,'Masaje abdominal','		',5,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (56,1,7,'Effleurage profundo con digitopresión','		',7,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (57,1,8,' Oca musical','Musicoterapia',9,'Utilidad.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (59,1,8,'Sonidos de instrumentos musicales','Musicoterapia',3,'Utilidad.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (60,1,8,'Danzaterapia','Baile en Grupo Baile en pareja',4,'Utilidad.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (61,1,8,'Sensorama','Estaciones de los sentidos',9,'Utilidad.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (62,1,9,'Jeringa','		',8,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (63,1,9,'Goteo','		',10,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (64,1,9,'Bomba','		',8,'Duración.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (65,1,10,'Oral sólida','		',5,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (66,1,10,'Oral líquida','		',4,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (67,1,10,'Sublingual.','		',4,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (68,1,10,'Tópica Oftálmica','',5,'Ventajas y Desventajas.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (69,1,10,'Tópica Dérmica','',3,'Ventajas y Desventajas.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (70,1,10,'Tópica Nasal','',4,'Ventajas y Desventajas.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (71,1,10,'Tópica Ótica','',6,'Ventajas y Desventajas.','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (72,1,10,'Rectal','		',9,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (73,1,10,'Vaginal.','		',8,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (74,1,10,'Inhalatoria.','		',4,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (75,1,10,'Intramuscular','		',13,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (76,1,10,'Intravenoso','		',16,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (77,1,10,'Subcutánea o hipodérmica','		',13,'Ventajas y Desventajas.','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (78,1,11,'Reposo y sueño','Cómo mejorar el descanso',5,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (79,1,11,'Intimidad o privacidad','Creación de una habitación confortable',7,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (80,1,11,'Empatía','Recomendacione s para una buena comunicación',9,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (81,1,11,'Ocupación del tiempo libre','Actividades de distracción',6,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (82,1,11,'Apoyo a la familia','Incluir a la familia',6,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (83,1,11,'Espiritualidad','		',3,'Duración','Beneficios.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (84,1,11,'Disnea','Enseñar ejercicios de respiración.',8,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (85,1,11,'Alimentación','Realizar la alimentación de forma segura y a su gusto.',7,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (86,1,11,'Piel','Mantener la integridad de la superficie corporal.',9,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (87,1,11,'Estreñimiento','Aliviar los síntomas de estreñimiento y ayudar a evacuar',5,'Duración','Beneficios.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (88,1,11,'Tos y secreciones','Enseñar al enfermo a toser y evitar molestias.',11,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (89,1,13,'Aspiración de secreciones NASOFARÍNGEA','		',6,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (90,1,25,'Delirium','Causas:DeshidrataciónFiebreHipotermiaCambio de dosis de medicamentos o aumento en el número de fármacosAdministración simultánea de medicamentosAdministración de sedantes o hipnóticosAlteraciones visuales y/o auditivasMal uso de ayudas técnicas para deambularDescompensación de enfermedades pre-existentes',12,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (91,1,24,'Caída','Suele ser repentina, involuntaria e insospechada; puede ser confirmada o no, por la persona o un testigo. Se convierte en un problema cuando se presentan dos o más caídas en menos de un año. Factores de la persona que pueden condicionar la presencia de caídas: edad mayor a 80 años, sexo femenino, caída previa, debilidad muscular de miembros inferiores, artritis, alteraciones ungueales, malformaciones en pies, dificultad y uso de ayudas técnicas para deambular, alteraciones de la marcha y balance, trastornos visuales, deterioro en las actividades de la vida diaria, depresión, daño cognoscitivo, uso de psicofármacos (incluyendo antipsicóticos, antidepresivos, antidemenciales, sedantes y uso de más de 4 medicamentos). Vértigo, mareo, confusión, hipotensión postural, infección, desequilibrio hidroelectrolítico, obesidad, trastornos visuales o auditivos (vestibulares), etc.',12,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (92,1,23,'Crisis hipertensiva','		',4,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (93,1,22,'Infarto agudo al miocardio','		',6,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (94,1,21,'Angina de pecho','		',6,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (95,1,20,'Hipoglucemia','		',3,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (96,1,19,'Convulsiones','		',8,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (97,1,18,'Crisis asmática','		',4,'Duración','Material.',NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (98,1,17,'Primer Grado','Quemadura Primer Grado: Es superficial, ya que solo afecta la epidermis, presentan enrojecimiento, inflamación, ardor, molestia al tacto e irritación de la piel.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (99,1,17,'Segundo grado','Quemadura Segundo grado: Se lesiona la capa externa de la piel en todo su espesor y la dermis. Se forman flictenas (ampollas), la piel adquiere un color rojo intenso y se ve manchada. Se acompaña de dolor intenso.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (100,1,17,'Tercer grado','Quemadura Tercer grado: Se consideran severas ya que abarcan todas las capas de la piel. Suelen afectar el tejido graso, nerviosos, músculos e inclusive huesos. Se aprecian áreas carbonizadas de color negro o deshidratado de aspecto blanquecino. No hay dolor. Todas las quemaduras por electricidad, se consideran de tercer grado.',11,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (101,1,15,'Arterial','Hemorragia Arterial: Es cuando se produce la ruptura de una arteria, por lo tanto, la sangre es de color rojo brillante, ésta sale de forma intermitente y potente debido a la fuerza del corazón.',11,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (102,1,15,'Venosa','Hemorragia Venosa: Se caracteriza por una salida continua de sangre, en forma de escurrimiento, debido a que las venas llevan sangre de los órganos hacia el corazón. La sangre es de color rojo oscuro.',5,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (103,1,15,'Capilar','Hemorragia Capilar: Es cuando se produce la ruptura de los vasos capilares, es decir, compromete a los vasos sanguíneos de pequeño calibre y se manifiesta como puntos rojos, se caracteriza por la escasa salida de sangre de color rojo brillante.',5,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (104,1,15,'Abrasión','Herida Y Fractura Abrasión: Herida causada por fricción con superficies rugosas o ásperas, como una caída sobre rocas o asfalto “raspón”.',5,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (105,1,15,'Contusión','Herida Y Fractura Contusión: Herida cerrada causada por golpes con objetos romos, es decir, sin punta ni filo, como un palo, un martillo, entre otros, (no hay hemorragia externa, solo interna).',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (106,1,15,'Punción','Herida Y Fractura Punción: Herida originada por objetos con punta que penetran al cuerpo, como clavos, desarmadores, agujas, balas, ente otros. Sus bordes son redondos y regulares, (el objeto puede permanecer incrustado).',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (107,1,15,'Incisión','Herida Y Fractura Incisión: Herida producida por objetos con filo, como hojas para rasurar, sierras o cuchillos entre otros. Tiene bordes regulares uniformes.',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (108,1,15,'Avulsión','Herida Y Fractura Avulsión: Herida provocada por mordeduras o mecanismos que provoquen un desgarramiento de la piel, sus bordes son completamente irregulares y forman colgajos de piel que quedan unidos al cuerpo por una de sus partes.',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (109,1,15,'Laceración','Herida Y Fractura Laceración: Corte en la piel con bordes irregulares. Este tipo de herida se produce por una rasgadura forzada del tejido cutáneo.',5,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (110,1,15,'Herida de abdomen con evisceración','Herida Y Fractura Herida de abdomen con evisceración: Lesión grave del abdomen, en la cual los órganos internos escapan o se proyectan hacia afuera desde la herida.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (111,1,15,'Amputación','Herida Y Fractura Amputación: El corte y desprendimiento de una parte del cuerpo.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (112,1,15,'Esguince','Herida Y Fractura Esguince: Elongación o ruptura de los ligamentos de una articulación.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (113,1,15,'Luxación','Herida Y Fractura Luxación: Lesión donde los Apósitos extremos de los huesos de una articulación, pierden continuidad.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (114,1,15,'Fractura','Herida Y Fractura Fractura: Pérdida de la continuidad del tejido óseo Hay dos tipos de fracturas: 1.- Fractura cerrada: Cuando no existe herida abierta, en torno al sitio de fractura 2.- Fractura expuesta: Cuando existe una herida abierta, y el extremo del hueso roto puede sobresalir a través de la piel.',6,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (115,1,26,'Paro cardiorespiratorio','Nota: Realice las compresiones con ambas manos en una persona adulta, con una o dos manos en un niño y dos dedos en un lactante. El tórax de una persona adulta, debe comprimirse por lo menos 5 cm, el de un niño cerca de 5 cm y, el de un lactante alrededor de 3.6 cm.1 El uso de un DAE es exclusivo de personal capacitado.',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (116,1,26,'DAE','Paro cardiorespiratorio',9,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (117,1,0,'Abducción','Movimiento de una extremidad, que se separa de la línea media del cuerpo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (118,1,0,'Acupuntura','Método de producir analgesia o alterar la función de un sistema corporal, mediante la inserción de finas agujas en puntos específicos de la superficie ',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (119,1,0,'Aducción','Movimiento de una extremidad, en dirección al cuerpo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (120,1,0,'Alucinación','Percepción de una imagen, objeto o estímulo exterior inexistente, que son considerados como reales.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (121,1,0,'Ampolla','Acumulación de líquido bajo la piel, debido a pequeñas lesiones.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (122,1,0,'Anastomosis','Conexión quirúrgica entre dos estructuras habitualmente similares, principalmente, de tipo tubulares como intestinales (grueso o delgado), fístulas arterio-venosas, uretrales.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (123,1,0,'Anemia','Trastorno que se caracteriza por la disminución de la hemoglobina sanguínea, hasta concentraciones inferiores a los límites normales',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (124,1,0,'Antidepresivo','Medicamento utilizado para los trastornos por depresión, diversos trastornos de ansiedad, así como ciertos desórdenes de conducta alimentaria.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (125,1,0,'Apéndice xifoides','Extremo inferior del esternón.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (126,1,0,'Apnea','Ausencia de respiración espontánea.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (127,1,0,'Apósito','Tejido limpio o estéril, que se aplica directamente a una herida o lesión, para absorber secreciones, detener una hemorragia, etc.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (128,1,0,'Arteriosclerosis','Enfermedad arterial frecuente, que se caracteriza por engrosamiento, pérdida de elasticidad y calcificación de las paredes arteriales, que condiciona una disminución del riego sanguíneo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (129,1,0,'Asepsia','Eliminación o destrucción de los gérmenes patológicos, o los materiales infectados.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (130,1,0,'Atelectasia','Disminución del volumen pulmonar causada por obstrucción de las vías aéreas (bronquios o bronquiolos) o por presión en la parte externa del pulmón. Colapso de una parte o todo el pulmón.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (131,1,0,'Atrofia','Desaparición o disminución del tamaño o la actividad fisiológica de una parte del cuerpo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (132,1,0,'Axón','Prolongación de las neuronas especializadas, en conducir los impulsos nerviosos hasta una o varias células.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (133,1,0,'Bradicardia','El corazón disminuye su función, a una frecuencia inferior a 60 latidos por minuto.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (134,1,0,'Bradipnea','Frecuencia respiratoria anormalmente lenta.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (135,1,0,'Broncoaspiración','Inhalación del contenido gástrico u orofaríngeo dentro de la laringe o tracto respiratorio bajo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (136,1,0,'Broncoespasmo','Contracción anormal del músculo liso de los bronquios, lo que resulta en un estrechamiento y obstrucción de las vías respiratorias. Se presenta disnea, palidez y/o cianosis y tos con sibilancias.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (137,1,0,'Cefalea','Dolor de cabeza.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (138,1,0,'Céfalo-caudal','Relación entre la cabeza y la base de la columna.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (139,1,0,'Circunducción','Movimiento circular de una parte del cuerpo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (140,1,0,'Clónico','Movimientos espasmódicos rítmicos e involuntarios en los músculos en ambos lados del cuerpo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (141,1,0,'Colón sigmoide','Porción del colón que se extiende desde el final del colón descendente en la pelvis hasta su unión con el recto.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (142,1,0,'Decúbito lateral','Posición horizontal acostada (de lado).',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (143,1,0,'Decúbito prono','Posición horizontal acostada (boca abajo).',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (144,1,0,'Decúbito supino','Posición horizontal acostada (boca arriba).',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (145,1,0,'Deglución','Acción de deglutir, tragar, hacer pasar de la boca hacia el estómago.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (146,1,0,'Dendritas','Extensión ramificada que forma parte de las neuronas dedicadas principalmente a la recepción de impulsos.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (147,1,0,'Dermatoma','Capa mesodérmica del desarrollo embrionario precoz que origina las capaz dérmicas de la piel.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (148,1,0,'Dermis','Es la capa de la piel situada por debajo de la epidermis.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (149,1,0,'Desbridar','Retirar la suciedad, los objetos extraños y el tejido lesionado para evitar una infección y facilitar la cicatrización.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (150,1,0,'Diaforesis','Sudoración excesiva que puede ser la respuesta a la actividad física, a una emoción, a temperatura ambiental alta, un síntoma de una enfermedad subyacente o el efecto crónico de anfetaminas. Se le debe prestar mayor atención cuando se acompaña de fiebre, pérdida de peso, dolor torácico, disnea y taquicardia.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (151,1,0,'Disnea','Dificultad para respirar que puede deberse a ciertas enfermedades cardiacas o respiratorias, ejercicio extenuante o ansiedad.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (152,1,0,'Edema','Acúmulo anormal del líquido en los espacios intersticiales.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (153,1,0,'Encéfalo','Conjunto de órganos que conforman parte del sistema nervioso, comúnmente denominado cerebro.  ',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (154,1,0,'Enema','Procedimiento por el cual se introduce una solución en el recto, con fines terapéuticos o de limpieza.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (155,1,0,'Enervante','Quitar o debilitar la fuerza.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (156,1,0,'Epidermis','Capa superficial de la piel, constituida por una región externa.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (157,1,0,'Epistaxis','Se le denomina a toda hemorragia con origen en las fosas nasales.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (158,1,0,'Eritema','Enrojecimiento o inflamación de la piel o las membranas mucosas, como resultado de la dilatación y congestión de los capilares superficiales.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (159,1,0,'Espasmo','Contracción muscular involuntaria de comienzo brusco, como el hipo o un tic.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (160,1,0,'Estado de shock o choque','Estado de bajo aporte sanguíneo a los órganos y tejidos, con la subsecuente falta de oxígeno y de nutrientes al organismo.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (161,1,0,'Estoma','Apertura artificial de un órgano interno en la superficie del cuerpo, creada quirúrgicamente.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (162,1,0,'Extensión','Movimiento realizado por ciertas articulaciones del esqueleto, que aumenta el ángulo entre dos huesos adjuntos.',0,'Duración','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (163,1,0,'Fascia','Tejido conjuntivo fibroso que puede estar separado de otras estructuras específicamente organizadas como los tendones, aponeurosis y ligamentos.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (164,1,0,'Fecalito','Masa dura e impactada de heces en el colón. ',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (165,1,0,'Férula','Dispositivo ortopédico para inmovilizar, limitar el movimiento o sostener cualquier parte del cuerpo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (166,1,0,'Fístula','Comunicación anormal entre un órgano interno y la superficie corporal.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (167,1,0,'Flexión','Movimiento permitido por ciertas articulaciones del esqueleto, que disminuye el ángulo entre dos huesos adyacentes.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (168,1,0,'Fowler','Colocación de la persona acostada con la mitad superior del cuerpo levantada.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (169,1,0,'Glucosa','(azúcar) Fuente fundamental de energía, presente en los líquidos corporales.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (170,1,0,'Hematoma','Acumulación de sangre extravasada, incluida en los tejidos de la piel o en un órgano, se forma como consecuencia de un traumatismo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (171,1,0,'Hipertermia','Aumento de la temperatura corporal por arriba de los niveles normales. ',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (172,1,0,'Hipotensión','Disminución de la presión arterial por debajo de los niveles normales del individuo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (173,1,0,'Hipotermia','Disminución de la temperatura corporal por debajo de los niveles normales.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (174,1,0,'Hipoxemia','Disminución anormal de la presión parcial de oxígeno en la sangre arterial. No debe confundirse con hipoxia.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (175,1,0,'Hipoxia','Estado de deficiencia de oxígeno en la sangre, células y tejidos del organismo, con compromiso de la función de éstos.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (176,1,0,'Hisopo','Palito o pinza que lleva sujeta una gasa o algodón absorbente y se utiliza para la asepsia corporal.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (177,1,0,'Homeostasis','Mantenimiento del medio interno en un estado constante, gracias a un conjunto de respuestas adaptativas que permiten conservar la salud y la vida.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (178,1,0,'Ilusión','Percepción distorsionada de un estímulo externo efectivamente existente, caracterizado por la percepción de imágenes que son falsas o erróneas.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (179,1,0,'Involución','Proceso normal caracterizado por la disminución del tamaño de un órgano debida a la reducción del volumen de sus células.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (180,1,0,'Isquemia','Disminución del aporte de sangre a un órgano o a una zona del organismo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (181,1,0,'Maléolo','Tuberosidades óseas sobresalientes bajo la piel y a cada lado situadas a nivel de la articulación del tobillo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (182,1,0,'Malnutrición','Cualquier trastorno relativo a la nutrición.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (183,1,0,'Memoria a corto plazo','Permite retener solamente durante unos segundos, cierta información.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (184,1,0,'Memoria a largo plazo','Sirve para conservar la información durante minutos, horas, semanas o años.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (185,1,0,'Necrosis','Muerte de una porción de tejido consecutiva a enfermedad o lesión.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (186,1,0,'Neurona pos-sináptica','Neurona encargada de recibir el impulso nervioso',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (187,1,0,'Neurona pre-sináptica','Neurona encargada de liberar los neurotransmisores, para que el impulso nervioso pase a la siguiente neurona.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (188,1,0,'Obnubilado','Persona que presenta una disminución del nivel de consciencia que se caracteriza por la existencia de confusión, movimientos torpes, lentitud en el pensamiento, disminución en la atención y de la percepción.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (189,1,0,'Occipital','Hueso del cráneo, que constituye el límite posterior de la cabeza y la mitad de su base.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (190,1,0,'Ortopnea','Proceso anormal en el que una persona debe sentarse o permanecer de pie, para respirar profunda o confortablemente.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (191,1,0,'Ostomía','Procedimiento quirúrgico en el que se hace una abertura, para permitir el paso de orina desde la vejiga o, de heces desde el intestino, hasta una incisión o estoma.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (192,1,0,'Palangana','Recipiente circular, ancho y poco profundo, usado para lavarse.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (193,1,0,'Parálisis','Proceso anormal caracterizado por la pérdida de función muscular o de sensibilidad.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (194,1,0,'Patología','Estudio de las características, causas y efectos de la enfermedad.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (195,1,0,'Periestomal','Piel que rodea a un estoma u orificio creado quirúrgicamente en la pared abdominal.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (196,1,0,'Periné','Parte del cuerpo situada por detrás del arco púbico.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (197,1,0,'Peristaltismo','Contracciones coordinadas, rítmicas y seriadas del músculo liso, que fuerzan el desplazamiento.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (198,1,0,'Persona no colaboradora','Es aquella que no es capaz de participar en la actividad, debido a su actitud, condición física o cognoscitiva.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (199,1,0,'Persona que colabora','Es aquella que por su actitud y condiciones físicas y cognoscitivas, es capaz de participar en la actividad.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (200,1,0,'Plexo','Red de nervios, vasos sanguíneos o vasos linfáticos entrecruzados.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (201,1,0,'Posición de Sims','La persona está en posición prona ligeramente inclinada, con la cabeza girando hacia un lado.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (202,1,0,'Posición ginecológica','La persona se encuentra en decúbito supino, con las rodillas separadas y flexionadas, ya sea por arriba de la altura del cuerpo o a la misma altura de éste.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (203,1,0,'Posición sedente','La persona se encuentra en posición de Fowler, excepto que el torso está en posición vertical, formando un ángulo recto.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (204,1,0,'Pródromo','Síntomas inespecíficos o estadio que preceden a las manifestaciones propias de la enfermedad.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (205,1,0,'Prolapso','Caída, hundimiento o deslizamiento de un órgano desde su posición o localización corporal normal.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (206,1,0,'Pronación','Rotación del antebrazo que permite situar la mano con el dorso hacia arriba.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (207,1,0,'Riñonera','Pequeña palangana en forma de riñón.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (208,1,0,'Shock','Estado fisiológico anormal, que constituye la primera fase de la reacción del organismo, frente a una lesión traumática.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (209,1,0,'Supinación','Rotación del antebrazo, que permite situar la mano con el dorso hacia abajo.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (210,1,0,'Taquicardia','La frecuencia cardiaca se acelera con una frecuencia de 100 a 150 latidos por minuto.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (211,1,0,'Taquipnea','Aceleración anómala de la frecuencia respiratoria.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (212,1,0,'Tejidos profundos','Capas profundas de los músculos.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (213,1,0,'Tónicos','Contracción muscular sostenida, que puede durar segundos o minutos, causando rigidez.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (214,1,0,'Trocánter','Cualquiera de las dos apófisis, por debajo del cuello del fémur.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (215,1,0,'Varices esofágicas','Venas anormalmente inflamadas dentro del recubrimiento del esófago, las cuales pueden provocar hemorragias severas a quien las padece, al romperse alguna de ellas.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (216,1,0,'Vasoconstricción','Estrechamiento de vasos sanguíneos, por parte de pequeños músculos en sus paredes.',0,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (217,1,4,'Sonda vesical o Foley (Vaciamiento de la bolsa)','Es un catéter interno, que llega hasta la vejiga a través de la uretra; puede ser temporal o permanente, con el objetivo de drenar la orina fuera del cuerpo Se utiliza en caso de incontinencia, retención urinaria, previo y durante una cirugía, así como otro tipo de problemas de salud',9,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (218,1,4,'Sonda vesical o Foley (Fijacion de la Sonda)','Es un catéter interno, que llega hasta la vejiga a través de la uretra; puede ser temporal o permanente, con el objetivo de drenar la orina fuera del cuerpo Se utiliza en caso de incontinencia, retención urinaria, previo y durante una cirugía, así como otro tipo de problemas de salud',8,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (219,1,4,'Sonda vesical o Foley (Limpieza de la sonda y piel)','Es un catéter interno, que llega hasta la vejiga a través de la uretra; puede ser temporal o permanente, con el objetivo de drenar la orina fuera del cuerpo Se utiliza en caso de incontinencia, retención urinaria, previo y durante una cirugía, así como otro tipo de problemas de salud',12,'Duracion','Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (220,1,5,'Tobillo:Flexión plantar y dorsal','',6,NULL,'Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (221,1,5,'Tobillo:Supinación y pronación.','',6,NULL,'Material.',NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (222,1,12,'Cuidado de ulceras','Se conoce como úlceras por presión, a las lesiones de origen isquémico en la piel y sus tejidos subyacentes, de acuerdo a los diferentes factores extrínsecos e intrínsecos. Las úlceras pueden ser muy frecuentes, es por ello que, es importante dar a conocer las intervenciones básicas para su prevención y cuidado. Los cuidadores podrán asistir a las personas en este aspecto, coadyuvando a mejorar su calidad de vida.',6,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (223,1,12,'Prevención Decubito supino','Colocacion.Almohada en cabeza.Almohada en gemelos, para facilitar el retorno venoso.Almohada debajo de los pies para evitar el pie equino.Almohada en trocánter, para evitar la rotación.',6,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (224,1,12,'Prevención Decúbito prono','Colocacion.Almohada debajo de la cabeza.Almohada en el abdomen para evitar tensión muscular.Almohada debajo de las piernas para favorecer la circulación.Almohada debajo de los hombros para bajar la tensión muscular.',6,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (225,1,12,'Prevención Decúbito lateral','Colocacion.Almohada debajo de la cabeza.Almohada a lo largo de la espalda.Almohada entre las rodillas y tobillos para evitar roces.Almohada debajo del brazo.',6,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (226,1,12,'Prevención Decúbito sentado','Colocación.Almohada detrás de la cabeza.Almohada debajo del brazo.Almohada debajo de los pies.',6,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (227,1,13,'Aspiración de secreciones OROFARÍNGEA','		',6,NULL,NULL,NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (228,1,14,'Delirium:Factores predisponentes','Factores socio-demográficos: Edad igual o mayor a 65 años Sexo masculino.Vivir en una institución de asistencia. Pobre red de apoyo familiar.		',1,NULL,NULL,NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (229,1,14,'Delirium:Factores predisponentes','		',1,NULL,NULL,NULL);									";
        query2 += "	INSERT INTO 'manual' ('id','id_idioma','menu','Actividad','Auxiliar','pasos','dur','mat','id_pregunta') VALUES (230,1,14,'Delirium:Intervención','',1,NULL,NULL,NULL);											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (1,1,1,1,'Mojarse las manos.','1_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (2,1,1,2,'Verter jabón líquido, en la palma de la mano ','1_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (3,1,1,3,'Frotar las palmas entre sí.','1_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (4,1,1,4,'Frotar la palma de la mano derecha contra el dorso de la mano izquierda, entrelazando los dedos y viceversa.','1_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (5,1,1,5,'Frotar las palmas con los dedos entrelazados.','1_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (6,1,1,6,'Frotar los dedos de una mano contra los de la otra mano, en forma de gancho, con un movimiento lateral.','1_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (7,1,1,7,'Frotar con un movimiento de rotación el pulgar izquierdo, sujetándolo con la palma derecha y viceversa, posteriormente, hágalo con el pulgar derecho. ','1_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (8,1,1,8,'Frotar la punta de los dedos de la mano derecha contra la palma de mano izquierda, haciendo un movimiento de rotación del centro a la periferia y viceversa, posteriormente, hágalo con la mano izquierda.','1_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (9,1,1,9,'Enjuague las manos con suficiente agua.','1_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (10,1,1,10,'Seque con una toalla desechable.','1_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (11,1,1,11,'Cerrar la llave con la toalla.','1_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (12,1,1,12,'Desechar la toalla en el cesto de basura.','1_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (13,1,2,1,'Saludar a la persona e informar el  procedimiento.','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (14,1,2,2,'Cerrar puerta y ventanas.','2_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (15,1,2,3,'Preparar el material a utilizar.','2_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (16,1,2,4,'Lavarse las manos y colocarse los guantes. ','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (17,1,2,5,'Retirar la colcha y cobijas. Para mayor comodidad, deje la almohada.','2_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (18,1,2,6,'Colocar a la persona en decúbito supino y retirar la ropa cuidadosamente. Cuide la integridad colocándole una sábana, (solo se descubrirá la zona que se lavará).','2_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (19,1,2,7,'El baño se realiza de forma céfalocaudal(de cabeza a pies), de distal a proximal. Enjabonar, enjuagar y secar, debe hacerse rápido para evitar que el jabón se seque, y pueda provocar resequedad en la piel causando comezón.','2_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (20,1,2,8,'Cambiar el agua cuantas veces sea necesaria.','2_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (21,1,2,9,'Lave la cara y tenga cuidado con los ojos. (Ver Higiene del cabello, Higiene de ojos, así como de pabellón auricular)','2_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (22,1,2,10,'Se continúa lavando el cuello, hombros, manos, brazos, axilas, tórax, mamas, abdomen, piernas y termine con los pies (Ver higiene de pies)','2_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (23,1,2,11,'Una vez lavada la parte anterior, coloque a la persona en decúbito lateral, lave la espalda y nalgas, termine el aseo con la zona genital (Ver higiene de genitales). ','2_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (24,1,3,1,'Colocar a la persona en posición de Rosier (acostado con la cabeza colgado).','3_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (25,1,3,2,'Colocar un tapón de algodón en cada oído.','3_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (26,1,3,3,'Colocar la palangana vacía debajo de la cabeza.','3_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (27,1,3,4,'Mojar el cabello con abundante agua asegurándose que la temperatura sea tibia','3_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (28,1,3,5,'Aplicar champú, y masajear suavemente en círculos abarcando toda la superficie del cuero cabelludo','3_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (29,1,3,6,'Enjuagar con abundante agua y repetir si es necesario.','3_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (30,1,3,4,'Mojar el cabello con abundante agua asegurándose que la temperatura sea tibia','3_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (31,1,3,5,'Aplicar champú, y masajear suavemente en círculos abarcando toda la superficie del cuero cabelludo ','3_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (32,1,3,6,'Enjuagar con abundante agua y repetir si es necesario.','3_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (33,1,3,7,'Retirar la palangana y el hule protector.','3_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (34,1,3,8,'Posteriormente, se utilizará una toalla limpia la cual mantendrá el cabello cubierto unos minutos, para que absorba la humedad.','3_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (35,1,3,9,'Deje a la persona en una posición cómoda.','3_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (42,1,5,1,'Colocar la cabeza de la persona, flexionada hacia el hombro del lado contrario al oído que se va a asear','5_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (43,1,5,2,'Traccione ligeramente el pabellón auricular hacia arriba y hacia atrás','5_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (44,1,5,3,'Para limpiar el pabellón auricular, se utilizará hisopo o una gasa debidamente humedecida en suero salino o en agua jabonosa, se limpia cada pliegue o surco del pabellón desde el centro a la periferia','5_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (45,1,5,4,'De la misma forma, se secará con una gasa limpia para cada pabellón','5_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (46,1,5,5,'Se continúa lavando la cara evitando que entre jabón en los ojos de la persona','5_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (47,1,6,1,'Poner atención en la piel para descubrir: rotura de la uña y/o grietas entre los dedos.','6_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (48,1,6,2,'Flexionar las piernas de la persona y meterlas en la tina pequeña.','6_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (49,1,6,3,'Lavar los pies con agua tibia y jabón. ','6_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (50,1,6,4,' Con una toalla secar bien cada pie, dedo por dedo.','6_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (51,1,6,5,'Lubricar los pies con vaselina, aceite de almendras o una crema.','6_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (52,1,7,1,'Colocar a la persona en decúbito supino, con las piernas flexionadas','7_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (53,1,7,2,'Colocar la cuña o cómodo debajo de las nalgas','7_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (54,1,7,3,'Dejar caer el agua tibia y con la esponja enjabonada, limpie los labios, realizando los movimientos de adelante hacia atrás.','7_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (55,1,7,4,'Enjuagar con agua tibia.','7_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (56,1,7,5,'Con una toalla, seque de adelante hacia atrás, con mayor preferencia en los pliegues.','7_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (57,1,7,6,'Retirarse los guantes y lavarse las manos.','7_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (58,1,8,1,'Colocar a la persona en decúbito supino, con las piernas flexionadas.','8_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (59,1,8,2,'Colocar el cómodo debajo de las nalgas.','7_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (60,1,8,3,'Con ayuda de la esponja enjabonada, limpie el pene y los testículos.','8_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (61,1,8,4,'Retirar la piel del prepucio y limpiar el glande con delicadeza.','8_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (62,1,8,5,'Deje caer el chorro de agua para enjuagar','8_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (63,1,8,6,'Devolver la piel del prepucio a su lugar','8_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (64,1,8,7,'Con una toalla secar suavemente, enfatizando los pliegues.','8_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (65,1,8,8,'Retirarse los guantes y lavarse las manos','7_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (66,1,9,1,'Colocarse un par de guantes desechables limpios','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (67,1,9,2,'Colocar a la persona en decúbito lateral','9_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (68,1,9,3,'Colocar el pañal limpio en la cama, la parte posterior a la altura de la cintura','9_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (69,1,9,4,'Enrollar uno de los extremos del pañal','9_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (70,1,9,5,'Colocar a la persona en decúbito supino','9_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (71,1,9,6,'Estirar el extremo enrollado del pañal','9_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (72,1,9,7,'Meter el pañal entre las piernas y ajustarlo','9_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (73,1,9,8,'Pegar los adhesivos','9_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (74,1,10,1,'Cambiarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (75,1,10,2,'Colocar a la persona en posición de fowler','30_41');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (76,1,10,3,'Colocar una toalla sobre su tórax','10_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (77,1,10,4,'Aplicar pasta dental en el cepillo','10_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (78,1,10,5,'Comenzar a cepillar los dientes superiores con movimientos de arriba abajo, los dientes inferiores de abajo hacia arriba y, las muelas con movimientos circulares en la parte superior','10_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (79,1,10,6,'Permitir a la persona enjuagar las veces que sea necesario','10_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (80,1,10,7,'Sostener la riñonera a una altura adecuada, para que la persona pueda vaciar cómodamente el agua de enjuague','10_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (81,1,10,8,'Secar con una toalla','10_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (82,1,11,1,'Colocar los pies de forma recta y realizar el corte (a opción de la persona)','11_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (83,1,11,2,'En las manos será de forma curva','11_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (84,1,11,3,'Si es necesario limarlas.','11_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (85,1,12,1,'Usar 15 centímetros de hilo dental, enrollando un extremo alrededor del dedo medio de una mano y, enrollando el resto en el dedo medio de la mano opuesta','12_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (86,1,12,2,'Sujetar el hilo firmemente entre el pulgar y el dedo índice de cada mano, e introducirla suavemente entre los dientes','12_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (87,1,12,3,'Colocar el hilo doblado en forma de “c” contra el costado del diente','12_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (88,1,12,4,'Pasar el hilo suavemente de arriba hacia abajo, manteniéndolo apretado contra el diente. No dar tirones al hilo ni cortarlo.','12_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (89,1,12,5,'Limpiar todos los dientes con el hilo. No olvidar usar el hilo en la parte posterior de los dientes.','12_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (90,1,13,1,'Si utiliza prótesis dental recibirla en una toalla desechable y colocarla en el recipiente','13_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (91,1,13,2,'Lavar la prótesis con cepillo y pasta al chorro de agua','13_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (92,1,13,3,'Ofrecer la prótesis a la persona y colocarla nuevamente','13_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (93,1,14,1,'Colocar la alfombra en el piso o en la tina para evitar caídas.','14_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (94,1,14,2,'Regular la temperatura del agua a tolerancia de la persona','14_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (95,1,14,3,'Ayudar a sentarse en la bañera o en la silla,si es necesario.','14_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (96,1,14,4,'Supervisar a la persona en el baño, y asistirlo en lo que no pueda hacer.','14_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (97,1,14,5,'Secar con una toalla de manera céfalo-caudal y con énfasis en los pliegues.','14_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (98,1,14,6,'Ayudar a la persona a vestirse (solo si lo requiere).','14_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (99,1,14,7,'Dejar en su lugar el material.','66_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (100,1,15,1,'Cubrir con una toalla grande el tórax, para evitar que se moje la ropa y/o sábanas si permanece en la cama','15_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (101,1,15,2,'Colocar sobre el rostro una toalla húmeda (agua caliente a tolerancia de la persona), durante cinco minutos','15_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (102,1,15,3,'Aplicar jabón o espuma con la brocha,sobre el rostro','15_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (103,1,15,4,'Inicie desde las patillas con breves movimientos hacia abajo, afeitar en dirección del crecimiento del vello','15_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (104,1,15,5,'Alrededor de nariz, boca y cuello, estirar la piel y afeite con mucho cuidado,deslizando el rastrillo suavemente','15_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (105,1,15,6,'Enjuagar el rastrillo con agua tibia para retirar los restos del vello, continuar afeitando hasta terminar','15_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (106,1,15,7,'Finalmente, enjuagar el rostro,retirando todos los residuos de jabón','15_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (107,1,15,8,'Secar con una toalla facial','15_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (108,1,15,9,'Aplicar loción para después de afeitar, con las palmas de las manos (solo si la persona lo desea)','15_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (109,1,15,10,'Dejar a la persona en una posición cómoda, si permanece en la cama','30_41');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (110,1,16,1,'Lavarse las manos y colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (111,1,16,2,'Colocar el material a utilizar en una silla o en los pies de la cama,en el orden en que se irá ocupando','16_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (112,1,16,3,'Colocar la cama en posición horizontal','16_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (113,1,16,4,'Quitar la colcha, cobija y sábanas, colocarlas en la bolsa','16_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (114,1,16,5,'Quitar la almohada','16_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (115,1,16,6,'Extienda la sábana de cajón sobre la cama, (deberá de quedar bien extendida para evitar pliegues)','16_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (116,1,16,7,'Colocar la sábana plana desdoblándola desde la cabecera hasta los pies El dobladillo ancho, se pondrá al nivel del borde superior del colchón','16_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (117,1,16,8,'Extender la cobija sobre la cama. Remeter los pies y hacer las esquinas','16_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (118,1,16,9,'Extender la colcha','16_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (119,1,16,10,'Realizar el dobladillo de la sábana plana sobre la cobija y la colcha, procurando que quede a una altura suficiente para cubrir los hombros de la persona','16_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (120,1,16,11,'Cambiar la funda de la almohada y colocarla en la cabecera de la cama','16_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (121,1,16,12,'Recoger todo el material y colocarlo en su lugar','16_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (122,1,16,13,'Quitarse los guantes y lavarse las manos','7_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (125,1,18,1,'Saludar a la persona e informarle sobre el procedimiento que se va a realizar','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (126,1,18,2,'Lavarse las manos y colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (127,1,18,4,'Retirar la colcha y la cobija','16_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (128,1,18,5,'Para cuidar la integridad de la persona, se cubrirá únicamente con la sábana','18_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (129,1,18,6,'Se colocará a la persona en decúbito lateral.','18_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (130,1,18,7,'Retirar la almohada','16_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (131,1,18,8,'Soltar la sábana de cajón y la plana, doblándolas hacia el cuerpo de la persona','18_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (132,1,18,9,'Colocar las sábanas limpias solo de ese lado, comenzando por la sábana de cajón luego la sábana plana','18_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (133,1,18,10,'Pedirle a la persona moverse hacia el lado en el que se encuentra usted','18_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (134,1,18,11,'Una vez instalada la persona en el lado limpio y ordenado, trasladarse al lado opuesto de la cama','18_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (135,1,18,12,'Retirar las dos sábanas sucias, limpiar colchón de migas u objetos','16_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (136,1,18,13,'Termine de tender las sábanas','18_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (137,1,18,14,'Ayudar a la persona a trasladarse al centro de la cama colocándole la almohada','18_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (138,1,18,15,'Continuar tendiendo la cobija y la colcha','18_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (139,1,18,16,'Agradecer a la persona por su cooperación y retirar el material sucio colocándolo en su lugar','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (140,1,18,17,'Retirarse los guantes y lavarse las manos','7_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (141,1,19,1,'Existen varios sitios para tomarlo: carotideo, axilar, braquial, radial, femoral, poplíteo, pedio y tibial posterior. El más utilizado es el radial.','19_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (142,1,19,2,'La persona debe estar cómoda con la extremidad apoyada o, sostenida con la palma hacia arriba','19_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (143,1,19,3,'Presione suavemente con los pulpejos (yemas) de su dedo índice, medio y anular en el punto en que la arteria pasa por encima de hueso (parte externa de la muñeca)','19_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (144,1,19,4,'Cuente los latidos durante un minuto, verificando la fase de ascenso y descenso, ritmo, frecuencia y amplitud','19_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (145,1,19,5,'Registre e interprete (de 16 a 59 años debe de estar en 60 y 80, de 60 años o mas  	40 y 80)','19_5');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (146,1,20,1,'Lo más cómodo posible y sin alertar a la persona, mire los movimientos torácicos','30_27');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (147,1,20,2,'Cuente las respiraciones (movimiento de inspiración y espiración) durante 1 minuto Se observa frecuencia, ritmo y profundidad','20_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (148,1,20,3,'Registre el dato, interprete  (de 16 a 59 años 16 a 20, de 60 años o mas 14 a 16)','20_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (149,1,20,4,'La respiración es el único signo vital que se puede modificar con voluntad propia','20_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (150,1,21,1,'Idealmente, la persona debe estar descansada, acostada o sentada Ubicar el brazo apoyado en su cama o mesa en posición supina','71_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (151,1,21,2,'Colocar el esfigmomanómetro de manera que la escala sea visible','21_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (152,1,21,3,'Fijar el brazalete alrededor del brazo, con el borde inferior 2.5 cm por encima de la articulación del codo, altura que corresponda a la del corazón, evitando excesiva presión del brazo','21_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (153,1,21,4,'Verificar que no tenga aire la cámara del brazalete','21_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (154,1,21,5,'Busque el pulso radial','21_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (155,1,21,6,'Coloque los dedos índice y medio en el pulso radial y comience a insuflar la cámara hasta que deje de sentir el pulso','21_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (156,1,21,7,'Mida en la columna de mercurio o el reloj, el momento donde dejó de sentir las pulsaciones y desinfle totalmente el manguito en forma rápida y continua.','21_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (157,1,21,9,'Con los pulpejos de los dedos medio e índice, localizar el pulso humoral, coloque el estetoscopio en este lugar, procurando no quede por abajo del brazalete, sólo que toque la piel sin presionar','21_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (158,1,21,10,'Sostener la perilla de goma con la otra mano y cerrar la válvula','21_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (159,1,21,11,'Mantener el estetoscopio sobre la arteria, con los dedos índice y medio','21_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (160,1,21,12,'Realizar la acción de bombeo o llenado de la cámara con la perilla, e insuflar aumentando 30 a 40 mmHg más de la medida obtenida en el pasó 6','21_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (161,1,21,13,'Abra cuidadosamente la válvula y dejar que el aire escape lentamente (2 a 4 mmHg por segundo)','21_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (162,1,21,14,'Escuchar con atención el primer latido claro y rítmico de Hg y hacer la lectura Esta cifra es la presión sistólica','21_14');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (163,1,21,15,'Siga abriendo la válvula para que el aire escape lentamente y mantenga la mirada fija en la columna de mercurio o reloj Escuchar cuando el sonido agudo cambia por un golpe fuerte y amortiguado Este último sonido claro es la presión diastólica','21_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (164,1,21,16,'Abrir completamente la válvula, dejando escapar todo el aire del brazalete y retirarlo','21_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (165,1,21,17,'Si necesita confirmar los valores obtenidos, puede repetir el procedimiento 5 ó 10 minutos después','29_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (166,1,21,18,'Registrar las anotaciones correspondientes y tomar las conductas pertinentes a los y tomar las conductas pertinentes a los hallazgos.(16 a 59  años de 120 sobre 80 mmHg mas o menos  10 , 60 años  o mas  80 sobre 50 o 139 sobre 89 mmHg).','21_18');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (167,1,22,1,'Colocar la silla a un lado de la cama, con los frenos puestos y retirar los reposapiés','22_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (168,1,22,2,'Una vez que la persona se encuentre recostada sobre la cama, el cuidador se colocará a un costado de ésta, con las piernas ligeramente separadas, colocando una pierna por delante y las rodillas semi flexionadas creando un punto de apoyo','22_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (169,1,22,3,'Posteriormente, se usará la sábana que se encuentra debajo de la persona para jalarla a un costado de la cama, usando el cuerpo del cuidador como barrera para evitar una caída','22_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (170,1,22,4,'Movilizar las piernas de la persona hacia abajo, a un costado de la cama','22_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (171,1,22,5,'Pedir la colaboración de la persona si es posible, para que se sostenga del cuidador rodeándolo del cuello con sus brazos, a manera que sirva de sostén','22_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (172,1,22,6,'El cuidador sostendrá a la persona, tomándolo por la espalda a la altura de los hombros, para poder levantarlo usando el cuerpo del cuidador como contrapeso, realizando un giro hasta que quede sentado en la orilla de la cama','22_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (173,1,22,7,'Tomar un tiempo para verificar que la persona que recibe la intervención, no se sienta mareada en lo que se coloca el calzado pertinente.','22_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (174,1,22,8,'Una vez que la persona se encuentra sentada, el cuidador deberá sostenerla rodeándola por la cintura con sus brazos y, se le pedirá que se sostenga nuevamente del cuello del cuidador, posteriormente, se levantará a la persona girando su cuerpo hacia la silla','22_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (175,1,22,9,'Una vez colocada en la silla, se reacomodará para que quede sentado correctamente','22_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (176,1,23,1,'Colocar la silla de ruedas a un lado de la cama, con los frenos puestos y retirar los reposapiés','23_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (177,1,23,2,'El cuidador se colocará frente a la persona, con las piernas ligeramente separadas colocando una pierna por delante y las rodillas semi flexionadas, creando un punto de apoyo','23_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (178,1,23,3,'El cuidador sostendrá a la persona, colocando sus brazos por debajo de los brazos, a manera que quede abrazándolo mientras que se sostiene del cuidador rodeando su cuello con sus brazos','23_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (179,1,23,4,'Posteriormente, se realizará un giro hacia la cama con la persona, evitando cargar el peso con la columna','23_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (180,1,23,5,'Se sentará a la persona sobre la orilla de la cama y el cuerpo del cuidador, actuará como barrera para evitar una caída','23_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (181,1,23,6,'Ayudaremos a la persona a recostarse sobre la cama, elevando sus piernas al mismo tiempo que se gira el torso','23_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (182,1,23,7,'Una vez que la persona se encuentre recostada, se acomoda hasta que quede en la posición deseada','23_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (183,1,24,1,'Colocar la silla de ruedas junto al escusado a 90°','24_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (184,1,24,2,'Poner el freno de las llantas de la silla de ruedas y levantar los reposapiés','24_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (185,1,24,3,'El cuidador deberá colocar sus brazos bajo los brazos de la persona, sosteniéndola por la espalda','24_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (186,1,24,4,'Pedirle que se sostenga de las barras auxiliares (si es que se encuentran instaladas), mientras se le retira la ropa','24_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (187,1,24,5,'Ayudar a bajarla al asiento del escusado','24_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (188,1,24,6,'El cuidador deberá explicarle que se encontrará cerca por si necesita algo','29_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (189,1,24,7,'Cerrar la puerta para brindar privacidad','72_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (190,1,24,8,'Ayudar con la limpieza, solo en caso de que se requiera','28_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (191,1,25,1,'El automóvil deberá estar estacionado con la puerta abierta','25_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (192,1,25,2,'Colocar la silla de ruedas lo más cercano al asiento de su preferencia, con los frenos puestos y retirar el reposapiés','25_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (193,1,25,3,'El cuidador deberá levantarla colocando sus brazos bajo los brazos de la persona, mientras que se sostiene del cuidador, para tener apoyo','25_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (194,1,25,4,'El cuidador levantará a la persona para transferirla al asiento del vehículo','25_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (195,1,25,5,'Ayudarle a sentarse, acomodarla y subir sus pies al vehículo','25_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (196,1,25,6,'El cuidador acomodará a la persona en el asiento y colocará el cinturón de seguridad','25_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (197,1,26,1,'Informar el procedimiento a la persona','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (198,1,26,2,'Lavarse las manos y colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (199,1,26,3,'Colocar a la persona en decúbito supino con el abdomen descubierto','2_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (200,1,26,4,'Proteger la cama con un hule protector (El modo para cambiar el sistema de bolsa, depende del tipo de sistema que utiliza)','26_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (202,1,26,5,'Pasos básicos para vaciar o cambiar el sistema de bolsa: 1)Tenga a la mano todos los elementos','26_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (203,1,26,6,'2)Retire el sistema de bolsa que tiene colocado de arriba hacia abajo','26_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (204,1,26,7,'3) Limpie el área con agua tibia y jabón neutro y la esponja desechable','28_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (205,1,26,8,'4) Seque suavemente la piel periestomal con gasas esteriles, dando pequeños toquecitos (puntilleo), evitando tallar o presionar','26_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (206,1,26,9,'Para vaciar una bolsa drenable: 1) Abra la parte inferior de la bolsa y vacíe las heces en un recipiente y posteriormente en el inodoro','26_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (207,1,26,10,'2) Limpie el extremo de la bolsa con gasas','26_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (208,1,26,11,'3) Cierre bien la abertura de la bolsa','26_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (209,1,26,12,'4) Si utiliza un sistema de bolsa de dos piezas, vuelva a acoplar la bolsa','26_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (210,1,26,13,'Para cambiar una bolsa drenable:1) Quitar la bolsa, de arriba hacia abajo, si utiliza un sistema de dos piezas, sostenga cuidadosamente el protector cutáneo','26_14');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (211,1,26,14,'2) Coloque la bolsa de ostomía adentro de otras dos bolsas comunes, cada una cerrada individualmente','26_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (212,1,26,15,'3) Arroje la bolsa a la basura (nunca intente arrojarla por el inodoro)','1_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (213,1,26,16,'4) Realice el aseo correspondiente y seque','26_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (214,1,26,17,'5) Para colocar la nueva bolsa se debe medir la estoma y marcarlo en la parte que contiene el protector cutáneo (antes de retirar el papel que lo cubre), ya que el estoma debe entrar y mantener un espacio no mayor a 2 milímetros de espacio entre la piel y el protector Para mejorar la adhesión del protector, frote entre sus manos, antes de retirar el papel que lo cubre, después retire el papel y coloque el dispositivo de abajo hacia arriba.','26_24');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (215,1,26,18,'Para cambiar una bolsa cerrada o no drenable:1) Retire la bolsa, si utiliza un sistema de dos piezas, sostenga cuidadosamente el protector cutáneo','26_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (216,1,26,19,'2) Coloque la bolsa de ostomía adentro de otras dos bolsas comunes, cada una cerrada individualmente','26_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (217,1,26,20,'3) Arroje la bolsa a la basura (nunca intente arrojarla por el inodoro)','26_21');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (218,1,26,22,'4) Coloque una nueva bolsa','26_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (219,1,26,23,'Muchas bolsas tienen filtros que evitan que se “infle” cuando se llena de gases','26_23');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (220,1,26,24,'El filtro también ayuda a disminuir el olor','26_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (221,1,26,25,'Es recomendable que cubra el filtro cuando nade o se duche','26_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (222,1,26,26,'Limpieza del estoma y la piel circundante: Si existe edema y suturas, se puede utilizar solución inyectable y gasas estériles. Posteriormente, utilizar esponja, con agua templada y jabón neutro.','26_27');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (223,1,26,27,'Secar la piel cuidadosamente, presionando con suavidad, no frotar, por puntilleo','26_27');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (224,1,26,28,'Si es posible, deje la piel expuesta al aire durante unos minutos.En el caso de la urostomía, mientras se coloca el disco y para evitar que la orina moje la piel, se puede colocar una gasa en el orificio de la estoma.','26_28');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (225,1,26,29,'Observar la estoma y la piel circundante en busca de enrojecimiento, irritación, involución, prolapso, heridas o necrosis','26_29');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (226,1,26,30,'Si existiera vello alrededor de la estoma, recortarlo con tijeras, nunca rasurar','26_30');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (227,1,26,31,'Una vez preparada la piel y bien seca, medir el estoma y marcarlo en la parte que contiene el protector cutáneo (antes de retirar el papel que lo cubre), ya que  el estoma debe entrar y mantener un espacio no mayor a 2 milímetros de espacio entre la piel y el protector Para mejorar la adhesión del protector, frote entre sus manos, antes de retirar el papel que lo cubre, después retire el papel y coloque el dispositivo de abajo hacia arriba, presionando suave pero firmemente.','26_31');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (228,1,26,32,'Colocar la bolsa colectora, adaptándola en el aro de enganche del disco adhesivo','26_32');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (229,1,26,33,'Cierre el clíper, presionando sus extremos hasta oír un “clik”','26_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (230,1,26,34,'Asegurarse que la bolsa quede herméticamente cerrada, tirando de ella con las dos manos suavemente','26_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (231,1,26,35,'Verificar que la válvula inferior de drenaje esté cerrada','26_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (232,1,26,36,'Cambiar la bolsa o el disco hasta que presente fuga','26_36');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (233,1,27,1,'Informar el procedimiento a la persona','27_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (234,1,27,2,'Lavarse las manos y colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (235,1,27,3,'Colocar a la persona en decúbito supino con el abdomen descubierto','2_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (236,1,27,4,'Proteger la cama con un hule protector','27_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (237,1,27,5,'El modo para cambiar el sistema de bolsa, depende del tipo de sistema que utiliza','27_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (238,1,27,6,'Pasos básicos para vaciar o cambiar el sistema de bolsa: 1) Tenga a la mano todos los elementos','27_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (239,1,27,7,'2) Quite el sistema de bolsa que tiene colocado de arriba hacia abajo','27_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (240,1,27,8,'3) Limpie el área con agua tibia y jabón neutro y la esponja desechable','27_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (241,1,27,9,'4) Seque suavemente la piel periestomal con gasas esteriles, dando pequeños toquecitos (puntilleo), evitando tallar o presionar','27_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (242,1,27,10,'Para vaciar una bolsa drenable: 1) Abra la parte inferior de la bolsa y vacíe las heces en un recipiente y posteriormente en el inodoro','27_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (243,1,27,11,'2) Limpie el extremo de la bolsa con gasas','27_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (244,1,27,12,'3) Cierre bien la abertura de la bolsa','27_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (245,1,27,13,'4) Si utiliza un sistema de bolsa de dos piezas, vuelva a acoplar la bolsa','27_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (246,1,27,14,'Para cambiar una bolsa drenable:1) Quite la bolsa, de arriba hacia abajo, si utiliza un sistema de dos piezas, sostenga cuidadosamente el protector cutáneo','27_14');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (247,1,27,15,'2) Coloque la bolsa de ostomía adentro de otras dos bolsas comunes, cada una cerrada individualmente','27_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (248,1,27,16,'3) Arroje la bolsa a la basura (nunca intente arrojarla por el inodoro)','27_16');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (249,1,27,17,'4) Realice el aseo correspondiente y seque','27_17');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (250,1,27,18,'5) Para colocar la nueva bolsa se debe medir la estoma y marcarlo en la parte que contiene el protector cutáneo (antes de retirar el papel que lo cubre), ya que el estoma debe entrar y mantener un espacio no mayor a 2 milímetros de espacio entre la piel y el protector','27_18');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (251,1,27,19,'Para cambiar una bolsa cerrada o no drenable:1) Quite la bolsa, si utiliza un sistema de dos piezas, sostenga cuidadosamente el protector cutáneo','27_19');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (252,1,27,20,'2) Coloque la bolsa de ostomía adentro de otras dos bolsas comunes, cada una cerrada individualmente','27_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (253,1,27,21,'3) Arroje la bolsa a la basura (nunca intente arrojarla por el inodoro)','27_21');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (254,1,27,22,'4) Coloque una nueva bolsa','27_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (255,1,27,23,'Muchas bolsas tienen filtros que evitan que se “infle” cuando se llena de gases','27_23');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (256,1,27,24,'El filtro también ayuda a disminuir el olor','27_24');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (257,1,27,25,'Es recomendable que cubra el filtro cuando nade o se duche','27_25');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (258,1,27,26,'Limpieza del estoma y la piel circundante: Si existe edema y suturas, se puede utilizar solución inyectable y gasas estériles','27_26');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (259,1,27,27,'Secar la piel cuidadosamente, presionando con suavidad, no frotar, por puntilleo','27_27');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (260,1,27,28,'Si es posible, deje la piel expuesta al aire durante unos minutos','27_28');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (261,1,27,29,'Observar la estoma y la piel circundante en busca de enrojecimiento, irritación, involución, prolapso, heridas o necrosis','27_29');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (262,1,27,30,'Si existiera vello alrededor de la estoma, recortarlo con tijeras, nunca rasurar','27_30');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (263,1,27,31,'Una vez preparada la piel y bien seca, medir el estoma y marcarlo en la parte que contiene el protector cutáneo (antes de retirar el papel que lo cubre), ya que el estoma debe entrar y mantener un espacio no mayor a 2 milímetros de espacio entre la piel y el protector','27_31');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (264,1,27,32,'Colocar la bolsa colectora, adaptándola en el aro de enganche del disco adhesivo','27_32');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (265,1,27,33,'Cierre el clíper, presionando sus extremos hasta oír un “clik”','27_33');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (266,1,27,34,'Asegurarse que la bolsa quede herméticamente cerrada, tirando de ella con las dos manos suavemente','27_34');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (267,1,27,35,'Verificar que la válvula inferior de drenaje esté cerrada','27_35');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (268,1,27,36,'Cambiar la bolsa o el disco hasta que presente fuga','27_36');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (269,1,28,1,'Si la persona colabora, pídale que se coloque en decúbito supino con las rodillas flexionadas, apoyando los talones sobre la cama y levantando la cadera','28_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (270,1,28,2,'Si está débil, ayúdelo a quitarse la bata, pijama y ropa interior, explíquele la posición anterior, colaborando con él y procure que descienda con suavidad sobre el cómodo','28_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (271,1,28,3,'Si no puede colaborar, hágala girar en decúbito lateral, ponga el cómodo contra las nalgas y sosténgalo en su sitio, a medida que al dar la vuelta quede sobre éste','28_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (272,1,28,4,'Permita a la persona limpiarse por sí misma','28_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (273,1,28,5,'En caso de que no lo pueda realizar, es necesario el aseo de la zona perianal/genital (en la mujer siempre limpiar el periné de adelante hacia atrás)','28_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (274,1,28,6,'Retire el cómodo','28_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (275,1,28,7,'Asegurarse de que la persona quede en posición cómoda y correcta','28_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (276,1,28,8,'Ofrezca agua, jabón y toalla para el aseo de las manos. Deposite en el inodoro el contenido del orinal, retírese los guantes y lávese las manos.','28_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (279,1,29,1,'Si la persona no colabora o no puede, coloque el orinal de modo que el pene quede dentro de éste','29_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (280,1,29,2,'Ayúdelo sosteniendo el orinal si es necesario','29_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (281,1,29,3,'Cubrir y evitar la exposición de la persona','29_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (282,1,29,4,'Si está débil o confusa, permanezca cerca en caso de que necesite ayuda','29_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (283,1,29,5,'Permita a la persona limpiarse por sí misma','29_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (284,1,29,6,'En caso de que no lo pueda realizar, es necesario el aseo del pene para evitar la humedad','28_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (285,1,29,7,'Retire el orinal','29_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (286,1,29,8,'Asegurarse de que la persona quede en posición cómoda y correcta','30_41');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (287,1,29,9,'Ofrezca agua, jabón y toalla para el aseo de las manos. Deposite en el inodoro el contenido del orinal, retírese los guantes y lávese las manos.','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (289,1,30,1,'Cuidados generales Verificar que no presente fuga de orina, en cuyo caso revisar que el globo esté perfectamente inflado (la capacidad del globo viene impresa en la sonda) En caso de presentar fuga, avisar al médico','30_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (290,1,30,2,'Mantener la sonda siempre fija','30_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (291,1,30,3,'Verificar la permeabilidad','30_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (292,1,30,4,'Evitar que se doble','30_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (293,1,30,5,'Mantener la bolsa colectora de orina, siempre por debajo del nivel de la vejiga','30_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (294,1,30,6,'Utilizar siempre un sistema colector cerrado','30_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (295,1,30,7,'Vaciar la bolsa colectora cada 8 horas o antes, si es necesario','30_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (296,1,30,8,'Vigilar la coloración y el aspecto de la orina','30_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (297,1,30,9,'Anotar la fecha de colocación, retiro y cambio de la sonda en la bitácora','30_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (298,1,30,10,'Realizar control de la ingesta/ diuresis','30_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (299,1,30,11,'Realiza limpieza de la sonda y área circundante una vez al día','30_46');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (301,1,217,1,'Informar el procedimiento a la persona','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (302,1,217,2,'Tener el material a la mano (cómodo u orinal y guantes)','30_24');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (303,1,217,3,'Lavarse las manos','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (304,1,217,4,'Colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (305,1,217,5,'Colocar a la persona en decúbito supino','2_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (306,1,217,6,'Retirar el tapón del tubo de drenaje','30_28');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (307,1,217,7,'Colocar el tubo de drenaje de la bolsa colectora dentro del cómodo u orinal, abrir la pinza y permitir que salga por gravedad','30_19');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (308,1,217,8,'Al terminar el drenaje, cerrar la pinza y colocar el tapón','30_30');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (309,1,217,9,'Llevar el orinal o cómodo al baño, medir la cantidad extraída y anotarlo en la bitácora','30_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (311,1,218,1,'En caso de ser hombre, la sonda se fija en la cara anterior del muslo','30_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (312,1,218,2,'En la mujer, la sonda se fija en la cara interna del muslo','30_37');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (313,1,218,3,'Cortar una tira de tela adhesiva, de aproximadamente 15 cm de largo, doblarla sobre sí misma por el área del adhesivo','30_32');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (314,1,218,4,'Realizar dos ranuras al centro del apósito de fijación y, pasar la cinta hecha con la tela adhesiva a través de ellas','30_33');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (315,1,218,5,'Con una gasa húmeda limpiar la zona donde se fijará la sonda, posteriormente, secar','30_44');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (316,1,218,6,'Rociar benjuí sobre la piel limpia y esperar a que seque','30_35');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (317,1,218,7,'Retirar el protector del apósito, pegar el apósito en el área donde se impregnó el benjuí','30_36');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (318,1,218,8,'Anudar la sonda con la cinta, en el tubo donde se encuentra el globo','30_32');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (320,1,219,1,'Cuando la persona se baña diario, indicar que lave toda el área alrededor del sitio donde entra la sonda, posteriormente, secar con una toalla','14_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (321,1,219,2,'Informar el procedimiento a la persona','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (322,1,219,3,'Lavarse las manos','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (323,1,219,4,'Colocarse los guantes','30_35');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (324,1,219,5,'Colocar a la persona en decúbito supino con los genitales descubiertos','30_36');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (325,1,219,6,'Colocar un cómodo bajo la cadera de la persona','7_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (326,1,219,7,'Mojar una gasa con agua caliente y ponerle jabón líquido','30_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (327,1,219,8,'Lavar suavemente toda el área alrededor del sitio donde entra la sonda con la gasa enjabonada.Las mujeres deben limpiarse desde el frente hacia atrás y los hombres desde la punta del pene hacia abajo.','30_21');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (328,1,219,9,'Con otra gasa húmeda y con jabón, lavar suavemente la parte superior de las piernas y las nalgas','30_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (329,1,219,10,'Enjuagar el jabón con agua tibia y secar con la toalla','30_22');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (330,1,219,11,'No usar cremas, polvos ni aerosoles cerca de esta área','30_50');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (331,1,219,12,'Retirar el cómodo y vaciar su contenido en el baño, excepto las gasas que se tiran en el cesto de basura','30_43');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (335,1,31,1,'Una vez que la persona que reciba la intervención, se encuentre sentada:','37_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (336,1,31,2,'Colocará sus brazos a los laterales','31_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (337,1,31,3,'Posteriormente, realizará un movimiento de abduccion,por un segundo.','31_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (338,1,31,4,'Al finalizar este movimiento, hará un movimiento de aducción','31_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (339,1,31,5,'Conforme se vaya observando evolución en la persona que recibe la intervención, se pueden ir implementando pesas, para un mayor fortalecimiento','31_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (340,1,32,1,'Ya sea que la persona que recibe la intervención se encuentre de pie o sentada:','37_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (341,1,32,2,'La persona pondrá sus brazos a los laterales','31_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (342,1,32,3,'Se comenzará con un movimiento de extensión, movimiento que tendrá una duración aproximada de 2 segundos. Llevando los brazos hacia atrás,hasta alcanzar un ángulo de 45°','32_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (343,1,32,4,'Posteriormente, realizará un movimiento flexión,bajando los brazos a la altura de los hombros, llevándolos por el frente a la altura de la cabeza,realizando un ángulo de 180°','32_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (344,1,33,1,'La persona que recibe la intervención, realizará abducción de brazos','33_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (345,1,33,2,'Manteniendo los brazos en esta posición, comenzará a desarrollar movimientos en circunducción Estos movimientos deben ser de forma lenta','33_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (346,1,34,1,'La persona que recibe la intervención pondrá su articulación del hombro en flexión','34_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (347,1,34,2,'Posteriormente, realizará un movimiento de flexión de codo','34_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (348,1,34,3,'A continuación, llevará a cabo un movimiento de extensión','34_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (349,1,35,1,'El paciente coloca su articulación del codo en flexión','35_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (350,1,35,2,'Realiza un movimiento de supinación','35_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (351,1,35,3,'Al terminar, lleva a cabo un movimiento de pronación','35_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (352,1,36,1,'Colocar el antebrazo al borde de una mesa, en posición supina, dejando caer el peso de la mano, a la orilla de ésta','36_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (353,1,36,2,'Posteriormente, moverá la mano hacia arriba, lentamente, esperará un segundo realizando esta acción','36_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (354,1,36,3,'Deberá alinear la mano con el brazo','36_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (355,1,36,4,'Regresará la mano a su posición inicial','36_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (356,1,36,5,'Realizará un movimiento contrario (hacia abajo), movimiento que durará un segundo','36_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (358,1,37,1,'La persona que recibe la intervención, colocará sus manos en una superficie plana (mesa)','37_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (359,1,37,2,'Éstas deberán encontrarse en posición recta y en pronación','37_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (360,1,37,3,'Coordinando ambas manos, el paciente realizará, lentamente, un movimiento de abducción','37_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (361,1,37,4,'Posteriormente, regresará las manos a su posición inicial','37_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (362,1,37,5,'Ahora, la persona que recibe la intervención realizará, igualmente, de forma lenta, un movimiento de aducción','37_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (363,1,38,1,'Se le proporcionará a la persona que recibe la intervención, una pelota de esponja, la cual tomará sin realizar presión alguna','38_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (364,1,38,2,'Al momento que se le indique, comenzará a presionar la pelota','38_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (365,1,38,3,'La presión que realice deberá durar dos segundos','38_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (366,1,38,4,'Posteriormente, dejará de ejercer presión, pero no soltará la pelota','38_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (367,1,39,1,'La persona se colocará en posición de bipedestación erecta o decúbito lateral','40_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (368,1,39,2,'En caso de que lo realice en bipedestación erecta, apoyará sus manos en unas barras paralelas','39_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (369,1,39,3,'Cuando la persona ya se haya apoyado, comenzará con un movimiento de flexión de cadera, estirando la pierna hacia al frente; este movimiento debe ser realizado de forma lenta, respetando el arco de movilidad que se tiene','39_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (370,1,39,4,'Posteriormente, regresará a su posición inicial','39_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (371,1,39,5,'Ahora, realizará un movimiento de extensión de cadera, estirando la pierna hacia atrás; este movimiento, también debe ser de forma lenta y respetando los arcos de movilidad de la persona','39_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (372,1,39,6,'Este ejercicio, debe ser realizado con ambos pies: primero será con uno y después alternando ambos','39_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (373,1,40,1,'La persona que recibirá la intervención, se colocará en posición de bipedestación erecta o decúbito lateral','40_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (374,1,40,2,'En caso de realizar el ejercicio en bipedestación erecta, apoyará sus manos en una barra paralela','39_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (375,1,40,3,'Realizará un movimiento de abducción de cadera; este movimiento debe ser realizado de forma lenta y respetando el arco de movilidad de la persona que recibe la intervención','40_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (376,1,40,4,'Posteriormente, realizará un movimiento de aducción, para regresar a su posición inicial','40_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (377,1,40,5,'Este ejercicio, se puede realizar alternando ambas piernas, o bien, una a la vez','40_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (378,1,41,1,'La persona se colocará en posición de bipedestación erecta','39_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (379,1,41,2,'Apoyará sus manos en las barras paralelas','39_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (380,1,41,3,'Comenzará a realizar movimiento de rotación de cadera, de forma lenta y respetando sus arcos de movilidad','41_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (381,1,41,4,'Este ejercicio, lo realizará con ambas piernas, una a la vez','41_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (382,1,42,1,'La persona que recibe la asistencia, podrá colocarse en posición de bipedestación erecta o en posición sedente','42_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (383,1,42,2,'Si la persona se encuentra en posición de bipedestación erecta, deberá apoyarse de unas barras paralelas','42_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (384,1,42,3,'Cuando la persona se encuentre cómoda, comenzará, con una rodilla (derecha o izquierda) a realizar movimiento de flexión y extensión','42_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (385,1,42,4,'Estos movimientos deben ser realizados de forma lenta, y no rebasando el arco de movilidad de la persona','42_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (386,1,43,1,'una vez que la persona se encuentre sentada, realizará flexión y extensión de ambas extremidades al mismo tiempo','43_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (387,1,43,2,'Posteriormente, la realizará intercalándolas (la extremidad derecha flexionará y la izquierda extenderá y viceversa)','43_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (388,1,44,1,'Ya sea que la persona que recibe la intervención se encuentre sentada o en posición de bipedestación erecta, realizará aducción y abducción al mismo tiempo con ambas extremidades','44_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (389,1,44,2,'Posteriormente, la realizará intercalándolas (la extremidad derecha realizará abducción y la izquierda aducción y viceversa)','44_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (390,1,45,1,'una vez que la persona se encuentre sentada, flexionará y extenderá ambas extremidades al mismo tiempo','45_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (391,1,45,2,'Posteriormente, realizará el ejercicio alternando las extremidades (la extremidad derecha flexionará y la izquierda extenderá)','45_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (392,1,46,1,'una vez que la persona se encuentre sentada o en posición de bipedestación erecta (apoyándose de las Barras o de una mesa), comenzará a extender y flexionar las extremidades al mismo tiempo','46_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (393,1,46,2,'Posteriormente, se realizará lo mismo pero intercalando la extremidad superior y la inferior (la extremidad superior flexionará y la inferior extenderá)','46_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (394,1,47,1,'la persona debe estar en posición de bipedestación erecta, apoyada de las Barras o de una mesa','47_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (395,1,47,2,'comenzará realizando abducción y aducción de ambas extremidades','47_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (396,1,47,3,'Posteriormente, se realizará el movimiento intercalando las extremidades (la extremidad superior realizará abducción y la inferior aducción)','47_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (397,1,48,1,'la persona estará en posición de bipedestación erecta','48_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (398,1,48,2,'comenzará extendiendo y flexionando ambas extremidades','48_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (399,1,48,3,'Posteriormente, se realizará el mismo ejercicio pero intercalando las extremidades (la extremidad superior extenderá y la inferior flexionará)','48_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (400,1,49,1,'en la misma posición de bipedestación','40_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (401,1,49,2,'la persona realizará aducción y abducción con ambas extremidades','49_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (402,1,49,3,'Posteriormente, se realizará intercalándolas (la extremidad superior realizará abducción y la inferior aducción)','49_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (403,1,50,1,'se realiza con movimientos deslizantes circulares y semicirculares, con ambas manos','50_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (404,1,50,2,'de forma distal a proximal (de las extremidades hacia el centro del cuerpo)','50_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (405,1,50,3,'Mantener la dirección, ritmo y frecuencia constante','50_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (406,1,51,1,'se realiza aprehendiendo entre los dedos la masa muscular','51_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (407,1,51,2,'las zonas carnosas de los dedos y pulgares, hacen un movimiento circular de presión creciente en la parte superior y, decreciente en la parte inferior del círculo','51_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (408,1,51,3,'el pulgar se cierra y se aproxima a los otros cuatro dedos, para después aflojar, disminuyendo la presión','51_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (409,1,52,1,'La vibración debe ser suave y, es un movimiento tembloroso para transmitir una onda vibratoria a través de las manos o los dedos, siempre en contacto con la piel, una o ambas manos','52_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (410,1,52,2,'se realizan aproximadamente, 6 movimientos por segundo','52_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (411,1,53,1,'La fricción se aplica de forma rápida y se puede realizar friccionando músculos o tejidos con la mano, la palma, los nudillos o dedos pulgares, dependiendo de la zona, el tamaño y la forma del área que va a recibir la fricción','53_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (412,1,54,1,'Se realiza con las manos ligeramente ahuecadas en posición de concha (palmoteo cóncavo) con el talón de las manos ahuecadas, con un puño o los dos','54_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (413,1,54,2,'Moviéndose alternativamente y de forma rápida con flexo-extensiones de las muñecas','54_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (414,1,54,3,'También se realiza con la eminencia hipotenar (borde cubital de la mano y el 5º dedo)','54_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (415,1,55,1,'Frotar las palmas y colocarlas una junto de la otra, sobre el abdomen para iniciar el contacto (effleurage superficial)','55_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (416,1,55,2,'Masajee el abdomen en suaves círculos (effleurage profundo): coloque la mano plana una sobre la otra y, realice movimientos circulares en el sentido de las agujas del reloj','55_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (417,1,55,3,'Iniciar petrissage, colocando las palmas de las manos de derecha a izquierda y deslizarlas una contra la otra, ejerciendo poca presión','55_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (418,1,55,4,'Realizar vibración en el abdomen con los dedos ligeramente separados de izquierda a derecha, alrededor del ombligo, sacudiendo las manos con rápidas y pequeñas oscilaciones. Los dedos no se deslizan sobre la piel, sino que transmiten el movimiento directamente sobre la musculatura abdominal.','55_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (419,1,55,5,'Para finalizar, realice effleurage superficial, colocando nuevamente ambas manos planas y juntas sobre el abdomen','55_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (420,1,56,1,'Esparcir aceite en la zona a tratar (pie o mano)','56_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (421,1,56,2,'Realizar effleurage profundo de la siguiente manera:','56_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (422,1,56,3,'Se realizan movimientos deslizantes circulares y semicirculares con ambas manos','56_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (423,1,56,4,'De forma distal a proximal','56_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (424,1,56,5,'Mantener la dirección, ritmo y frecuencia constante','56_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (425,1,56,6,'Aplicar digitopresión, en puntos específicos, como se muestra en las cartografias','56_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (427,1,57,1,'la actividad se realiza de acuerdo a la imagen de la casilla, se pueden formar equipos o individual','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (428,1,57,2,'Preparar el tablero y los dados  ','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (429,1,57,4,'Mencionar el nombre del instrumento en su casilla','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (430,1,57,5,'Cuando toca Oca avanzar doble','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (431,1,57,6,'Saltar a la siguiente casilla, si cae en los dados','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (432,1,57,7,'Cuando cae en la cárcel, deberá esperar a que otro lo salve tomando su lugar','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (433,1,57,8,'si cae en la muerte, regresa al inicio del juego','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (434,1,57,9,'Cuando alguien cae en la misma casilla, éste se irá al lugar de otro','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (435,1,57,10,'Gana el equipo o persona que llegue primero al final','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (436,1,59,1,'Se mostrará el sonido que producen distintos tipos de instrumentos musicales a dos grupos de personas:','59_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (437,1,59,2,'Grupo A (activos o independientes) .Se muestran los diferentes instrumentos, mencionando el nombre de cada uno de ellos.','59_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (438,1,59,3,'Grupo B (pasivos o dependientes). Se muestran diferentes instrumentos, nombre y su sonido. Una vez que asocien nombre y sonido, se muestra el instrumento; una vez familiarizados con el sonido, el cuidador producirá un sonido sin ser visto y, los usuarios deberán de adivinar a qué instrumento corresponde.','59_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (441,1,60,1,'Se realizarán ejercicios de calentamiento que ayudarán a estimular la psicomotricidad','60_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (442,1,60,2,'Se proseguirá con los pasos de baile según sea el ritmo elegido, la rutina constará de 2 canciones.en grupo ,en pareja','60_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (443,1,60,3,'En el cierre de la rutina, se pone música de relajación y se realizan respiraciones profundas','60_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (444,1,60,4,'Por último, se realizará una retroalimentación preguntando a los participantes cómo se sintieron y qué tan difícil fue aprender los pasos de baile, esto ayuda a establecer rutinas posteriores','30_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (445,1,61,1,'Se acondiciona el área, donde se realizará el sensorama y se ameniza con música relajante de fondo','61_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (446,1,61,2,'Explicación de la actividad a los participantes','61_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (447,1,61,3,'Uno por Uno con los ojos vendados ó cerrados y acompañado de un guía, pasa por Cada mesa (estación)','61_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (448,1,61,4,'en la estación del gusto, podrá degustar Cada una de las cosas que se den a probar (identificar qué reacción Tiene Cada participante)','61_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (449,1,61,5,'en la estación del tacto, tocará objetos con diferentes texturas y formas','61_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (450,1,61,6,'en la estación del olfato, se darán a oler diferentes esencias y aromas','61_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (451,1,61,7,'en la estación del oído, podrá escuchar sonidos de objetos e instrumentos musicales','61_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (452,1,61,8,'por último, se pasa a la estación de la vista, el participante abrirá o se descubrirá los ojos, identificará una secuencia de dibujos e ilustraciones con diferentes colores','61_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (453,1,61,9,'al finalizar el recorrido, se reúne a los participantes y se realiza una retroalimentación para compartir experiencias y recuerdos','30_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (454,1,62,1,'Se abre el tapón de la sonda y se aspira con la jeringa por si hubiera restos','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (455,1,62,2,'Antes de proporcionar alimento, se lava la sonda conectando la jeringa, cargada con 30 centímetros cúbicos de agua','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (456,1,62,3,'Posteriormente, se tapa la sonda, hasta que se cargue el alimento','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (457,1,62,4,'Conectar la sonda con el equipo de ministración','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (458,1,62,5,'Una vez preparado el alimento, se abre el tapón de la sonda y se vacía el contenido de la jeringa de acuerdo a la prescripción médica .La entrada del alimento debe ser lenta, de acuerdo a la cantidad de alimento prescrito. El tiempo empleado debe ser como el de cualquier comida.','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (459,1,62,6,'Lavar la sonda con un poco de agua para evitar la adherencia de alimento','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (460,1,62,7,'Colocar el tapón al terminar','62_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (461,1,62,8,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (462,1,63,1,'Colocar la fórmula alimenticia en la bolsa para su administración e instalarla en el soporte a una altura de 30 cm sobre el punto de inserción de la sonda','63_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (463,1,63,2,'Verificar que no quede aire dentro de la bolsa donde se ministrará el alimento, y si es que hay aire, sacarlo por completo','63_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (464,1,63,3,'Conectar el tubo de la bolsa al dispositivo terminal de la sonda para alimentación, la cual permanece pinzada','63_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (465,1,63,4,'Despinzar la sonda para alimentación y dejar que fluya lentamente la fórmula alimenticia','63_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (466,1,63,5,'Regular la velocidad de la infusión prescrita','63_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (467,1,63,6,'Verificar que se mantenga la velocidad de la infusión según la prescripción','63_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (468,1,63,7,'Agitar la bolsa con el contenido de la fórmula alimenticia para favorecer que la mezcla permanezca con la misma consistencia, evitando la concentración de la misma y con ello, el taponamiento de la sonda','63_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (469,1,63,8,'Lavar la sonda con un poco de agua para evitar la adherencia de alimento','63_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (470,1,63,9,'Colocar el tapón al terminar','63_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (471,1,63,10,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (472,1,64,1,'Destapar el frasco que contiene la fórmula nutricional e insertar la boquilla del equipo al frasco','64_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (473,1,64,2,'Instalar el frasco en el soporte a una altura de 30 cm sobre el punto de inserción de la sonda','64_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (474,1,64,3,'Verificar que no se encuentre aire dentro del equipo','64_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (475,1,64,4,'Conectar el extremo de salida del equipo al dispositivo terminal de la sonda para alimentación, la cual debe permanecer pinzada','64_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (476,1,64,5,'Despinzar la sonda para alimentación y dejar que fluya a la velocidad programada','64_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (477,1,64,6,'Lavar la sonda con un poco de agua para evitar la adherencia de alimento','64_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (478,1,64,7,'Colocar el tapón al terminar','64_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (479,1,64,8,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (480,1,65,1,'Sacar del frasco o envoltura el medicamento','65_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (481,1,65,2,'Colocarlo directamente en el vaso graduado o el de papel','65_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (482,1,65,3,'Proporcionar a la persona el vaso con el medicamento e indicar que puede ingerirlo, a su vez, proporcionar el agua','65_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (483,1,65,4,'Verificar que la persona degluta la medicina','65_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (484,1,65,5,'En caso de dificultad para deglutir, moler el medicamento en el mortero y diluirlo en agua Puede ser ministrado con jeringa o en un vaso con agua','65_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (485,1,66,1,'Agitar el frasco de medicamento','66_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (486,1,66,2,'Destapar el frasco y verter la cantidad indicada en la cuchara, el vaso o aspirarla con la jeringa','66_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (487,1,66,3,'En caso de requerirlo, la persona podrá ingerir el vaso con agua','66_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (488,1,66,4,'Limpiar la boquilla del frasco para retirar los residuos y cerrarlo','66_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (489,1,67,1,'Sacar del frasco o envoltura el medicamento','67_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (490,1,67,2,'Vaciarlo directamente en el vaso graduado o el de papel','67_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (491,1,67,3,'Pedir a la persona que se lo coloque debajo de la lengua','67_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (492,1,67,4,'Cerciorarse que el medicamento permanezca en esa área','67_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (493,1,68,1,'Colocar al paciente acostado o sentado con la cabeza en ligera hiperextensión','68_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (494,1,68,2,'En caso de ser necesario, realizar limpieza ocular desde el ángulo externo del ojo con gasa y solución estéril','68_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (495,1,68,3,'Aplicar las gotas en el ángulo interno del ojo','68_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (496,1,68,4,'Limpiar el exceso de líquido que salga al exterior del ojo','68_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (497,1,68,5,'Si está indicado, cubrir el ojo con el apósito','68_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (498,1,69,1,'Descubrir el área','69_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (499,1,69,2,'Limpiar, en caso de ser necesario','69_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (500,1,69,3,'Aplicar el medicamento con el abatelenguas','69_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (501,1,70,1,'Colocar la cabeza de la persona en ligera hiperextensión.','70_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (502,1,70,2,'Realizar limpieza de fosas nasales, en caso de que se requiera con aplicadores.','70_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (503,1,70,3,'Aplicar las gotas en las fosas nasales.','70_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (504,1,70,4,'Indicar al paciente que mantenga esa posición por unos minutos.','70_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (505,1,71,1,'Colocar al paciente en posición de fowler o acostado, con el oído afectado hacia arriba','71_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (506,1,71,2,'En caso de ser necesario, limpiar el pabellón auricular','71_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (507,1,71,3,'Aplicar el medicamento','71_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (508,1,71,4,'Indicar a la persona que debe conservar esa posición por unos minutos','71_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (509,1,71,5,'Colocar un tapón con algodón para permitir que permanezca por más tiempo el medicamento, puede retirarse una hora después','71_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (510,1,71,6,'Colocar a la persona en una posición cómoda','71_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (511,1,72,1,'Proporcionar al paciente privacidad','72_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (512,1,72,2,'Colocarse los guantes','72_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (513,1,72,3,'Ofrecerle el cómodo o que asista al sanitario, para que evacué previo a la ministración del medicamento','72_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (514,1,72,4,'Pedir o colocar a la persona en posición de sims','72_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (515,1,72,5,'Lubricar el medicamento con vaselina en caso necesario e introducirlo con los dedos','72_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (516,1,72,6,'Realizar la limpieza necesaria en la zona','72_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (517,1,72,7,'Con una mano separar los glúteos y pedirle a la persona que puje un poco para facilitar la introducción del medicamento','72_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (518,1,72,8,'Colocar al paciente en una posición cómoda','72_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (519,1,72,9,'Pedirle a la persona que espere un tiempo mínimo de 20 minutos para ir al baño','72_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (520,1,73,1,'Proporcionar a la persona privacidad','73_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (521,1,73,2,'Colocarse los guantes','73_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (522,1,73,3,'Colocar a la persona en posición ginecológica','73_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (523,1,73,4,'Separar los labios y localizar el introito vaginal','73_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (524,1,73,5,'Introducir el medicamento con los dedos índice y medio','73_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (525,1,73,6,'Realizar la limpieza necesaria en la zona','73_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (526,1,73,7,'Colocarla en una posición cómoda','73_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (527,1,73,8,'Pedirle que no se levante por lo menos en 30 minutos','73_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (528,1,74,1,'Colocar al paciente en posición sentado con la cabeza inclinada hacia atrás','74_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (529,1,74,2,'El paciente debe espirar la mayor cantidad de aire posible','74_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (530,1,74,3,'Colocar el inhalador en la boca de la persona y pedirle que cierre la boca alrededor de éste, de forma que la dosis de medicamento que inhale sea la prescrita.','74_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (531,1,74,4,'Pedir al paciente que coordine la aplicación de la dosis con una inspiración profunda','74_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (532,1,75,1,'Informar al paciente y familiar, la técnica a realizar','75_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (533,1,75,2,'Proporcionar intimidad','75_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (534,1,75,3,'Lavarse las manos y ponerse los guantes desechables','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (535,1,75,4,'Seleccionar la zona para la administración de la medicación: - Región glútea en cuadrante superior externo.- Brazo en región deltoides.- Muslo en cara látero- anterior externa.','75_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (536,1,75,5,'Preparar el medicamento en la jeringa','75_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (537,1,75,6,'Desinfectar la zona con gasas / algodón impregnados de la solución antiséptica, evitando pasarlo dos veces por el mismo sitio, de arriba hacia abajo .','75_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (538,1,75,7,'Introducir la aguja profundamente, en un ángulo de noventa grados, con un movimiento rápido, directamente al músculo','75_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (539,1,75,8,'Aspirar para comprobar si se ha pinchado un vaso sanguíneo, si no aparece sangre, inyectar lentamente.','75_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (540,1,75,9,'Presionar la zona con gasas / algodón, retirando de una sola vez aguja y jeringa con un movimiento rápido y seguro','75_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (541,1,75,10,'Tirar el material utilizado al contenedor específico','75_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (542,1,75,11,'Seguir presionando un rato la zona inyectada, para evitar sangrado','75_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (543,1,75,12,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (544,1,75,13,'Registrar en hoja de comentarios la respuesta de la persona derivadas de la aplicación del medicamento, la hora y quién lo aplicó','75_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (545,1,76,1,'Informar al paciente y familia de la técnica a realizar','76_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (546,1,76,2,'Proporcionar intimidad','76_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (547,1,76,3,'Lavarse las manos y poner los guantes desechables','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (548,1,76,4,'Preparar el medicamento en la jeringa, previa indagación de respuestas secundarias al medicamento; en caso afirmativo, no se administra','76_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (549,1,76,5,'Colocar la ligadura 5 cm por arriba de la fosa cubital para visualizar la zona de punción','76_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (550,1,76,6,'Seleccionar la zona para la administración de la medicación:','76_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (551,1,76,7,'Desinfectar la zona con gasas / algodón impregnados de la solución antiséptica, evitando pasarlo dos veces por el mismo sitio, de arriba hacia abajo','76_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (552,1,76,8,'Introducir la aguja con el bisel hacia arriba, formando un ángulo de 30 grados, inmediatamente a un lado de la vena','76_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (553,1,76,9,'Bajar la aguja quedando casi paralela a la piel, puncionar la vena y seguir el trayecto venoso','76_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (554,1,76,10,'Aspirar para comprobar si está en la vena, en caso afirmativo, retirar la ligadura, si no aparece sangre, deberá buscar la vena','76_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (555,1,76,11,'Introducir lentamente el medicamento, estando atento de las reacciones de la persona, en caso de presentar alguna, suspender inmediatamente la administración','76_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (556,1,76,12,'Al terminar, presionar la zona con gasas / algodón, retirando de una sola vez aguja y jeringa con un movimiento rápido y seguro La presión con el algodón continuará por 1 minuto','76_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (557,1,76,13,'Tirar el material utilizado al contenedor específico, jamás reencapuchar la aguja','76_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (558,1,76,14,'Seguir presionando un rato la zona de aplicación, mínimo 1 minuto o hasta que deje de sangrar','76_14');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (559,1,76,15,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (560,1,76,16,'Registrar en hoja de comentarios la respuesta de la persona debido a la aplicación del medicamento, la hora y quien lo aplicó','76_16');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (561,1,77,1,'Informar al paciente y familia de la técnica a realizar','77_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (562,1,77,2,'Proporcionar intimidad','77_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (563,1,77,3,'Lavarse las manos','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (564,1,77,4,'Seleccionar la zona para la administración de la medicación','77_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (565,1,77,5,'Preparar el medicamento en la jeringa','77_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (566,1,77,6,'Desinfectar la zona con gasas / algodón impregnados de la solución antiséptica, evitando pasarlo dos veces por el mismo sitio, de arriba hacia abajo','77_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (567,1,77,7,'Con la mano no dominante, realizar un pliegue de 2 cm (pellizco)','77_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (568,1,77,8,'Introducir la aguja en un ángulo de 45º en relación con la base del pliegue, con el bisel de la guja hacia arriba','77_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (569,1,77,9,'Aspirar para comprobar que no se ha pinchado un vaso sanguíneo, si aparece sangre, retraer un poco la aguja y comprobar nuevamente Inyectar el medicamento','77_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (570,1,77,10,'Presionar la zona con gasas / algodón, retirando de una sola vez aguja y jeringa con un movimiento rápido y seguro','77_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (571,1,77,11,'Tirar el material utilizado al contenedor específico','77_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (572,1,77,12,'Lavarse las manos al término del procedimiento','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (573,1,77,13,'Registrar en hoja de comentarios la respuesta de la persona derivadas de la aplicación del medicamento, la hora y quien lo aplicó','77_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (574,1,78,1,'Crear un entorno relajado y cómodo.','78_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (575,1,78,2,'Evitar ruidos','78_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (576,1,78,3,'Administrar correctamente los medicamentos (si son hipnóticos se deben administrar a última hora de la noche)','78_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (577,1,78,4,'Evitar que la persona se sienta sola: hablarle, escucharle y transmitir confianza y cariño.','29_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (578,1,78,5,'Proporcionar alimentación adecuada, sin sobrecargas.','30_42');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (579,1,79,1,'Mantener o favorecer la ventilación de la habitación.','79_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (580,1,79,2,'Debe tener buena iluminación.','79_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (581,1,79,3,'La temperatura debe ser adecuada.','79_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (582,1,79,4,'Colocar una mesa a un lado de la cama, en donde pueda tener al alcance artículos personales.','79_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (583,1,79,5,'No dejar al alcance de la mano medicamentos.','65_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (584,1,79,6,'La habitación debe estar limpia.','79_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (585,1,79,7,'No debe haber ruido.','79_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (586,1,80,1,'Debe mantenerse informado sobre el padecimiento','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (587,1,80,2,'Dar tiempo y espacio para que pueda comunicarse en el momento adecuado.','80_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (588,1,80,3,'Saber respetar los silencios.','80_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (589,1,80,4,'Comprender el lenguaje no verbal como gestos o miradas.','29_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (590,1,80,5,'Realizar preguntas como:','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (591,1,80,6,'¿Qué es lo que más le molesta?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (592,1,80,7,'¿Qué es lo que más le preocupa?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (593,1,80,8,'¿Necesita más información?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (594,1,80,9,'¿Cuáles son sus deseos?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (595,1,81,1,'Tomar el sol.','81_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (596,1,81,2,'Promover la lectura que sea del interés de la persona.','81_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (597,1,81,3,'Pasear, si está en posibilidades de hacerlo.','81_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (598,1,81,4,'Escuchar música.','81_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (599,1,81,5,'Ver programas de televisión.','81_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (600,1,81,6,'Si es posible, incorporarle a las tareas del hogar como pelar verduras, picar fruta, etc.','81_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (601,1,82,1,'Proporcionar a la familia la mayor información posible sobre el padecimiento y estado del enfermo','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (602,1,82,2,'Explicar los beneficios que tiene la participación de la familia en los cuidados.','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (603,1,82,3,'Repartir el trabajo entre todos para evitar sobrecargas.','82_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (604,1,82,4,'La persona no debe escuchar comentarios negativos.','82_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (605,1,82,5,'Evitar que la persona se encuentre solo/a.','29_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (606,1,82,6,'Tratar de cumplir sus deseos en la medida de lo posible.','82_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (607,1,83,1,'Realizar preguntas como:','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (608,1,83,2,'¿Practica alguna religión?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (609,1,83,3,'¿Desea hablar con un representante de su religión?','80_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (610,1,84,1,'Respiración efectiva:','84_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (611,1,84,2,'Inspirar aire por la nariz o por la boca profundamente.','84_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (612,1,84,3,'Pedir que retenga el aire por algunos segundos.','84_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (613,1,84,4,'Expulsar lentamente con los labios estrechados, como si fuera a silbar.','84_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (614,1,84,5,'Respiración diafragmática:','84_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (615,1,84,6,'Se le pedirá a la persona que se siente, si es que puede.','84_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (616,1,84,7,'Debe colocar la mano sobre el abdomen.','84_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (617,1,84,8,'Observar cómo se eleva y contrae.','84_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (618,1,85,1,'Nunca obligarle a comer, se debe aconsejar: “lo que quiera, cuando quiera y cuanto quiera”.','85_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (619,1,85,2,'Realizar los cuidados oportunos en la boca (véase capítulo de higiene general, aseo bucal)','85_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (620,1,85,3,'Servir la comida en pequeños platos y varias veces al día.','85_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (621,1,85,4,'La comida debe adaptarse al gusto de la persona. Debe estar en una temperatura adecuada, poco condimentada y de fácil deglución.','85_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (622,1,85,5,'Favorecer que la persona coma en compañía de familiares o de otras personas.','85_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (623,1,85,6,'Ofrecer bebidas frecuentemente','85_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (624,1,85,7,'Evitar el uso de sondas en la medida de lo posible.','85_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (625,1,86,1,'Evitar piel seca:','86_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (626,1,86,2,'Utilizar jabón de pH neutro y secar con toallas suaves para el baño','86_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (627,1,86,3,'Aplicar compresas empapadas con agua (fría o caliente, dependiendo la situación) durante 20 minutos donde haya descamación.','86_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (628,1,86,4,'Aplicar crema después del baño, puede ser crema suave o crema emoliente.','86_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (629,1,86,5,'Evitar el exceso de humedad en la piel con:','86_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (630,1,86,6,'Baños de corta duración con jabón pH neutro y secar con toalla suave','86_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (631,1,86,7,'Limpiar la zona afectada con solución fisiológica y dejar actuar un par de minutos, secar con gasas o toallas suaves.','86_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (632,1,86,8,'Aplicar una ligera capa de talco.','86_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (633,1,86,9,'Asegurar que la zona de pliegues quede perfectamente seca.','86_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (634,1,87,1,'Dieta rica en fibras.','87_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (635,1,87,2,'Ingesta de líquidos frecuentes y abundantes.','87_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (636,1,87,3,'Control de la evacuación diaria.','87_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (637,1,87,4,'Movilización: si puede levantarse, hacer una pequeña caminata; si está postrado en cama, realizar cambios de postura y realizar masaje de marco cólico previa ingesta de la persona de agua tibia 20 minutos antes (Se inicia con movimientos circulares siguiendo el trayecto del intestino grueso con los dedos de la mano derecha, ejerciendo presión sin causar daño hasta terminar en la parte baja del abdomen; continuar realizando el masaje de acuerdo a las manecillas del reloj recorriéndose al centro del abdomen. Con los dedos de la mano izquierda efectuará únicamente presión siguiendo la mano derecha, para evitar que la materia fecal regrese).','87_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (638,1,87,5,'Hacer uso de laxantes únicamente bajo prescripción médica.','87_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (639,1,88,1,'No debe toser en decúbito supino. Elevar la cabecera, en semifowler o fowler.','88_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (640,1,88,2,'Evitar usar prendas compresivas.','88_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (641,1,88,3,'Colocarse los gogles y guantes (cuidador).','88_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (642,1,88,4,'Indicarle que respire tres veces de manera breve para seguir con una inspiración profunda por la nariz.','88_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (643,1,88,5,'Ofrecer papel higiénico para realizar la expectoración.','88_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (644,1,88,6,'Expulsar el aire de golpe contrayendo los músculos del abdomen y con la boca abierta sobre el papel.','88_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (645,1,88,7,'Practicar ejercicios de respiración. (Como en disnea).','88_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (646,1,88,8,'Mantener la calma para disminuir la ansiedad.','88_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (647,1,88,9,'Retirar el papel higiénico y desecharlo en la basura.','88_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (648,1,88,10,'En caso de presentar dificultad para expectorar, puede dar un vaso con agua tibia para facilitar la expulsión de las secreciones.','88_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (649,1,88,11,'Si no presenta disnea, se puede usar la técnica de palmo percusión para facilitar la expulsión, la cual consiste en realizar pequeñas vibraciones con la palma de la mano ahuecada (como palmadas) primero sobre el tórax después sobre la espalda, en dirección de abajo hacia arriba.','88_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (650,1,89,1,'Informar al paciente y familia de la técnica a realizar.','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (651,1,89,2,'Proporcionar intimidad.','72_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (652,1,89,3,'Lavarse las manos.','1_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (653,1,89,4,'Preparar el material y equipo, llevarlo al lugar donde se va a aspirar.','65_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (654,1,89,5,'Verificar la funcionalidad del aspirador.','89_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (655,1,89,6,'Verificar los signos vitales antes del procedimiento, F.C., F.R., temperatura y presión arterial.','19_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (656,1,89,7,'Colocarse las gafas y cubrebocas','89_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (657,1,89,8,'Colocar a la persona en semifowler con la cabeza girada hacia un lado para aspiración orotraqueal. En caso de aspirar vía nasotraqueal, cambia la posición de la cabeza con el cuello en hiperextensión a 70°.','89_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (658,1,89,9,'La presión de aspiración deberá estar de 100 a 150 mmHg.','89_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (659,1,89,10,'Abrir la envoltura de la sonda de succión solo por el área donde será unido y conectarlo al tubo de conexión, asegurar que la sonda en su extremo distal no toque nada, ni salga completamente de la envoltura.','89_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (660,1,89,11,' Verter solución fisiológica o agua estéril en un frasco, aproximadamente 50 ml.','89_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (661,1,89,12,'Colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (662,1,89,13,'Realizar primero la aspiración por nasofaringe y después por orofaringe.','89_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (663,1,89,14,'Previo a la aspiración, primero deberá lubricar la punta de la sonda en la solución y aspirar para ver su permeabilidad. ','89_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (664,1,89,15,'Sacar la sonda y medir la distancia entre el lóbulo de la oreja a la punta de la nariz de la persona sin tocarlos.','89_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (665,1,89,16,' Verificar las fosas nasales y utilizar la más permeable para realizar la aspiración.','89_16');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (666,1,89,17,'Indicar a la persona que inspire mientras se introduce la sonda suavemente por la fosa nasal, deslizándola por el suelo de la cavidad, hasta la medida indicada. Al introducir la sonda, cerciorarse que la sonda no aspire, hágalo de forma rápida y gentil, posteriormente, destape el orificio para permitir que succione. El tiempo de aspiración no debe sobrepasar los 10 a 15 segundos entre cada introducción (desde la introducción hasta la salida de la sonda).','89_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (667,1,89,18,'Tomar una gasa con la mano no dominante y colocarla alrededor de la sonda, sostenerla ahí hasta sacar completamente la sonda.','89_18');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (668,1,89,19,'Para retirar la sonda, realizar pequeños movimientos rotatorios al mismo tiempo que se extrae.','89_18');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (669,1,89,20,' Insertar la sonda en el frasco con solución aspirando, para enjuagar las secreciones que pudiesen quedar adheridas al tubo.','89_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (670,1,89,21,'Repetir el procedimiento de aspiración, no más de 3 ocasiones continuas.','89_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (671,1,89,22,' Al terminar, desechar la sonda, la gasa y la solución que resta en el frasco.','1_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (673,1,227,15,'Pedir a la persona que abra la boca.','227_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (674,1,227,16,'Introducir la sonda deslizándola por los carrillos (laterales de la boca) hasta llegar a la orofaringe.','227_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (675,1,227,17,'Aspirar por las mejillas, debajo de la lengua y la parte posterior de la nasofaringe, según sea necesario.','227_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (676,1,227,18,'La aspiración se realiza de la parte posterior hacia afuera.','227_15');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (677,1,227,19,'Al introducir la sonda, cerciorarse que la sonda no aspira, hacerlo de forma gentil, posteriormente, destapar el orificio para permitir que succione.','227_16');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (678,1,227,20,'Tomar una gasa con la mano no dominante y colocarla alrededor de la sonda, sostenerla ahí hasta sacar completamente la sonda.','89_25');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (679,1,227,21,'Para retirar la sonda, realizar pequeños movimientos rotatorios, al mismo tiempo que la extrae.','89_18');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (680,1,227,22,'Inserte la sonda en el frasco con solución aspirando, para enjuagar las secreciones que pudiesen quedar adheridas al tubo.','89_20');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (681,1,227,23,'Repita el procedimiento hasta verificar que la cavidad se encuentra libre de secreciones.','89_25');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (682,1,90,1,'Evaluación del estado de consciencia (orientación, respuesta verbal y motora)','90_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (683,1,90,2,'Informar al familiar la situación por la que está pasando la persona.','90_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (684,1,90,3,'Monitorear continuamente los signos vitales, cada 15 minutos.','90_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (685,1,90,4,'Evitar traer objetos colgados al cuello.','90_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (686,1,90,5,'En caso de que la persona tenga una enfermedad preexistente, verificar que no esté descompensada (diabetes mellitus: medir la glucosa; hipertensión arterial: medir presión arterial).','90_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (687,1,90,6,'Control de la temperatura en caso de presentar hipertermia o hipotermia.','90_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (688,1,90,7,'Anotar en la bitácora el incidente.','90_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (689,1,90,8,'En caso de presentar ilusiones o alucinaciones, evitar que se lesione la persona a sí misma o a otros; se debe retirar cualquier objeto que pueda causar o causarse daño.','90_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (690,1,90,9,'Solicitar al familiar más cercano, que acompañe a la persona y al cuidador, evitando quedarse una sola persona.','90_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (691,1,90,10,'En caso de que la persona se comporte agresiva, es preferible una sujeción gentil, solicitando el apoyo de un familiar.','90_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (692,1,90,11,'Informar lo más pronto posible al médico tratante del episodio, realizando una descripción del evento.','90_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (693,1,90,12,'Esperar hasta que personal capacitado evalúe y brinde tratamiento adecuado (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias, ya que es necesario que sea revisado por un médico).','90_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (694,1,91,1,'Evaluación del estado de consciencia (orientación, respuesta verbal y motora), utilizar escala de Glasgow','91_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (695,1,91,2,'Descartar la presencia de lesiones (herida, fractura o hemorragia, en cuyo caso, aplique las medidas mencionadas en el apartado correspondiente)','91_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (696,1,91,3,'Tomar signos vitales','91_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (697,1,91,4,'Indagar la existencia de: cefalea, mareo, referencia de vista nublada (fosfenos) o con presencia de lucecitas, referencia de zumbido de oídos (acufenos), confusión, dificultad para la marcha, vómito y diarrea.','91_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (698,1,91,5,'Monitorear continuamente los signos vitales.','91_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (699,1,91,6,'En caso de que la persona tenga una enfermedad preexistente, verificar que no esté descompensada (diabetes mellitus: medir la glucosa; hipertensión arterial: medir presión arterial).','91_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (700,1,91,7,'Si las condiciones de la persona lo permiten, ayudarlo a levantase, en caso contrario, cubrirlo con una manta para evitar que pierda temperatura y acompañarlo en todo momento.','91_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (701,1,91,8,'Anotar en la bitácora el incidente.','91_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (702,1,91,9,'Esperar hasta que personal capacitado evalúe y brinde tratamiento adecuado (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias).','91_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (703,1,91,10,'En caso de que la persona se comporte agresiva, es preferible una sujeción gentil, solicitando el apoyo de un familiar.','91_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (704,1,91,11,'Informar lo más pronto posible al médico tratante del episodio, realizando una descripción del evento.','91_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (705,1,91,12,'Esperar hasta que personal capacitado evalúe y brinde tratamiento adecuado (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias, ya que es necesario que sea revisado por un médico).','91_12');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (706,1,92,1,'Tomar signos vitales','92_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (707,1,92,2,'Si la persona tiene medicamento prescrito por el médico por problemas de hipertensión, ayudar a su ministración.','92_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (708,1,92,3,'Monitorear continuamente los signos vitales.','92_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (709,1,92,4,'Esperar hasta que personal capacitado evalúe y brinde tratamiento adecuado (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias).','92_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (710,1,93,1,'Mantenga cómoda a la persona.','93_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (711,1,93,2,'No darle nada de beber ocomer','93_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (712,1,93,3,'Aflojar la ropa ajustada.','93_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (713,1,93,4,'Tomar signos vitales','93_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (714,1,93,5,'Si no hay pulso y la persona deja de respirar, comenzar a realizar RCP, solo si está capacitado para hacerlo.','93_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (715,1,93,6,'Esperar hasta que personal capacitado evalúe y brinde tratamiento adecuado (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias).','93_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (722,1,94,1,'Tranquilizar a la persona.','94_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (723,1,94,2,'Sentarlo o colocarlo en posición semifowler.','94_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (724,1,94,3,'Aflojar la ropa u objetos ajustados.','94_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (725,1,94,4,'Si la persona tiene medicamento prescrito, ayudarlo con la ministración.','94_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (726,1,94,5,'Si el dolor continúa por más de 10 minutos, sospeche de un infarto cardiaco.','94_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (727,1,94,6,'Esperar hasta que personal capacitado evalúe y brinde el tratamiento adecuado. (Si el personal tarda más de 30 minutos, trasladarlo a un servicio de emergencias).','94_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (728,1,95,1,'Si está capacitado, verificar el nivel de glucosa de la persona.','95_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (729,1,95,2,'Administrar una cucharada de azúcar, media lata de refrescoo jugo.','95_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (730,1,95,3,'Esperar hasta que personal capacitado evalúe y brinde el tratamiento adecuado.','95_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (739,1,97,1,'Tranquilizar a la persona y pedirle que realice respiraciones profundas y pausadas','97_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (740,1,97,2,'Si la persona cuenta con inhalador para asma bajo prescripción, ayúdele a usarlo.','97_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (741,1,97,3,'Si la persona queda inconsciente, realizar ventilación mecánica.','97_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (742,1,97,4,'Esperar hasta que personal capacitado evalúe y brinde el tratamiento adecuado.','97_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (743,1,96,1,'Evitar las lesiones alejando cualquier objeto peligroso, sobre todo evitar que golpee su cabeza contra el suelo, colocando algo suave','96_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (744,1,96,2,'Aflojar cualquier prenda de ropa ajustada.','96_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (745,1,96,3,'Girar la cabeza para ayudar a mantener las vías aéreas libres y, evitar que se pueda morder la lengua.','96_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (746,1,96,4,'Medir el tiempo del episodio, así como la frecuencia.','96_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (747,1,96,5,'Ayudarle a incorporarse, posterior al episodio.','96_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (748,1,96,6,'En caso de relajación de esfínteres, ayudarle a cambiarse de ropa','96_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (749,1,96,7,'Terminado el episodio, evaluar la presencia de heridas y atiéndalas en la medida de lo posible.','96_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (750,1,96,8,'Esperar hasta que personal capacitado, evalúe ybrinde el tratamiento adecuado.','96_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (751,1,98,1,'Evitar las lesiones alejando cualquier objeto peligroso, sobre todo evitar que golpee su cabeza contra el suelo, colocando algo suave','98_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (752,1,98,2,'Aflojar cualquier prenda de ropa ajustada.','98_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (753,1,98,3,'Girar la cabeza para ayudar a mantener las vías aéreas libres y, evitar que se pueda morder la lengua.','98_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (754,1,98,4,'Medir el tiempo del episodio, así como la frecuencia.','98_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (755,1,98,5,'Ayudarle a incorporarse, posterior al episodio.','98_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (756,1,98,6,'En caso de relajación de esfínteres, ayudarle a cambiarse de ropa','98_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (757,1,99,1,'Cuando las quemaduras son leves, incluidas las de segundo grado, limitadas a un área no mayor a 5 o 7 cms de diámetro, se puede realizar lo siguiente:','99_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (758,1,99,2,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en el área afectada.','99_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (759,1,99,3,'Colocar la región lesionada al chorro de agua o sumergirla en agua, no debe de ser fría, debe de estar a temperatura ambiente durante 15 minutos.','99_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (760,1,99,4,'Inmovilizar el área lesionada.','99_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (761,1,99,5,'Colocar una venda estéril y húmeda.','99_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (762,1,99,6,'Esperar hasta que personal capacitado evalúe la quemadura y brinde tratamiento adecuado.','99_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (763,1,100,1,'Cuando las quemaduras son severas, incluidas las de segundo y tercer grado, se puede realizar lo siguiente:','100_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (764,1,100,2,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en el área afectada.','100_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (765,1,100,3,'Inmovilice el área lesionada.','100_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (766,1,100,4,'Si ya están adheridos a la piel ropa u objetos, no retirarlos.','100_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (767,1,100,5,'Colocar una venda o apósito estéril, ligeramente húmedo','100_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (768,1,100,6,'No reventar ámpulas ni colocar ungüentos ni pomadas.','100_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (769,1,100,7,'Monitorear y mantener los signos vitales.','100_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (770,1,100,8,'No tocar el área afectada.','100_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (771,1,100,9,'Evitar que la piel quemada toque piel con piel.','100_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (772,1,100,10,'No quitar los pedazos de piel.','100_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (773,1,100,11,'Esperar hasta que personal capacitado, evalúe la quemadura y brinde el tratamiento adecuado.','100_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (774,1,101,1,'Cuando las quemaduras son severas, incluidas las de segundo y tercer grado, se puede realizar lo siguiente:','101_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (775,1,101,2,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en el área afectada.','101_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (776,1,101,3,'Inmovilice el área lesionada.','101_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (777,1,101,4,'Si ya están adheridos a la piel ropa u objetos, no retirarlos.','101_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (778,1,101,5,'Colocar una venda o apósito estéril, ligeramente húmedo','101_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (779,1,101,6,'No reventar ámpulas ni colocar ungüentos ni pomadas.','101_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (780,1,101,7,'Monitorear y mantener los signos vitales.','101_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (781,1,101,8,'No tocar el área afectada.','101_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (782,1,101,9,'Evitar que la piel quemada toque piel con piel.','101_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (783,1,101,10,'No quitar los pedazos de piel.','101_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (784,1,101,11,'Esperar hasta que personal capacitado, evalúe la quemadura y brinde el tratamiento adecuado.','101_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (785,1,102,1,'Presión directa: Se realiza con un apósito o gasa limpia colocándolo sobre la herida y presionando firmemente con la palma de la mano, ésta compresión directa puede ser sustituida con un vendaje de presión cuando las heridas son demasiado grandes.','102_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (786,1,102,2,'Presión Indirecta: Se realiza comprimiendo los puntos arteriales más cercanos, entre la herida y el corazón. Esta técnica, reduce la irrigación de toda la zona afectada y, no solo de la herida como sucede con la presión directa.','102_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (787,1,102,3,'Elevación de la extremidad: Deberá utilizarse cuando no se contenga la hemorragia, elevando la extremidad por encima del nivel del corazón, ya que esto disminuye la presión de la sangre en el lugar de la herida, siempre y cuando no tenga alguna fractura.','102_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (788,1,102,4,'Crioterapia: Consiste en la aplicación de frío en una zona cerca de la herida para lograr vasoconstricción y por lo tanto, la disminución del flujo sanguíneo; no se debe colocar el hielo directamente sobre la herida, para evitar que se dañen los tejidos.','102_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (789,1,102,5,'Esperar hasta que personal capacitado evalúe la herida y, brinde el tratamiento adecuado.','102_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (790,1,103,1,'Presión directa: Se realiza con un apósito o gasa limpia colocándolo sobre la herida y presionando firmemente con la palma de la mano, ésta compresión directa puede ser sustituida con un vendaje de presión cuando las heridas son demasiado grandes.','103_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (791,1,103,2,'Presión Indirecta: Se realiza comprimiendo los puntos arteriales más cercanos, entre la herida y el corazón. Esta técnica, reduce la irrigación de toda la zona afectada y, no solo de la herida como sucede con la presión directa.','103_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (792,1,103,3,'Elevación de la extremidad: Deberá utilizarse cuando no se contenga la hemorragia, elevando la extremidad por encima del nivel del corazón, ya que esto disminuye la presión de la sangre en el lugar de la herida, siempre y cuando no tenga alguna fractura.','103_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (793,1,103,4,'Crioterapia: Consiste en la aplicación de frío en una zona cerca de la herida para lograr vasoconstricción y por lo tanto, la disminución del flujo sanguíneo; no se debe colocar el hielo directamente sobre la herida, para evitar que se dañen los tejidos.','103_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (794,1,103,5,'Esperar hasta que personal capacitado evalúe la herida y, brinde el tratamiento adecuado.','103_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (795,1,104,1,'Presión directa: Se realiza con un apósito o gasa limpia colocándolo sobre la herida y presionando firmemente con la palma de la mano, ésta compresión directa puede ser sustituida con un vendaje de presión cuando las heridas son demasiado grandes.','104_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (796,1,104,2,'Presión Indirecta: Se realiza comprimiendo los puntos arteriales más cercanos, entre la herida y el corazón. Esta técnica, reduce la irrigación de toda la zona afectada y, no solo de la herida como sucede con la presión directa.','104_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (797,1,104,3,'Elevación de la extremidad: Deberá utilizarse cuando no se contenga la hemorragia, elevando la extremidad por encima del nivel del corazón, ya que esto disminuye la presión de la sangre en el lugar de la herida, siempre y cuando no tenga alguna fractura.','104_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (798,1,104,4,'Crioterapia: Consiste en la aplicación de frío en una zona cerca de la herida para lograr vasoconstricción y por lo tanto, la disminución del flujo sanguíneo; no se debe colocar el hielo directamente sobre la herida, para evitar que se dañen los tejidos.','104_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (799,1,104,5,'Esperar hasta que personal capacitado evalúe la herida y, brinde el tratamiento adecuado.','104_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (800,1,105,1,'Controlar la hemorragia si se encuentra presente.','105_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (801,1,105,2,'Retirar la ropa que cubre la herida.','105_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (802,1,105,3,'Asepsia: Lavar con agua y con jabón quirúrgico sin que éste toque dos veces la lesión, durante 5 minutos. No realizar en caso de objeto incrustado.','105_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (803,1,105,4,'Enjuagar la herida con agua corriente bajo presión. No realizar en caso de objeto incrustado.','105_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (804,1,105,5,'No aplicar ningún tipo de remedio casero, medicamentos o antibióticos, debido a que se puede causar una reacción alérgica.','105_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (805,1,105,6,'Cubrir la herida con material estéril y, si es posible, no adherente; colocar un vendaje solo para detener el apósito.','105_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (806,1,105,7,'Cambiar el vendaje o apósito de forma periódica si se requiere.','105_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (807,1,105,8,'Si tiene un objeto incrustado no lo retire, estabilizarlo con vendas.','105_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (808,1,105,9,'Esperar hasta que personal especializado evalúe la herida y brinde tratamiento adecuado.','105_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (809,1,106,1,'Controlar la hemorragia si se encuentra presente.','106_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (810,1,106,2,'Retirar la ropa que cubre la herida.','106_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (811,1,106,3,'Asepsia: Lavar con agua y con jabón quirúrgico sin que éste toque dos veces la lesión, durante 5 minutos. No realizar en caso de objeto incrustado.','106_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (812,1,106,4,'Enjuagar la herida con agua corriente bajo presión. No realizar en caso de objeto incrustado.','106_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (813,1,106,5,'No aplicar ningún tipo de remedio casero, medicamentos o antibióticos, debido a que se puede causar una reacción alérgica.','106_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (814,1,106,6,'Cubrir la herida con material estéril y, si es posible, no adherente; colocar un vendaje solo para detener el apósito.','106_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (815,1,106,7,'Cambiar el vendaje o apósito de forma periódica si se requiere.','106_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (816,1,106,8,'Si tiene un objeto incrustado no lo retire, estabilizarlo con vendas.','106_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (817,1,106,9,'Esperar hasta que personal especializado evalúe la herida y brinde tratamiento adecuado.','106_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (818,1,107,1,'Controlar la hemorragia si se encuentra presente.','107_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (819,1,107,2,'Retirar la ropa que cubre la herida.','107_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (820,1,107,3,'Asepsia: Lavar con agua y con jabón quirúrgico sin que éste toque dos veces la lesión, durante 5 minutos. No realizar en caso de objeto incrustado.','107_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (821,1,107,4,'Enjuagar la herida con agua corriente bajo presión. No realizar en caso de objeto incrustado.','107_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (822,1,107,5,'No aplicar ningún tipo de remedio casero, medicamentos o antibióticos, debido a que se puede causar una reacción alérgica.','107_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (823,1,107,6,'Cubrir la herida con material estéril y, si es posible, no adherente; colocar un vendaje solo para detener el apósito.','107_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (824,1,107,7,'Cambiar el vendaje o apósito de forma periódica si se requiere.','107_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (825,1,107,8,'Si tiene un objeto incrustado no lo retire, estabilizarlo con vendas.','107_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (826,1,107,9,'Esperar hasta que personal especializado evalúe la herida y brinde tratamiento adecuado.','107_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (827,1,108,1,'Controlar la hemorragia si se encuentra presente.','108_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (828,1,108,2,'Retirar la ropa que cubre la herida.','108_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (829,1,108,3,'Asepsia: Lavar con agua y con jabón quirúrgico sin que éste toque dos veces la lesión, durante 5 minutos. No realizar en caso de objeto incrustado.','108_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (830,1,108,4,'Enjuagar la herida con agua corriente bajo presión. No realizar en caso de objeto incrustado.','108_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (831,1,108,5,'No aplicar ningún tipo de remedio casero, medicamentos o antibióticos, debido a que se puede causar una reacción alérgica.','108_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (832,1,108,6,'Cubrir la herida con material estéril y, si es posible, no adherente; colocar un vendaje solo para detener el apósito.','108_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (833,1,108,7,'Cambiar el vendaje o apósito de forma periódica si se requiere.','108_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (834,1,108,8,'Si tiene un objeto incrustado no lo retire, estabilizarlo con vendas.','108_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (835,1,108,9,'Esperar hasta que personal especializado evalúe la herida y brinde tratamiento adecuado.','108_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (836,1,109,1,'Controlar hemorragia si se encuentra presente','109_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (837,1,109,2,'Retirar la ropa que cubre la herida.','109_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (838,1,109,3,'Cubrir la herida con apósitos estériles y húmedos, ya sea con solución estéril o fisiológica al 0.9%, hacer un vendaje de sostén con una venda estéril y húmeda.','109_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (839,1,109,4,'Prevenir el estado de shock.','109_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (840,1,109,5,'Nunca intentar regresar las vísceras a su lugar','109_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (841,1,110,1,'Controlar hemorragia, si se presenta','110_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (842,1,110,2,'Retirar la ropa que cubre la herida.','110_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (843,1,110,3,'Inmovilizar la zona afectada.','110_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (844,1,110,4,'Colocar un vendaje de muñón en la zona afectada.','110_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (845,1,110,5,'La estructura amputada deberá cubrirse con apósitos húmedos, posteriormente, se debe colocar en una bolsa de plástico y, finalmente, en una bolsa o recipiente con hielos. Ésta nunca debe colocarse directamente sobre hielo.','110_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (846,1,110,6,'Esperar hasta que personal capacitado evalúe la herida y brinde tratamiento adecuado. 1,5','110_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (847,1,111,1,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en la zona afectada, y que al inflamarse pueda causar más dolor, dificulte la inmovilización o la circulación.','111_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (848,1,111,2,'Inmovilizar la zonaafectada.','111_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (849,1,111,3,'Asistir con material limpio, no pesado y adecuado, para inmovilizar la lesión.','111_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (850,1,111,4,'Se pueden improvisar férulas para inmovilizar firmemente, siempre y cuando cubran desde una articulación arriba, hasta una debajo del sitio de la lesión.','111_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (851,1,111,5,'Colocar un vendaje de forma compresiva con la intención de disminuir hemorragias internas. (Evitar torniquetes)','111_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (852,1,111,6,'Esperar hasta que personal capacitado evalúe la lesión y brinde tratamiento adecuado.','111_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (853,1,112,1,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en la zona afectada, y que al inflamarse pueda causar más dolor, dificulte la inmovilización o la circulación.','112_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (854,1,112,2,'Inmovilizar la zonaafectada.','112_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (855,1,112,3,'Asistir con material limpio, no pesado y adecuado, para inmovilizar la lesión.','112_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (856,1,112,4,'Se pueden improvisar férulas para inmovilizar firmemente, siempre y cuando cubran desde una articulación arriba, hasta una debajo del sitio de la lesión.','112_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (857,1,112,5,'Colocar un vendaje de forma compresiva con la intención de disminuir hemorragias internas. (Evitar torniquetes)','112_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (858,1,112,6,'Esperar hasta que personal capacitado evalúe la lesión y brinde tratamiento adecuado.','112_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (859,1,113,1,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en la zona afectada, y que al inflamarse pueda causar más dolor, dificulte la inmovilización o la circulación.','113_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (860,1,113,2,'Inmovilizar la zonaafectada.','113_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (861,1,113,3,'Asistir con material limpio, no pesado y adecuado, para inmovilizar la lesión.','113_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (862,1,113,4,'Se pueden improvisar férulas para inmovilizar firmemente, siempre y cuando cubran desde una articulación arriba, hasta una debajo del sitio de la lesión.','113_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (863,1,113,5,'Colocar un vendaje de forma compresiva con la intención de disminuir hemorragias internas. (Evitar torniquetes)','113_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (864,1,113,6,'Esperar hasta que personal capacitado evalúe la lesión y brinde tratamiento adecuado.','113_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (865,1,114,1,'Retirar los anillos, pulseras, reloj o todo aquello que se encuentre en la zona afectada, y que al inflamarse pueda causar más dolor, dificulte la inmovilización o la circulación.','114_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (866,1,114,2,'Inmovilizar la zonaafectada.','114_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (867,1,114,3,'Asistir con material limpio, no pesado y adecuado, para inmovilizar la lesión.','114_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (868,1,114,4,'Se pueden improvisar férulas para inmovilizar firmemente, siempre y cuando cubran desde una articulación arriba, hasta una debajo del sitio de la lesión.','114_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (869,1,114,5,'Colocar un vendaje de forma compresiva con la intención de disminuir hemorragias internas. (Evitar torniquetes)','114_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (870,1,114,6,'Esperar hasta que personal capacitado evalúe la lesión y brinde tratamiento adecuado.','114_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (871,1,115,1,'Arrodillarse a un costado de la persona.','115_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (872,1,115,2,'Determine si hay respuesta dando golpecitos suaves en el hombro y preguntes si se encuentra bien, al mismo tiempo, observe si respira y revise la presencia de pulso.','115_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (873,1,115,3,'Si la víctima no responde y no presenta pulso, active el SMU e inicie las maniobras de RCP.','115_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (874,1,115,4,'Maniobra de RCP.','115_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (875,1,115,5,'Entrelazar los dedos de una mano con los de la otra, por encima del dorso de la primera.','115_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (876,1,115,6,'Talón de la mano sobre el esternón, de 3 a 5 centímetros por arriba del apéndice xifoides.','115_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (877,1,115,7,'Brazos y espalda rectos. Utilizar el peso de su cuerpo para hacer la compresión.','115_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (878,1,115,8,'Para realizar las respiraciones de rescate, abrir la vía aérea haciendo una hiperextensión de la cabeza y levantando el mentón, tape la nariz de la víctima pinzando con los dedos pulgar e índice y, forme un sello hermético sobre la boca con su propia boca e insufle','115_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (879,1,115,9,'Aplique 30 compresiones y dos respiraciones de rescate abriendo la vía aérea. La velocidad de las compresiones será de por lo menos 100 por minuto. Administre una respiración que dure un segundo, tome una respiración para sí mismo y luego dé a la persona otra respiración de un segundo. Cada respiración de rescate, debe hacer que el tórax de la víctima se eleve. Continuar con RCP hasta que llegue el personal del SMU, la persona reaccione, esté listo un DAE o hasta que se canse.','115_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (880,1,116,1,'Colocarse a un costado de la víctima para aplicarle los electrodos.','116_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (881,1,116,2,'Exponga el tórax de la víctima. La piel debe estar bastante seca, de manera que los cojinetes se adhieran y conduzcan la electricidad de forma adecuada.','116_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (882,1,116,3,'Si es necesario, rasure el área y seque la piel con una toalla.','116_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (883,1,116,4,'Retire la cubierta posterior de los cojinetes y aplíquelos con firmeza sobre el tórax desnudo de la persona, de acuerdo con el diagrama que éstos incluyen.','116_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (884,1,116,5,'Encienda el DAE.','116_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (885,1,116,6,'Apártese de la persona y no tocarla hasta recibir indicaciones del DAE.','116_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (886,1,116,7,'Asegúrese de que el cable esté conectado al DAE y hágase a un lado para analizar la actividad eléctrica del corazón, nadie puede estar en contacto con la persona en este momento, ni después, si está indicada una descarga.','116_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (887,1,116,8,'El DAE indicará la necesidad de aplicar una descarga y, de acuerdo con el dispositivo, le indicará al rescatista, que oprima un botón para administrar la descarga o la proporcionará de manera automática.','116_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (888,1,116,9,'Inicie RCP de forma inmediata después de la descarga y, siga las indicaciones que incluyen un nuevo análisis de la frecuencia. Si la descarga funcionó, la persona comenzará a recuperar signos de vida. Continúe con la atención hasta que llegue el personal del SMU, la persona reaccione o hasta que usted se canse.','116_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (889,1,4,1,'Colocar a la persona en decúbito supino y pida que mantenga los ojos cerrados','4_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (890,1,4,2,'Observar zonas enrojecidas y/o secreciones anormales','4_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (891,1,4,3,'Realizar el aseo iniciando por el canto interno, hasta el externo con una gasa humedecida con jabón','4_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (892,1,4,4,'Ayudarse de una gasa si es necesario, para eliminar secreciones y suciedad de los bordes de los ojos','4_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (893,1,4,5,'Retirar el jabón con una gasa humedecida en agua y secar con una gasa (una gasa para cada ojo)','4_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (894,1,4,6,'Se aplicará alguna pomada para humectar el interior de los ojos, siempre y cuando esté prescrito por el médico, siempre al final del baño','4_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (895,1,17,1,'Lavarse las manos y colocarse los guantes	','2_4');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (896,1,17,2,'Colocar el material a utilizar en una silla o en los pies de la cama,en el orden en que se irá ocupando	','16_2');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (897,1,17,3,'Colocar la cama en posición horizontal	','16_3');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (898,1,17,4,'Quitar la colcha, cobija y sábanas, colocarlas en la bolsa	','16_4');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (899,1,17,5,'Quitar la almohada	','16_5');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (900,1,17,6,'Extienda la sábana de cajón sobre la cama, (deberá de quedar bien extendida para evitar pliegues)	','16_6');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (901,1,17,7,'Colocar la sábana plana desdoblándola desde la cabecera hasta los pies El dobladillo ancho, se pondrá al nivel del borde superior del colchón	','16_7');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (902,1,17,8,'Extender la cobija sobre la cama. Remeter los pies y hacer las esquinas	','16_8');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (903,1,17,9,'Extender la colcha	','16_9');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (904,1,17,10,'Realizar el dobladillo de la sábana plana sobre la cobija y la colcha, procurando que quede a una altura suficiente para cubrir los hombros de la persona	','16_10');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (905,1,17,11,'Cambiar la funda de la almohada y colocarla en la cabecera de la cama	','16_11');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (906,1,17,12,'Recoger todo el colocarlo en su lugar	','16_12');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (907,1,17,13,'Quitarse los guantes y lavarse las manos	','7_6');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (908,1,17,14,'Hacer dobles de cortesía	','16_14');										";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (909,1,18,3,'Colocar el material a utilizar en una silla o en los pies de la cama,en el orden en que se irá ocupando','16_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (910,1,21,8,'Colocar el estetoscopio en posición de uso, en los oídos con las olivas hacia delante.','21_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (911,1,24,9,'Ayudarle a levantarse y acomodar su ropa','24_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (912,1,24,10,'El cuidador le apoyará, para que se siente en la silla de ruedas','24_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (913,1,24,11,'Ayudarle con el lavado de manos','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (914,1,24,12,'El cuidador deberá lavar sus manos','29_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (915,1,21,8,'Colocar el estetoscopio en posición de uso, en los oídos con las olivas hacia delante.','21_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (916,1,21,8,'Colocar el estetoscopio en posición de uso, en los oídos con las olivas hacia delante.','21_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (917,1,57,3,'La persona que recibe asistencia, se puede colocar en posición sedente, en bipedestación erecta o decúbito dorsal.','57_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (918,1,220,1,'La persona que recibe asistencia, se puede colocar en posición sedente, en bipedestación erecta o decúbito dorsal.','220_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (919,1,220,2,'Para la posición de bipedestación erecta, deberá apoyarse de unas barras paralelas, asimismo, deberá realizar semiflexión de cadera y rodilla, una pierna a la vez.','220_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (920,1,220,3,'Realizará un movimiento de flexión plantar, de forma lenta.','220_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (921,1,220,4,'Regresará a la posición inicial del tobillo.','220_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (922,1,220,5,'A continuación, llevará a cabo un movimiento de flexión dorsal.','220_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (923,1,220,6,'Ahora, con ambos, hará movimientos de','220_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (925,1,221,1,'La persona que recibe asistencia, se puede colocar en posición sedente, en bipedestación erecta o decúbito dorsal.','221_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (926,1,221,2,'Para la posición de bipedestación erecta, deberá apoyarse de unas barras paralelas, asimismo, deberá realizar semiflexión de cadera y rodilla, una pierna a la vez.','221_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (927,1,221,3,'Comenzará con un movimiento de pronación.','221_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (928,1,221,4,'Regresará a su posición inicial.','221_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (929,1,221,5,'Continuará con un movimiento de supinación.','221_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (930,1,222,1,'Factor Intrínsecos:Directamente relacionados con el estado de la persona.Determinantes.Disminución de la sensibilidad al dolor y presión. Adelgazamiento de la grasa y los músculos entre las prominencias óseas que soportan el peso y la piel.Contribuyentes.Atrofia por desuso.Malnutrición.Anemia.Edad avanzada.Parálisis.Inmovilidad.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (931,1,222,2,'Factores Extrínsecos:Son situaciones del entorno sobre los que más se puede actuar para evitarlos.Determinantes.La presión es la más importante, su fuerza y duración determinan directamente la extensión de la úlcera.Contribuyentes.Soporte mal ajustado.Arrugas en la ropa.Humedad.','222_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (932,1,222,3,'Eritema cutáneo que no palidece en piel intacta. En personas de piel oscura, se observa edema, induración, decoloración y calor local.','222_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (933,1,222,4,'Pérdida parcial del grosor de la piel que afecta la epidermis, dermis o ambas. Úlcera superficial que tiene aspecto de abrasión, ampolla o cráter superficial.','222_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (934,1,222,5,'Pérdida total del grosor de la piel que implica lesión o necrosis del tejido subcutáneo, que puede extenderse hacia abajo pero no por la fascia subyacente.','222_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (935,1,222,6,'Pérdida total del grosor de la piel con destrucción extensa, necrosis del tejido o lesión en músculo, hueso o estructuras de sostén (tendón, cápsula articular, entre otros).Tanto en el estadio IV como en el estadio III, pueden presentarse lesiones con cavernas, tunelizaciones o trayectos sinuosos (difíciles).','222_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (936,1,223,1,'Permanecer postrado en cama el menor tiempo posible.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (937,1,223,2,'Realizar cambios posturales cada 2 horas y cada hora en personas con silla de ruedas.','222_2');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (938,1,223,3,'Evitar el roce de prominencias entre sí.','222_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (939,1,223,4,'Aliviar la presión con: almohadas, colchón neumático (agua o aire), cojines de gel de frotación, protector de talones y codos, piel de borrego y felpa.','222_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (940,1,223,5,'Evitar levantar la cabeza de la cama más de 30°.','222_5');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (941,1,223,6,'Intentar mantener en todo momento la alineación corporal.','222_6');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (942,1,224,1,'Permanecer postrado en cama el menor tiempo posible.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (943,1,224,2,'Realizar cambios posturales cada 2 horas y cada hora en personas con silla de ruedas.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (944,1,224,3,'Evitar el roce de prominencias entre sí.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (945,1,224,4,'Aliviar la presión con: almohadas, colchón neumático (agua o aire), cojines de gel de frotación, protector de talones y codos, piel de borrego y felpa.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (946,1,224,5,'Evitar levantar la cabeza de la cama más de 30°.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (947,1,224,6,'Intentar mantener en todo momento la alineación corporal.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (948,1,225,1,'Permanecer postrado en cama el menor tiempo posible.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (949,1,225,2,'Realizar cambios posturales cada 2 horas y cada hora en personas con silla de ruedas.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (950,1,225,3,'Evitar el roce de prominencias entre sí.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (951,1,225,4,'Aliviar la presión con: almohadas, colchón neumático (agua o aire), cojines de gel de frotación, protector de talones y codos, piel de borrego y felpa.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (952,1,225,5,'Evitar levantar la cabeza de la cama más de 30°.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (953,1,225,6,'Intentar mantener en todo momento la alineación corporal.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (954,1,226,1,'Permanecer postrado en cama el menor tiempo posible.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (955,1,226,2,'Realizar cambios posturales cada 2 horas y cada hora en personas con silla de ruedas.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (956,1,226,3,'Evitar el roce de prominencias entre sí.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (957,1,226,4,'Aliviar la presión con: almohadas, colchón neumático (agua o aire), cojines de gel de frotación, protector de talones y codos, piel de borrego y felpa.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (958,1,226,5,'Evitar levantar la cabeza de la cama más de 30°.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (959,1,226,6,'Intentar mantener en todo momento la alineación corporal.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (960,1,221,6,'Intercalará ambos movimientos, haciendo una pequeña pausa entre cada movimiento.','222_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (961,1,227,1,'Informar al paciente y familia de la técnica a realizar.','2_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (962,1,227,2,'Proporcionar intimidad.','72_1');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (963,1,227,3,'Lavarse las manos.','1_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (964,1,227,4,'Preparar el material y equipo, llevarlo al lugar donde se va a aspirar.','65_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (965,1,227,5,'Verificar la funcionalidad del aspirador.','89_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (966,1,227,6,'Verificar los signos vitales antes del procedimiento, Frecuencua cardiaca, frecuenca respiratoria, temperatura y presión arterial.','19_3');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (967,1,227,7,'Colocarse las gafas y cubrebocas','89_7');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (968,1,227,8,'Colocar a la persona en semifowler con la cabeza girada hacia un lado para aspiración orotraqueal. En caso de aspirar vía nasotraqueal, cambia la posición de la cabeza con el cuello en hiperextensión a 70°.','89_8');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (969,1,227,9,'La presión de aspiración deberá estar de 100 a 150 mmHg.','89_9');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (970,1,227,10,'Abrir la envoltura de la sonda de succión solo por el área donde será unido y conectarlo al tubo de conexión, asegurar que la sonda en su extremo distal no toque nada, ni salga completamente de la envoltura.','89_10');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (971,1,227,11,'Verter solución fisiológica o agua estéril en un frasco, aproximadamente 50 ml.','89_11');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (972,1,227,12,'Colocarse los guantes','2_4');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (973,1,227,13,'Realizar primero la aspiración por nasofaringe y después por orofaringe.','89_13');											";
        query2 += "	INSERT INTO 'procedimiento' ('id','id_idioma','id_manual','id_pasos','Texto','Imagen') VALUES (974,1,227,14,'Previo a la aspiración, primero deberá lubricar la punta de la sonda en la solución y aspirar para ver su permeabilidad.','89_13');											";

        try
        {
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query2; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

//            titulo.text+= e.Message;

        }
#endif

    //inputField = GameObject.Find("inputChat").GetComponent<InputField>();
    Setup(LANG_CODE);

#if UNITY_ANDROID
        SpeechToText.instance.onPartialResultsCallback = onPartialSpeechResul;

#endif
        SpeechToText.instance.onResultCallback = onFinalSpeechResul;
        TextToSpeech.instance.onStartCallBack = onEmpezarHablar;
        TextToSpeech.instance.onDoneCallback = onPararHablar;

        checarPermiso();
        titulo.text = "si inicio";
        string nombre;
        string texto;
     
        int principal;
        idioma_num = 1;
        // idioma_num=2; dbcmd = dbconn.CreateCommand(); // create empty command
  //              titulo.text += "si cargo la conexion";
//        String query="";
        try
        {
            dbcmd = dbconn.CreateCommand(); // create empty command
            query = string.Format("SELECT * FROM comandos where id_idioma=\"{0}\"", idioma_num);
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

            titulo.text += e.Message;
            Debug.Log(e.Message);

        }
        while (reader.Read())
            {
                // idreaders = reader.GetString(1);
                nombre = reader.GetString(2);
                texto = reader.GetString(3);
      //          titulo.text += " campo =" + nombre + "texto=" + texto;
            }
        try
        {
            dbcmd = dbconn.CreateCommand(); // create empty command
            query = string.Format("SELECT principal, categoria FROM menu where id_idioma=\"{0}\"", idioma_num);
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader();
        } catch (Exception e)
        {

          titulo.text += e.Message;
            Debug.Log(e.Message);
        }
        while (reader.Read())
                    {
                    principal = reader.GetInt32(0);
                    texto = reader.GetString(1);
                    if (principal == 1)
                    listOptions1.Add(texto);
                    if (principal == 2)
                    listOptions2.Add(texto);
                    if (principal == 3)
                    listOptions3.Add(texto);
                    if (principal == 4)
                    listOptions4.Add(texto);
                    if (principal == 5)
                    listOptions5.Add(texto);
  //                          titulo.text +=" principal =" + texto;

                    }
                   
        lista1.AddOptions(listOptions1);
        lista2.AddOptions(listOptions2);
        lista3.AddOptions(listOptions3);
        lista4.AddOptions(listOptions4);
        Query("SELECT * FROM manual");
        QueryProcedimiento("SELECT actividad, material.Texto AS material, duracion.Texto AS duracion FROM manual INNER JOIN material ON manual.id = material.id_manual INNER JOIN duracion ON manual.id = duracion.id_manual");

        empezarHablar("Bienvenido al sistema de cuidado de adultos mayores.Por favor selecciones su idioma y precione en inicio.");
        //reader_function();
        dbconn.Close();
    }
    //Inicio
    void checarPermiso()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif


    }


    public void carga1()
    {
        Debug.Log(" si entro  a carga1 ");
        if (id.text == "")
        {
            UI_Dialog.SetActive(true);
            mensaje.text = "Tiene que seleccionar una categoria y un proceso";
            Debug.Log(" si entro  ventana de dialogo se debe de ver ");

        }
        else
        {
            UI_Generales.SetActive(false);
            UI_Estimulacion.SetActive(false);
            UI_Auxilios.SetActive(false);
            UI_Especificos.SetActive(false);
            UI_VentanaEvento.SetActive(true);

            String Actividad;
            Debug.Log(" si entro vale  " + id_pasos.text);
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = string.Format("SELECT pasos,Actividad,dur,mat,auxiliar FROM manual where Actividad=\"{0}\"", id.text);// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                while (reader.Read())
                {
                    // idreaders = reader.GetString(1);
                    pasos_max.text = reader.GetInt32(0).ToString();
                  Actividad = reader.GetString(1);
                  dur_aux.text = reader.GetString(2);
                  mat_aux.text = reader.GetString(3);
                    auxiliar.text = reader.GetString(4);

                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
            }
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd2 = dbconn.CreateCommand();
                string sqlQuery2 = string.Format("select duracion.Texto as duracion FROM manual inner join duracion on manual.id=duracion.id_manual  where  manual.Actividad=\"{0}\"", id.text);// table name
                dbcmd2.CommandText = sqlQuery2;
                IDataReader reader2 = dbcmd2.ExecuteReader();
                while (reader2.Read())
                {
                    // idreaders = reader.GetString(1);
                    dur.text = reader2.GetString(0);

                }
                reader2.Close();
                reader2 = null;
                dbcmd2.Dispose();
                dbcmd2 = null;
                dbconn.Close();
            }
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd3 = dbconn.CreateCommand();
                string sqlQuery3 = string.Format("SELECT  material.Texto as material FROM manual  inner join material on manual.id=material.id_manual where  manual.Actividad=\"{0}\"", id.text);// table name
                dbcmd3.CommandText = sqlQuery3;
                IDataReader reader3 = dbcmd3.ExecuteReader();
                while (reader3.Read())
                {
                    mat.text = reader3.GetString(0);
                }
                reader3.Close();
                reader3 = null;
                dbcmd3.Dispose();
                dbcmd3 = null;
                dbconn.Close();
            }

            siguiente.SetActive(true);
            Final.SetActive(false);
            empezarHablar(id.text + auxiliar.text + mat_aux.text + mat.text + dur_aux.text + dur.text);

            //       dbconn = null;
        }

    }
    
    public void carga2()
    {
            int paso =1;
        String Actividad;
        Debug.Log(" si entro " + id.text+ "paso=" + id_pasos.text);
            paso = Int32.Parse(id_pasos.text) + 1;
            if (paso > 1)
            {
                anterios.SetActive(true);
            }
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            id_pasos.text = paso.ToString();
            string sqlQuery = string.Format("SELECT texto,Imagen,id_pasos FROM procedimiento inner join manual on manual.id=procedimiento.id_manual where  manual.Actividad=\"{0}\" and  procedimiento.id_pasos=\"{1}\"", id.text,id_pasos.text);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                // idreaders = reader.GetString(1);
                instruccion.text = reader.GetString(0);
                imagen.text = reader.GetString(1);
                Sprite newImage = Resources.Load<Sprite>(imagen.text);
                if (newImage != null)
                {
                    im.sprite = newImage;
                }
            }
            
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
        if (paso == Int32.Parse(pasos_max.text))
        {
            siguiente.SetActive(false);
            Final.SetActive(true);

        }


        empezarHablar(instruccion.text);

    }

    public void carga3()
    {
        int paso = 1;
        String Actividad;
        Debug.Log(" si entro " + id.text + "paso=" + id_pasos.text);
        using (dbconn = new SqliteConnection(conn))
        {
            paso = Int32.Parse(id_pasos.text) - 1;
            if (paso == 1) {
                anterios.SetActive(false);
            }


            id_pasos.text = paso.ToString();
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = string.Format("SELECT texto,Imagen,id_pasos FROM procedimiento inner join manual on manual.id=procedimiento.id_manual where  manual.Actividad=\"{0}\" and  procedimiento.id_pasos=\"{1}\"", id.text, id_pasos.text);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                // idreaders = reader.GetString(1);
                instruccion.text = reader.GetString(0);
                imagen.text = reader.GetString(1);
                Sprite newImage = Resources.Load<Sprite>(imagen.text);
                if (newImage != null)
                {
                    im.sprite = newImage;
                }
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            siguiente.SetActive(true);
            Final.SetActive(false);

        }
        empezarHablar(instruccion.text);

    }

    void DropdownValueChanged(TMPro.TMP_Dropdown change)
    {
      //  Debug.Log(" si entro " + idioma_num + " valor="+ change.options[change.value].text);
        string Actividad;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = string.Format("SELECT Actividad FROM manual inner join menu on manual.menu=menu.id where  manual.id_idioma=\"{0}\" and menu.categoria=\"{1}\" ", idioma_num, change.options[change.value].text);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            List<string> listOptions1 = new List<string>();
            listOptions1.Add("Seleccione el proceso");
            while (reader.Read())
            {
                lista21.options.Clear();
                // idreaders = reader.GetString(1);
                Actividad = reader.GetString(0);
                listOptions1.Add(Actividad);
       //         Debug.Log(" catalogo =" + change.value + " actividad =" + Actividad);
            }
            lista21.AddOptions(listOptions1);
            lista21.value=0;
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            //       dbconn = null;

        }

    }


    void DropdownValueChanged2(TMPro.TMP_Dropdown change)
    {
    //    Debug.Log(" si entro " + idioma_num + " valor=" + change.options[change.value].text);
        string Actividad;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = string.Format("SELECT Actividad   FROM manual inner join menu on manual.menu=menu.id where  manual.id_idioma=\"{0}\" and menu.categoria=\"{1}\" ", idioma_num, change.options[change.value].text);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            List<string> listOptions1 = new List<string>();
            listOptions1.Add("Seleccione el proceso");
            //       
            while (reader.Read())
            {
                lista22.options.Clear();
                // idreaders = reader.GetString(1);
                Actividad = reader.GetString(0);
                listOptions1.Add(Actividad);
    //            Debug.Log(" catalogo =" + change.value + " actividad =" + Actividad);
            }
            lista22.AddOptions(listOptions1);
            lista22.value = 0;
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            //       dbconn = null;


        }

    }
    void DropdownValueChanged3(TMPro.TMP_Dropdown change)
    {
        //Debug.Log(" si entro " + idioma_num + " valor=" + change.options[change.value].text);
        string Actividad;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = string.Format("SELECT Actividad    FROM manual inner join menu on manual.menu=menu.id where  manual.id_idioma=\"{0}\" and menu.categoria=\"{1}\" ", idioma_num, change.options[change.value].text);// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            List<string> listOptions1 = new List<string>();
            listOptions1.Add("Seleccione el proceso");
            while (reader.Read())
            {
                lista23.options.Clear();
                // idreaders = reader.GetString(1);
                Actividad = reader.GetString(0);
                listOptions1.Add(Actividad);
             //   Debug.Log(" catalogo =" + change.value + " actividad =" + Actividad);
            }
            lista23.AddOptions(listOptions1);
            lista23.value = 0;
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            //       dbconn = null;

        }
        
    }
        void DropdownValueChanged4(TMPro.TMP_Dropdown change)
    {
       //     Debug.Log(" si entro " + idioma_num + " valor=" + change.options[change.value].text);
            string Actividad;
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = string.Format("SELECT Actividad   FROM manual inner join menu on manual.menu=menu.id where  manual.id_idioma=\"{0}\" and menu.categoria=\"{1}\" ", idioma_num, change.options[change.value].text);// table name
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                List<string> listOptions1 = new List<string>();
                listOptions1.Add("Seleccione el proceso");
            while (reader.Read())
                {
                    lista24.options.Clear();
                    // idreaders = reader.GetString(1);
                    Actividad = reader.GetString(0);
                listOptions1.Add(Actividad);
                Debug.Log(" catalogo =" + change.options[change.value].text + " actividad =" + Actividad);
            }
            lista24.AddOptions(listOptions1);
            lista24.value = 0;
            reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                //       dbconn = null;

            }

        }
    void DropdownValueChangedsegundo(TMPro.TMP_Dropdown change)
    {
        Debug.Log(" segundo changed " + change.options[change.value].text);
        id.text = change.options[change.value].text;
        id2.text = change.options[change.value].text;

    }
    public void leer()
    {
        Debug.Log("estoy en leer");
        empezarHablar(instruccion.text);
    }

    // Update is called once per frame
    public void final() {
        id_pasos.text = "0";
        pasos_max.text = "0";
        dur.text = "";
        mat.text = "";
        dur_aux.text = "";
        mat_aux.text = "";




        anterios.SetActive(false);
        Sprite newImage = Resources.Load<Sprite>("Portada");
            im.sprite = newImage;
    }
    void Update()
    {

    }
    public void voz()
    {
        Debug.Log("Hola estoy en el metodo VOZ");
        /*string result = "proceso de Higiene de manos";
        Debug.Log("You: ");
        string[] splitData = Regex.Split(getProcedimiento(Regex.Split(result.ToLower(), @"\s|[,:;.?!-_]\s*")), @"&");//Se separa la info de la bd
        if (splitData.Length > 1)
        {
            nombre.text = splitData[0].Substring(0, 1).ToUpper() + splitData[0].Substring(1);//before.Substring(0, 1).ToUpper() + before.Substring(1);
            Debug.Log("nombre " + nombre.text);
            materiales.text = splitData[1];
            Debug.Log("materiales " + materiales.text);
            duracion.text = splitData[2];
            Debug.Log("duracion" + duracion.text);
            activacionUIVentanaEvento();
            unity ogjUnity = new unity();
            //ogjUnity.carga3(splitData[0]);
            ogjUnity.carga2Voz(nombre.text, id_pasos.text);
        }*/
        SpeechToText.instance.StartRecording();
        SpeechToText.instance.StopRecording();
    }
    
    #region Texto a Voz

    public void empezarHablar(string msg)
    {

        TextToSpeech.instance.StartSpeak(msg);

    }

    public void pararHablar()
    {
        TextToSpeech.instance.StopSpeak();

    }

    void onEmpezarHablar()
    {
        Debug.Log("Empezando hablando...");
    }

    void onPararHablar()
    {
        Debug.Log("Ya no esta hablando");
    }
    #endregion

    #region Voz a Texto

    public void empezarEscuchar()
    {
        SpeechToText.instance.StartRecording();

    }

    public void pararEscuchar()
    {
        SpeechToText.instance.StopRecording();

    }

    void onFinalSpeechResul(string result)
    {
        /* Eleccion del metodo que se va a utilizar */
        input.text = result;

        string comando = result.ToLower().Trim();
        Debug.Log("Comando de voz: " + comando);

        // COMANDOS DE NAVEGACION
        if (comando == "siguiente" || comando.Contains("siguiente paso"))
        {
            Debug.Log("VOZ -> Siguiente");
            if (id.text != "")
            {
                int pasoActual = Int32.Parse(id_pasos.text);
                int maxPaso = Int32.Parse(pasos_max.text);
                if (pasoActual < maxPaso) carga2();
                else Debug.Log("VOZ -> Ya se encuentra en el ultimo paso");
            }
            else carga1();
            return;
        }

        if (comando == "anterior" || comando.Contains("paso anterior"))
        {
            Debug.Log("VOZ -> Anterior");
            if (id.text != "")
            {
                int pasoActual = Int32.Parse(id_pasos.text);
                if (pasoActual > 1) carga3();
                else Debug.Log("VOZ -> Ya se encuentra en el primer paso");
            }
            else carga1();
            return;
        }

        // PRIMER PASO
        if (comando.Contains("primer paso") || comando.Contains("primer procedimiento"))
        {
            Debug.Log("VOZ -> Primer paso");
            if (id.text != "")
            {
                id_pasos.text = "0";
                anterios.SetActive(false);
                siguiente.SetActive(true);
                Final.SetActive(false);
                carga2();
            }
            else carga1();
            return;
        }

        // PROCEDIMIENTO + NUMERO DE PASO
        Match pasoMatch = Regex.Match(comando, @"(?:procedimiento\s+)?(?:paso|pasos)\s+(\d+)");
        if (pasoMatch.Success)
        {
            int pasoSolicitado;
            if (Int32.TryParse(pasoMatch.Groups[1].Value, out pasoSolicitado))
            {
                Debug.Log("VOZ -> Paso solicitado: " + pasoSolicitado);

                string[] procesoData = Regex.Split(
                    getProcedimiento(Regex.Split(comando, @"\s|[,:;.?!-_]\s*")), @"&");

                if (procesoData.Length >= 4 &&
                    !procesoData[0].Contains("No estoy seguro") &&
                    !procesoData[0].Contains("repetirlo"))
                {
                    id.text = procesoData[0].Substring(0, 1).ToUpper() + procesoData[0].Substring(1);
                    Debug.Log("Proceso reconocido: " + id.text);
                    id_pasos.text = "0";
                    carga1();
                }

                if (id.text != "")
                {
                    int maxPaso = Int32.Parse(pasos_max.text);
                    if (maxPaso > 0)
                    {
                        if (pasoSolicitado < 1) pasoSolicitado = 1;
                        if (pasoSolicitado > maxPaso) pasoSolicitado = maxPaso;

                        // carga2() incrementa antes de consultar la BD.
                        id_pasos.text = (pasoSolicitado - 1).ToString();
                        anterios.SetActive(pasoSolicitado > 1);
                        siguiente.SetActive(pasoSolicitado < maxPaso);
                        Final.SetActive(pasoSolicitado == maxPaso);
                        carga2();
                    }
                }
            }
            return;
        }

        // PROCEDIMIENTO: carga el primer paso del proceso actual.
        if (comando == "procedimiento" || comando == "ver procedimiento" ||
            comando == "mostrar procedimiento")
        {
            Debug.Log("VOZ -> Procedimiento");
            if (id.text != "")
            {
                id_pasos.text = "0";
                anterios.SetActive(false);
                siguiente.SetActive(true);
                Final.SetActive(false);
                carga2();
            }
            else carga1();
            return;
        }

        // DEFINICIONES
        List<string> requiredWords = new List<string> { "significado", "significa", "concepto", "refiere" };
        List<string> userMsg = new List<string>(Regex.Split(comando, @"\s|[,:;.?!-_]\s*"));
        int palabraRequerida = 0;
        requiredWords.ForEach(delegate (string i) { if (userMsg.Contains(i)) palabraRequerida++; });

        if (palabraRequerida > 0)
        {
            Debug.Log("VOZ -> Definicion");
            string[] splitData = Regex.Split(
                getResponse(Regex.Split(comando, @"\s|[,:;.?!-_]\s*")), @"&");
            if (splitData.Length > 0) empezarHablar(splitData[0]);
            return;
        }

        // NOMBRE DE PROCESO. Ejemplo: "Higiene de manos"
        Debug.Log("VOZ -> Buscar proceso");
        string[] proceso = Regex.Split(
            getProcedimiento(Regex.Split(comando, @"\s|[,:;.?!-_]\s*")), @"&");

        if (proceso.Length >= 4 &&
            !proceso[0].Contains("No estoy seguro") &&
            !proceso[0].Contains("repetirlo"))
        {
            id.text = proceso[0].Substring(0, 1).ToUpper() + proceso[0].Substring(1);
            Debug.Log("Proceso reconocido por voz: " + id.text);

            id_pasos.text = "0";
            anterios.SetActive(false);
            carga1();
            return;
        }

        Debug.Log("No se reconocio un proceso o comando de voz: " + result);
    }

    void onPartialSpeechResul(string result)
    {
        input.text = result;
    }
    #endregion


    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);



        // Console.WriteLine();

    }


    public void Query(string q)
    {

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            //connection.Open(); //Open connection to the database.
            using (var command = dbconn.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        Manual man = new Manual((reader["Actividad"] + "").ToLower(), (reader["Auxiliar"] + "").ToLower());
                        resultadosBD.Add(man);
                    }
                }
            }
            dbconn.Close();
        }
    }
    public void QueryProcedimiento(string q)
    {

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            //connection.Open(); //Open connection to the database.
            using (var command = dbconn.CreateCommand())
            {
                command.CommandText = q;
                using (IDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        Procedimiento proc = new Procedimiento((reader["actividad"] + "").ToLower(), reader["material"] + "", reader["duracion"] + "");
                        procedimientosBD.Add(proc);
                    }
                }
            }
        dbconn.Close();
        }
    }


    public string getResponse(string[] msgs)
    {

        if (resultadosBD.Count > 0)
        { //mientras se tenga informacion de la BD

            //para obtener las probabilidades de cada mensaje
            var highest_prob = new Dictionary<string, double>();

            void computeResponse(Dictionary<string, double> m, List<string> userMsg, string res, List<string> words, bool single = false, List<string> requiredWords = null)
            {
                /*se agrega al diccionario m cada mensaje con su probabilidad*/
                m.Add(res, probability(userMsg, words, single, requiredWords));
            }


            for (int i = 0; i < resultadosBD.Count; i++)
            {

                //Debug.Log(splitData[i - 1] + " " + splitData[i]);

                string[] nombre = Regex.Split(resultadosBD[i].getActividad(), @" ");//Separa el nombre para agregarlas como palabras clave

                List<string> coincidenciasManual = new List<string>(); //guardara las palabras clave direccion


                /*Agregara las palabras del nombre en la lista de coincidencias*/
                for (int j = 0; j < nombre.Length; j++)
                {
                    if (!nombre[j].Equals("") && !nombre[j].Equals(""))
                    {
                        string aux = nombre[j] + "";

                        coincidenciasManual.Add(aux);

                    }
                }

                /*Se agregan las palablas clave para obtener la direccion*/
                coincidenciasManual.Add("significado");
                coincidenciasManual.Add("significa");
                coincidenciasManual.Add("concepto");
                coincidenciasManual.Add("refiere");


                /*se manda a traer el metodo para sacar la probabilidad*/
                computeResponse(
                    highest_prob,
                    new List<string>(msgs), resultadosBD[i].getAuxiliar() + "&" + i,
                    coincidenciasManual,
                    true,
                    new List<string> { "significado", "significa", "concepto", "refiere","definicion","que"}
                    );



            }
            string varM = "";
            /*se compara para obtener el mensaje*/
            var keyForBiggest = new List<KeyValuePair<string, double>>();
            foreach (var item in highest_prob) keyForBiggest.Add(new KeyValuePair<string, double>(item.Key, item.Value));
            keyForBiggest.Sort((x, y) => x.Value.CompareTo(y.Value));
            /*retorna el mensaje*/

            varM = keyForBiggest.Count != 0 && keyForBiggest[keyForBiggest.Count - 1].Value > 0 ? keyForBiggest[keyForBiggest.Count - 1].Key : anyResponse();
            //Match m2 = respuestaregex.Match(varM);
            if (Regex.IsMatch(varM, "sinInfo"))
            {
                varM = "Por el momento no se cuenta con está información";
                Debug.Log("if " + varM);
            }
            Debug.Log("Resultado: " + varM);
            return varM;

        }
        return "no hay respuesta";
    }
    public string getProcedimiento(string[] msgs)
    {

        if (procedimientosBD.Count > 0)
        { //mientras se tenga informacion de la BD

            //para obtener las probabilidades de cada mensaje
            var highest_prob = new Dictionary<string, double>();

            void computeResponse(Dictionary<string, double> m, List<string> userMsg, string res, List<string> words, bool single = false, List<string> requiredWords = null)
            {
                /*se agrega al diccionario m cada mensaje con su probabilidad*/
                m.Add(res, probability(userMsg, words, single, requiredWords));
            }


            for (int i = 0; i < procedimientosBD.Count; i++)
            {

                //Debug.Log(splitData[i - 1] + " " + splitData[i]);

                string[] nombre = Regex.Split(procedimientosBD[i].getActividad(), @" ");//Separa el nombre para agregarlas como palabras clave

                List<string> coincidenciasManual = new List<string>(); //guardara las palabras clave direccion


                /*Agregara las palabras del nombre en la lista de coincidencias*/
                for (int j = 0; j < nombre.Length; j++)
                {
                    if (!nombre[j].Equals("") && !nombre[j].Equals(""))
                    {
                        string aux = nombre[j] + "";
                        coincidenciasManual.Add(aux);
                    }
                }
                /*Se agregan las palablas clave para obtener la direccion*/
                coincidenciasManual.Add("procedimiento");
                coincidenciasManual.Add("proceso");
                coincidenciasManual.Add("realiza");
                coincidenciasManual.Add("hace");


                /*se manda a traer el metodo para sacar la probabilidad*/
                computeResponse(
                highest_prob,
                new List<string>(msgs),
                procedimientosBD[i].getActividad() + "&" +
                procedimientosBD[i].getMaterial() + "&" +
                procedimientosBD[i].getDuracion() + "&" + i,
                coincidenciasManual,
                true,
                null
                );
            }
            string varM = "";
            /*se compara para obtener el mensaje*/
            var keyForBiggest = new List<KeyValuePair<string, double>>();
            foreach (var item in highest_prob) keyForBiggest.Add(new KeyValuePair<string, double>(item.Key, item.Value));
            keyForBiggest.Sort((x, y) => x.Value.CompareTo(y.Value));
            /*retorna el mensaje*/

            varM = keyForBiggest.Count != 0 && keyForBiggest[keyForBiggest.Count - 1].Value > 0 ? keyForBiggest[keyForBiggest.Count - 1].Key : anyResponse();
            //Match m2 = respuestaregex.Match(varM);
            if (Regex.IsMatch(varM, "sinInfo"))
            {
                varM = "Por el momento no se cuenta con está información";
                Debug.Log("if " + varM);
            }
            Debug.Log("Resultado: " + varM);
            return varM;

        }
        return "no hay respuesta";
    }
    /*Si no hay ningun resultado muestra un mensaje aleatorio*/
    public string anyResponse()
    {
        var w = new List<string> { "¿Puedes repetirlo de nuevo?", "No estoy seguro de lo que quieres" };
        return w[rnd.Next(0, w.Count)];
    }
    public double probability(List<string> userMsg, List<string> words, bool single = false, List<string> requiredWords = null)
    {
        requiredWords = requiredWords ?? new List<string>();
        /*Contadores*/
        double certainty = 0;
        double certainty2 = 0;
        bool hasRequired = true;
        double longWords = 0;
        int palabraRequerida = 0;
        /*Si no hay una palabra requerida retorna la probailidad de 0 */
        requiredWords.ForEach(delegate (string i) { if (userMsg.Contains(i)) palabraRequerida++; });
        if (palabraRequerida == 0) return 0;

        /*compara el mensaje del usario con las palabras clave y obtener un conteo de cuantas coincidieron*/
        userMsg.ForEach(delegate (string i) { if (words.Contains(i)) certainty++; });
        foreach (string uM in userMsg)
        {
            longWords = 0;
            foreach (string uM2 in words)
            {
                longWords++;
                //Debug.Log("Comparacion:::" + uM.GetType() + " " + uM + " = " + uM2.GetType() + " " + uM2);
                string s1 = uM;
                string s2 = Regex.Replace(uM2, @"[^\w\.@-]", "");
                if (s1.Equals(s2))
                {
                    //Debug.Log("entro");
                    certainty2++;
                }
                else
                {
                    //Debug.Log("no entro");
                }
            }
        }
        //Debug.Log("Certainty:::" + palabraRequerida);
        //se obtiene el porcentaje
        double percentage = certainty2 / longWords;
        // requiredWords.ForEach(w => { if (!userMsg.Contains(w)) hasRequired = false; });
        /*Se retorna la probabilidad*/
        if (hasRequired || single)
        {
            double prob = percentage * 100;
            Debug.Log("Probabilidad: " + prob);
            return prob;
        }
        return 0;
    }

    public class Manual
    {

        private string actividad;
        private string auxiliar;

        public Manual(string actividad, string auxiliar)
        {
            this.actividad = actividad;
            this.auxiliar = auxiliar;
        }

        public string getActividad()
        {
            return actividad;
        }
        public string getAuxiliar()
        {
            return auxiliar;
        }

    }
    public class Procedimiento
    {

        private string actividad;
        private string material;
        private string duracion;

        public Procedimiento(string actividad, string material, string duracion)
        {
            this.actividad = actividad;
            this.material = material;
            this.duracion = duracion;
        }

        public string getActividad()
        {
            return actividad;
        }
        public string getMaterial()
        {
            return material;
        }
        public string getDuracion()
        {
            return duracion;
        }
    }
    public void activacionUIVentanaEvento()
    {
        UI_Portada.SetActive(false);
        UI_Introduccion.SetActive(false);
        UI_Modulos.SetActive(false);
        UI_Generales.SetActive(false);
        UI_Estimulacion.SetActive(false);
        UI_Especificos.SetActive(false);
        UI_Auxilios.SetActive(false);
        UI_Cuidador.SetActive(false);
        UI_Bitacora.SetActive(false);
        UI_Dialog.SetActive(false);
        UI_VentanaProcedimiento.SetActive(false);
        UI_VentanaEvento.SetActive(true);
        UI_PieLogos.SetActive(true);
        UI_PieBtnVoz.SetActive(true);
    }
}
