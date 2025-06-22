using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;
using NUnit.Framework;
using System.Linq;
using System;

public class GenerarDefinitivo : MonoBehaviour
{
    public List<GameObject> npcPrefabs;
    private float distanciaMinima = 1.7f;

    public static List<GameObject> listanpc = new List<GameObject>();
    private List<Vector3> posiciones = new List<Vector3>();

    public List<GameObject> agentesDentro = new List<GameObject>();
    public List<GameObject> agentesFuera = new List<GameObject>();

    private OcupacioSemanal ocupacioSemana;
    public float porcentaje = 80f;

    public TMP_Text reloj;
    public TMP_Text dia;
    public TMP_Text porcentaje_text;
    public TMP_Text gente;
    public TMP_Text porcentaje_esperado;

    private int maxdentro = 110;
    private int total = 160;

    private string text_porcentaje;



    private string[] semana = { "DILLUNS", "DIMARTS", "DIMERCRES", "DIJOUS", "DIVENDRES", "DISSABTE", "DIUMENGE" };


    void Start()
    {
        CarregarJSON();
        porcentaje = ObtenerPorcentajeActual();
        text_porcentaje = porcentaje.ToString() + "%";
        porcentaje_text.text = text_porcentaje;
        porcentaje_esperado.text = text_porcentaje;
        
        int numDentro = Mathf.RoundToInt(porcentaje * maxdentro / 100f);
        int numFuera = total - numDentro;
        Debug.Log("Al principio tenemos " + numDentro + " dentro y " + numFuera + " fuera");
        
        List<Vector3> posicionesDentro = ObtenerPosiciones("Suelo", numDentro);
        List<Vector3> posicionesFuera = ObtenerPosiciones("Fuera", numFuera);
        Debug.Log($"Dentro: {posicionesDentro.Count}, Fuera: {posicionesFuera.Count}");

        CrearNPCs(posicionesDentro, true);
        CrearNPCs(posicionesFuera, false);
        
        Invoke(nameof(Gente_Dentro), 0.25f);
    }

    List<Vector3> ObtenerPosiciones(string area, int cantidad)
    {
        List<Vector3> posicionesValidas = new List<Vector3>();
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        int areaIndex = NavMesh.GetAreaFromName(area);

        for (int i = 0; i < triangulation.indices.Length && posicionesValidas.Count < cantidad; i += 3)
        {
            Vector3 v1 = triangulation.vertices[triangulation.indices[i]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[i + 1]];
            Vector3 v3 = triangulation.vertices[triangulation.indices[i + 2]];
            Vector3 centro = (v1 + v2 + v3) / 3;

            if (area == "Fuera")
            {
                Vector3[] candidatos = new Vector3[] { v1, v2, v3, centro };

                foreach (var punto in candidatos)
                {
                    if (NavMesh.SamplePosition(punto, out NavMeshHit hit, 0.1f, 1 << areaIndex))
                    {
                        if (EsValida(hit.position))
                        {
                            posicionesValidas.Add(hit.position);
                            posiciones.Add(hit.position);
                        }
                    }
                }
            }
            else
            {
                if (NavMesh.SamplePosition(centro, out NavMeshHit hit, 0.1f, 1 << areaIndex))
                {
                    if (EsValida(hit.position))
                    {
                        posicionesValidas.Add(hit.position);
                        posiciones.Add(hit.position);
                    }
                }
            }

        }

        return posicionesValidas;
    }

    void CrearNPCs(List<Vector3> posiciones, bool dentro)
    {
        foreach (Vector3 pos in posiciones)
        {
            GameObject prefab = npcPrefabs[UnityEngine.Random.Range(0, npcPrefabs.Count)];
            GameObject npc = Instantiate(prefab, pos, Quaternion.identity);

            listanpc.Add(npc);
            if (!dentro)
            {
                agentesFuera.Add(npc);
            }
        }
    }

    bool EsValida(Vector3 position)
    {
        foreach (Vector3 pos in posiciones)
        {
            if (Vector3.Distance(pos, position) < distanciaMinima)
                return false;
        }
        return true;
    }

    void CarregarJSON()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("data");
        if (jsonText != null)
        {
            ocupacioSemana = JsonUtility.FromJson<OcupacioSemanal>(jsonText.text);
        }
        else
        {
            Debug.Log("Error con el porcentaje");
        }
    }

    float ObtenerPorcentajeActual()
    {
        DateTime hora_datetime = DateTime.Now;

        int horaActual = hora_datetime.Hour;

        Debug.Log("La hora: " + horaActual);

        int diaSemana = (int)DateTime.Now.DayOfWeek;


        if (diaSemana == 0)
        {
            diaSemana = 6;
        }
        else
        {
            diaSemana -= 1;
        }

        string dia_string = semana[diaSemana];

        List<OcupacioHora> listaDelDia = dia_string switch
        {
            "DIUMENGE" => ocupacioSemana.Su,
            "DILLUNS" => ocupacioSemana.Mo,
            "DIMARTS" => ocupacioSemana.Tu,
            "DIMECRES" => ocupacioSemana.We,
            "DIJOUS" => ocupacioSemana.Th,
            "DIVENDRES" => ocupacioSemana.Fr,
            "DISSABTE" => ocupacioSemana.Sa,
            _ => null
        };

        OcupacioHora ocupacion = listaDelDia.Find(o => o.hour == horaActual);

        Debug.Log(ocupacion.occupancyPercent + " " + ocupacion.hour);

        if (ocupacion != null)
        {
            return ocupacion.occupancyPercent;
        }
        else
        {
            porcentaje = 0;
            return porcentaje;
        }
    }
    void Gente_Dentro()
    {
        int dentro = 0;

        dentro = agentesDentro.Count;

        gente.text = "Hi " + dentro + " persones dins";

    }

}


