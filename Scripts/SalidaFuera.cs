using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SalidaFuera : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool movimiento = false;
    private List<Vector3> puntosDestino = new();
    private int destinoActual = 0;
    private bool carril1;
    private Vector3 puerta1 = new Vector3(0, 0, 35);
    private Vector3 puerta2 = new Vector3(60, 0, 9);
    private bool lateral;
    void Start()
    {
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
        destinoActual = PuntoSalida();
       
    }

    void Update()
    {
        if (movimiento && puntosDestino.Count != 0 && !agent.pathPending && agent.remainingDistance < 1.5f)
        {
            destinoActual = (destinoActual + 1) % puntosDestino.Count;
            agent.SetDestination(puntosDestino[destinoActual]);
        }
    }

    int PuntoSalida()
    {
        int indice = 0;

        if (agent.destination == puerta1)
        {
            if (carril1)
            {
                if (Random.value > 0.5f)
                {
                    indice = 1;
                }
                else
                {
                    indice = 0;
                }
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    indice = 1;
                }
                else
                {
                    indice = 2;
                }
            }
        }
        else
        {
            if (carril1)
            {
                if (Random.value > 0.5f)
                {
                    indice = 3;
                }
                else
                {
                    indice = 0;
                }
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    indice = 3;
                }
                else
                {
                    indice = 2;
                }
            }
        }

        return indice;
    }
}

