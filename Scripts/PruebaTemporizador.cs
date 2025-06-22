using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PruebaTemporizador : MonoBehaviour
{

    public GenerarDefinitivo generarDefinitivo;
    public List<GameObject> agentes_salen = new List<GameObject>();
    public List<GameObject> agentes_entran = new List<GameObject>();
    private int max;
    private bool paramos = false;
    public void Sacar(int num)
    {
        paramos = false;
        int index_salen = 0;
        while (agentes_salen.Count < num && !paramos)
        {
            GameObject npc = generarDefinitivo.agentesDentro[index_salen];
            EstadoNPC estado = npc.GetComponent<EstadoNPC>();

            if (!estado.saliendo && !estado.entrando)
            {
                agentes_salen.Add(npc);
            }
            index_salen += 1;

            if (index_salen >= generarDefinitivo.agentesDentro.Count)
            {
                paramos = true;
            }
        }

        paramos = false;

        int index_entran = 0;
        max = generarDefinitivo.agentesDentro.Count;

        while (agentes_entran.Count < max && !paramos)
        {
            GameObject npc = generarDefinitivo.agentesFuera[index_entran];
            EstadoNPC estado = npc.GetComponent<EstadoNPC>();

            if (estado.saliendo == false && estado.entrando == false)
            {
                agentes_entran.Add(npc);
            }
            index_entran += 1;

            if (index_entran >= generarDefinitivo.agentesFuera.Count)
            {
                paramos = true;
            }

        }
        int total = 0;
        foreach (GameObject npc in agentes_entran)
        {
            total += 1;
        }

        if (agentes_entran.Count > agentes_salen.Count)
        {
            while (agentes_entran.Count != agentes_salen.Count)
            {
                agentes_entran.RemoveAt(0);
            }
        }

        Debug.Log("Vamos a sacar " + agentes_entran.Count + " personas con el temporizador");

        InvokeRepeating("SalirCada15", 15f, 15f);
    }


    void SalirCada15()
    {
        if (agentes_salen.Count != 0)
        {
            GameObject sale = agentes_salen[0];
            Vector3 posicion = sale.transform.position;
            GameObject entra = agentes_entran[0];

            NavMeshAgent agente = entra.GetComponent<NavMeshAgent>();
            agente.SetDestination(posicion);

            StartCoroutine(Salir(sale));

            agentes_salen.RemoveAt(0);
            agentes_entran.RemoveAt(0);
        }
        else
        {
            CancelInvoke("SalirCada15");
        }
    }

    private IEnumerator Salir(GameObject npc)
    {
        yield return new WaitForSeconds(5f);
        npc.AddComponent<MovimientoFuera>();
    }

    public void CambioHora()
    {
        agentes_entran.Clear();
        agentes_salen.Clear();
        CancelInvoke("SalirCada15");
    }

    public void AlarmaActivada()
    {

        CancelInvoke("SalirCada15");
        foreach (GameObject npc in agentes_entran)
        {
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            agent.ResetPath();
            MovimientoFuera mf = npc.GetComponent<MovimientoFuera>();
            if (mf == null)
            {
                npc.AddComponent<MovimientoFuera>();
            }
            else
            {
                Destroy(mf);
                npc.AddComponent<MovimientoFuera>();
            }
        }

        foreach (GameObject npc in agentes_salen)
        {
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            agent.ResetPath();
            MovimientoFuera mf = npc.GetComponent<MovimientoFuera>();
            if (mf == null)
            {
                npc.AddComponent<MovimientoFuera>();
            }
            else
            {
                Destroy(mf);
                npc.AddComponent<MovimientoFuera>();
            }
        }

        agentes_entran.Clear();
        agentes_salen.Clear();
    }
}
