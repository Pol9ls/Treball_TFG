using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

public class MoverDefinitivo : MonoBehaviour
{
    private float porcentaje = 80f;
    private const int maxDentro = 110;
    private float minDistanceBetweenNPCs = 1.7f;
    private List<NavMeshAgent> listaAgentes = new List<NavMeshAgent>();

    private List<Vector3> posicionesDentro = new List<Vector3>();
    private List<Vector3> posicionesFuera = new List<Vector3>();
    private List<Vector3> posicionesValidas = new List<Vector3>();

    private OcupacioSemanal ocupacioSemana;

    public TMP_Text reloj;
    public TMP_Text dia;
    public TMP_Text porcentaje_text;
    public TMP_Text gente;
    public TMP_Text porcentaje_esperado;

    private string reloj_anterior;
    private string dia_anterior;

    private bool cambiar;

    private string text_porcentaje;
    private int porcentaje_real;

    private int dentro;

    public GenerarDefinitivo generarDefinitivo;
    public PruebaTemporizador pruebaTemporizador;
    public DeteccionCollider deteccionCollider;
    public NavMeshArea navMeshArea;

    void Start()
    {
        cambiar = false;

        Invoke(nameof(Conseguir_Posiciones), 1f);
        Invoke(nameof(CarregarJSON), 1f);
        Invoke(nameof(Iniciar), 1f);
    }

    void Iniciar()
    {
        foreach (GameObject npc in GenerarDefinitivo.listanpc)
        {
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
                listaAgentes.Add(agent);
        }
        Debug.Log(porcentaje);
        Mover();
        Cambio_Temporizador();
        Debug.Log("Total NPCs: " + listaAgentes.Count);
    }

    void Update()
    {
        if (cambiar != false)
        {
            if(reloj_anterior != reloj.text || dia_anterior != dia.text)
            {
                pruebaTemporizador.CambioHora();
                porcentaje = ObtenerPorcentajeActual();
                Debug.Log("El porcentaje total es " + porcentaje);
                text_porcentaje = porcentaje.ToString() + "%";
                porcentaje_esperado.text = text_porcentaje;
                reloj_anterior = reloj.text;
                dia_anterior = dia.text;
                Mover();
                Cambio_Temporizador();
            }
            Gente_Dentro();

        }
    }

    void CarregarJSON()
    {
        reloj_anterior = reloj.text;
        dia_anterior = dia.text;

        TextAsset jsonText = Resources.Load<TextAsset>("data");
        if (jsonText != null)
        {
            ocupacioSemana = JsonUtility.FromJson<OcupacioSemanal>(jsonText.text);
            cambiar = true;
        }
        else
        {
            Debug.Log("Error con el porcentaje");
        }

        porcentaje = ObtenerPorcentajeActual();
    }

    float ObtenerPorcentajeActual()
    {
        int horaActual;

        if (!int.TryParse(reloj.text.Split(':')[0], out horaActual))
        {
            Debug.Log("Fallo al obtener la hora");
            return porcentaje;
        }

        List<OcupacioHora> listaDelDia = dia.text switch
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

    void Conseguir_Posiciones()
    {
        posicionesDentro = ObtenerPosicionesDesdeNavMesh("Suelo");
        posicionesFuera = ObtenerPosicionesDesdeNavMesh("Fuera");

        posicionesDentro = posicionesDentro.OrderBy(x => UnityEngine.Random.value).ToList();

        Debug.Log("Moviment Posicions dins: " + posicionesDentro.Count);
        Debug.Log("Moviment Posicions fora: " + posicionesFuera.Count);
    }

    void Mover()
    {
        int numDentro = Mathf.RoundToInt(porcentaje * maxDentro / 100f);
        int numFuera = listaAgentes.Count - numDentro;
        int area_fuera = 1 << NavMesh.GetAreaFromName("Fuera");

        for (int i = 0; i < listaAgentes.Count; i++)
        {
            NavMeshAgent agent = listaAgentes[i];

            if (i < numDentro && i < posicionesDentro.Count)
            {
                Vector3 destino = posicionesDentro[i % posicionesDentro.Count];


                agent.SetDestination(destino);

                MovimientoFuera mf = agent.GetComponent<MovimientoFuera>();
                if (mf != null)
                {
                    EstadoNPC estadoNPC = agent.GetComponent<EstadoNPC>();
                    if(estadoNPC != null)
                    {
                        estadoNPC.entrando = true;
                        estadoNPC.saliendo = false;
                    }
                    Destroy(mf);
                }
                GameObject npc = agent.gameObject;             


            }
            else if ((i - numDentro) < posicionesFuera.Count)
            {
                Vector3 destino = posicionesFuera[(i - numDentro) % posicionesFuera.Count];
                MovimientoFuera mf = agent.GetComponent<MovimientoFuera>();

                bool yaEstabaFuera = mf != null;
                bool destinoActualSigueSiendoFuera = false;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(agent.destination, out hit, 0.5f, area_fuera))
                {
                    destinoActualSigueSiendoFuera = true;
                }

                
                if (!yaEstabaFuera || !destinoActualSigueSiendoFuera)
                {
                    agent.SetDestination(destino);
                    if (!yaEstabaFuera)
                    {

                        EstadoNPC estadoNPC = agent.GetComponent<EstadoNPC>();
                        if (estadoNPC != null)
                        {
                            if (estadoNPC.empiezafuera)
                            {
                                estadoNPC.empiezafuera = false;
                            }
                            else
                            {
                                estadoNPC.entrando = false;
                                estadoNPC.saliendo = true;
                            }
                        }
                        agent.gameObject.AddComponent<MovimientoFuera>();

                    }
                        
                }
            }
        }
    }

    List<Vector3> ObtenerPosicionesDesdeNavMesh(string area)
    {
        List<Vector3> posiciones = new List<Vector3>();
        int areaIndex = 1 << NavMesh.GetAreaFromName(area);
        NavMeshTriangulation triangulo = NavMesh.CalculateTriangulation();

        for (int i = 0; i < triangulo.indices.Length; i += 3)
        {
            Vector3 v1 = triangulo.vertices[triangulo.indices[i]];
            Vector3 v2 = triangulo.vertices[triangulo.indices[i + 1]];
            Vector3 v3 = triangulo.vertices[triangulo.indices[i + 2]];
            Vector3 centro = (v1 + v2 + v3) / 3;

            
            if (area == "Fuera")
            {
                Vector3[] candidatos = new Vector3[] { v1, v2, v3, centro };

                foreach (var punto in candidatos)
                {
                    if (NavMesh.SamplePosition(punto, out NavMeshHit hit, 0.1f, areaIndex))
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
                if (NavMesh.SamplePosition(centro, out NavMeshHit hit, 0.1f, areaIndex))
                {
                    if (EsValida(hit.position))
                    {
                        posicionesValidas.Add(hit.position);
                        posiciones.Add(hit.position);
                    }
                }
            }

        }
        return posiciones;
    }

    bool EsValida(Vector3 position)
    {
        foreach (Vector3 pos in posicionesValidas)
        {
            if (Vector3.Distance(pos, position) < minDistanceBetweenNPCs)
                return false;
        }
        return true;
    }

    void Gente_Dentro()
    {
        dentro = 0;
        dentro = generarDefinitivo.agentesDentro.Count;
        gente.text = "Hi ha " + dentro + " persones dins";
        float aux = dentro * 100f;
        porcentaje_real = Mathf.RoundToInt(aux / 110f);
        text_porcentaje = porcentaje_real.ToString() + "%";
        porcentaje_text.text = text_porcentaje;
        deteccionCollider.Actualizar_Personas(porcentaje,porcentaje_real);
    }

    void Cambio_Temporizador()
    {
        int personas = 0;

        foreach (GameObject npc in generarDefinitivo.agentesDentro)
        {
            EstadoNPC estado = npc.GetComponent<EstadoNPC>();
            if (estado.saliendo == false)
            {
                personas += 1;
            }
        }
        if (personas <= 60 && personas >= 30)
        {
            Debug.Log("Sacamos a 3 porque hay " + personas);
            pruebaTemporizador.Sacar(3);
        }
        else if (personas > 60)
        {
            pruebaTemporizador.Sacar(5);

            Debug.Log("Sacamos a 5 porque hay " + personas);
        }

    }

}

