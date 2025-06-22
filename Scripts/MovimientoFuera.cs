using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoFuera : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool movimiento = false;
    private List<Vector3> puntosDestino = new List<Vector3>();
    private int destinoActual = 0;
    private bool carril1;
    private Vector3 puerta1 = new Vector3(0, 0, 85);
    private Vector3 puerta2 = new Vector3(106, 0, 9);
    private GenerarDefinitivo generarDefinitivo;

    void Start()
    {
        generarDefinitivo = FindFirstObjectByType<GenerarDefinitivo>();

        if (!generarDefinitivo.agentesDentro.Contains(gameObject))
        {
            if (Random.value > 0.7f)
            {
                return;
            }
        }
        movimiento = true;
        agent = GetComponent<NavMeshAgent>();

        carril1 = Random.value < 0.5f;

        if (carril1)
        {
            puntosDestino.Add(new Vector3(-116f, 0f, 84f));
            puntosDestino.Add(new Vector3(115f, 0f, 84f));
            puntosDestino.Add(new Vector3(115f, 0f, -101f));
            puntosDestino.Add(new Vector3(-115f, 0f, -101f));
        }
        else
        {
            puntosDestino.Add(new Vector3(-92f, 0f, 68f));
            puntosDestino.Add(new Vector3(-92f, 0f, -73f));
            puntosDestino.Add(new Vector3(92f, 0f, -73f));
            puntosDestino.Add(new Vector3(90f, 0f, 68f));
        }
            destinoActual = BuscarPuntoMasCercano(transform.position);
            agent.SetDestination(puntosDestino[destinoActual]);
    }

    void Update()
    {
         if (movimiento && puntosDestino.Count != 0 && !agent.pathPending && agent.remainingDistance < 1.5f)
         {
           destinoActual = (destinoActual + 1) % puntosDestino.Count;
           agent.SetDestination(puntosDestino[destinoActual]);
           EstadoNPC estadoNPC = agent.GetComponent<EstadoNPC>();
           if (estadoNPC != null)
           {
                estadoNPC.entrando = false;
                estadoNPC.saliendo = false;
           }
        }
    }

    int BuscarPuntoMasCercano(Vector3 posicion)
    {
        int indice = 0;
        float distancia = Mathf.Infinity;

        for (int i = 0; i < puntosDestino.Count; i++)
        {
            float distancia_actual = Vector3.Distance(posicion, puntosDestino[i]);
            if (distancia_actual < distancia)
            {
                distancia = distancia_actual;
                indice = i;
            }
        }

        return indice;
    }
}

