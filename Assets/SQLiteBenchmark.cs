using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;

public class SQLiteBenchmark : MonoBehaviour
{
    public int repetitions = 50;
    public string databaseFileName = "manual.s3db";
    public int testManualId = 1;

    private string dbPath;
    private string csvPath;
    private string lastMessage = "";
    private bool showMessage = false;
    private bool benchmarkFinished = false;

    void Start()
    {
        StartCoroutine(RunBenchmark());
    }

    IEnumerator RunBenchmark()
    {
        yield return new WaitForSeconds(1f);

        dbPath = Path.Combine(Application.persistentDataPath, databaseFileName);

        if (!File.Exists(dbPath))
        {
            ShowMessage("PRUEBA SQL\n\nNo se encontró la base de datos.\n\n" +
                        "Archivo buscado:\n" + dbPath);
            yield break;
        }

        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        csvPath = Path.Combine(Application.persistentDataPath,
                               "sqlite_benchmark_" + stamp + ".csv");

        List<BenchmarkResult> results = new List<BenchmarkResult>();

        results.Add(RunTest("T1_Consulta_manual",
            "SELECT id, Actividad, Auxiliar, pasos, dur FROM manual LIMIT 1;"));

        results.Add(RunTest("T2_Consulta_procedimiento",
            "SELECT id, id_pasos, Texto, Imagen FROM procedimiento " +
            "WHERE id_manual = " + testManualId + " ORDER BY id_pasos;"));

        results.Add(RunTest("T3_Consulta_paso",
            "SELECT id, id_pasos, Texto, Imagen FROM procedimiento " +
            "WHERE id_manual = " + testManualId + " AND id_pasos = 1;"));

        results.Add(RunTest("T4_Consulta_join_manual_procedimiento",
            "SELECT m.Actividad, m.dur, m.mat, p.id_pasos, p.Texto, p.Imagen " +
            "FROM manual m INNER JOIN procedimiento p ON m.id = p.id_manual " +
            "WHERE m.id = " + testManualId + " ORDER BY p.id_pasos;"));

        results.Add(RunTest("T5_Consulta_todos_procedimientos",
            "SELECT p.id_manual, p.id_pasos, p.Texto, p.Imagen " +
            "FROM procedimiento p ORDER BY p.id_manual, p.id_pasos;"));

        WriteCsv(results);
        benchmarkFinished = true;

        string message =
            "PRUEBA SQL TERMINADA\n\n" +
            "5 pruebas × " + repetitions + " repeticiones\n\n" +
            "Archivo:\n" + Path.GetFileName(csvPath) +
            "\n\nPulsa COMPARTIR CSV para enviarlo o guardarlo.";

        ShowMessage(message);
    }

    BenchmarkResult RunTest(string name, string sql)
    {
        List<double> times = new List<double>();
        int valid = 0;

        using (SqliteConnection connection =
               new SqliteConnection("URI=file:" + dbPath))
        {
            connection.Open();

            ExecuteQuery(connection, sql); // calentamiento

            for (int i = 0; i < repetitions; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int rows = ExecuteQuery(connection, sql);
                sw.Stop();

                times.Add(sw.Elapsed.TotalMilliseconds);
                if (rows > 0) valid++;
            }
        }

        times.Sort();

        return new BenchmarkResult
        {
            test = name,
            repetitions = repetitions,
            mean = Mean(times),
            median = Median(times),
            min = times[0],
            max = times[times.Count - 1],
            p95 = Percentile(times, 0.95),
            validResults = valid
        };
    }

    int ExecuteQuery(SqliteConnection connection, string sql)
    {
        int rows = 0;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = sql;

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    rows++;
            }
        }

        return rows;
    }

    void WriteCsv(List<BenchmarkResult> results)
    {
        StringBuilder csv = new StringBuilder();

        csv.AppendLine("test,repetitions,mean_ms,median_ms,min_ms,max_ms,p95_ms,valid_results");

        foreach (BenchmarkResult r in results)
        {
            csv.AppendLine(
                r.test + "," +
                r.repetitions + "," +
                r.mean.ToString("F4", System.Globalization.CultureInfo.InvariantCulture) + "," +
                r.median.ToString("F4", System.Globalization.CultureInfo.InvariantCulture) + "," +
                r.min.ToString("F4", System.Globalization.CultureInfo.InvariantCulture) + "," +
                r.max.ToString("F4", System.Globalization.CultureInfo.InvariantCulture) + "," +
                r.p95.ToString("F4", System.Globalization.CultureInfo.InvariantCulture) + "," +
                r.validResults
            );
        }

        File.WriteAllText(csvPath, csv.ToString(), Encoding.UTF8);
    }

    void ShareCsv()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            string authority = Application.identifier + ".fileprovider";

            using (AndroidJavaClass intentClass =
                   new AndroidJavaClass("android.content.Intent"))
            using (AndroidJavaObject intent =
                   new AndroidJavaObject("android.content.Intent"))
            using (AndroidJavaClass unityPlayer =
                   new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaObject fileProvider =
                    new AndroidJavaObject(
                        "androidx.core.content.FileProvider");

                // FileProvider requiere estar declarado en el proyecto Android.
                // Si tu proyecto usa un FileProvider propio, adapta esta autoridad.
                AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>(
                    "getUriForFile",
                    activity,
                    authority,
                    new AndroidJavaObject("java.io.File", csvPath));

                intent.Call<AndroidJavaObject>("setAction",
                    intentClass.GetStatic<string>("ACTION_SEND"));

                intent.Call<AndroidJavaObject>("setType", "text/csv");
                intent.Call<AndroidJavaObject>("putExtra",
                    intentClass.GetStatic<string>("EXTRA_STREAM"), uri);

                intent.Call<AndroidJavaObject>("addFlags",
                    intentClass.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));

                AndroidJavaObject chooser =
                    intentClass.CallStatic<AndroidJavaObject>(
                        "createChooser", intent, "Compartir CSV");

                activity.Call("startActivity", chooser);
            }
        }
        catch (Exception ex)
        {
            ShowMessage("No se pudo abrir el menú de compartir.\n\n" +
                        "El CSV sigue guardado en:\n" + csvPath +
                        "\n\nDetalle:\n" + ex.Message);
        }
#else
        ShowMessage("El botón COMPARTIR CSV funciona en Android.\n\n" +
                    "En el Editor el archivo está en:\n" + csvPath);
#endif
    }

    void ShowMessage(string message)
    {
        lastMessage = message;
        showMessage = true;
    }

    void OnGUI()
    {
        if (!showMessage)
            return;

        GUIStyle box = new GUIStyle(GUI.skin.box);
        box.fontSize = Mathf.Max(22, Screen.width / 30);
        box.wordWrap = true;
        box.alignment = TextAnchor.UpperLeft;

        GUIStyle button = new GUIStyle(GUI.skin.button);
        button.fontSize = Mathf.Max(24, Screen.width / 27);

        float margin = Screen.width * 0.05f;
        float width = Screen.width - margin * 2;

        GUI.Box(
            new Rect(margin, Screen.height * 0.08f,
                     width, Screen.height * 0.60f),
            lastMessage, box);

        if (benchmarkFinished &&
            GUI.Button(
                new Rect(margin, Screen.height * 0.70f,
                         width, Screen.height * 0.10f),
                "COMPARTIR CSV", button))
        {
            ShareCsv();
        }

        if (GUI.Button(
            new Rect(margin, Screen.height * 0.82f,
                     width, Screen.height * 0.09f),
            "CERRAR", button))
        {
            showMessage = false;
        }
    }

    double Mean(List<double> v)
    {
        double sum = 0;
        foreach (double x in v) sum += x;
        return sum / v.Count;
    }

    double Median(List<double> v)
    {
        int n = v.Count;
        if (n % 2 == 1) return v[n / 2];
        return (v[n / 2 - 1] + v[n / 2]) / 2.0;
    }

    double Percentile(List<double> v, double p)
    {
        if (v.Count == 1) return v[0];

        double pos = (v.Count - 1) * p;
        int lower = (int)Math.Floor(pos);
        int upper = (int)Math.Ceiling(pos);

        if (lower == upper) return v[lower];

        return v[lower] +
               (v[upper] - v[lower]) * (pos - lower);
    }

    class BenchmarkResult
    {
        public string test;
        public int repetitions;
        public double mean;
        public double median;
        public double min;
        public double max;
        public double p95;
        public int validResults;
    }
}
