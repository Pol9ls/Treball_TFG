using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Alarma : MonoBehaviour
{
    public AudioSource alarm;

    private bool lleno = false;

    public TMP_Text tiempo;

    public Reloj reloj;

    private int segundos = 0;

    public MoverDefinitivo mover;

    public GameObject puertadelantera;
    public GameObject puertalateral;

    private Vector3 puerta1 = new Vector3(0,0,35);
    private Vector3 puerta2 = new Vector3(60, 0, 9);

    private int dentro;

    public DeteccionCollider deteccionCollider;

    public GenerarDefinitivo generarDefinitivo;

    public PruebaTemporizador pruebaTemporizador;


    private List<Vector3> puntosDestino = new List<Vector3>();

    public bool sonando = false;

    public bool puertalateralcercana;

    void Start()
    {
        dentro = 0;
        sonando = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !alarm.isPlaying)
        {

            alarm.loop = true;
            alarm.Play();

            Vaciar_Local_Collider();

            Vaciar_Entrando();

            pruebaTemporizador.AlarmaActivada();

            sonando = true;

            if (reloj != null)
            {
                reloj.Alarma_Sonando(true);
            }

            StartCoroutine(Comprovar());
        }
    }

    void Vaciar_Local_Collider()
    {
        foreach (GameObject npc in generarDefinitivo.agentesDentro)
        {
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                dentro += 1;
                if (puertadelantera.activeSelf == false && puertalateral.activeSelf == false)
                {
                    float distancia1 = Vector3.Distance(npc.transform.position, puerta1);
                    float distancia2 = Vector3.Distance(npc.transform.position, puerta2);


                    if (distancia1 > distancia2)
                    {
                        agent.SetDestination(puerta2);
                        npc.gameObject.AddComponent<SalidaFuera>();

                    }
                    else
                    {
                        agent.SetDestination(puerta1);
                        npc.gameObject.AddComponent<SalidaFuera>();
                    }
                }
                else if (puertadelantera.activeSelf == false && puertalateral.activeSelf == true)
                {
                    agent.SetDestination(puerta1);
                    npc.gameObject.AddComponent<SalidaFuera>();


                }
                else if (puertadelantera.activeSelf == true && puertalateral.activeSelf == false)
                {
                    agent.SetDestination(puerta2);
                    npc.gameObject.AddComponent<SalidaFuera>();

                }
            }
        }
    }

    IEnumerator Comprovar()
    {
        lleno = true;

        while (lleno)
        {
            segundos += 1;

            tiempo.text = "Temps de desallotjament: " + segundos.ToString() + "s";

            int numDentro = ContarNPCs();

            if (generarDefinitivo.agentesDentro.Count < 10)
            {
                foreach (GameObject npc in generarDefinitivo.agentesDentro)
                {
                    NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

                    agent.SetDestination(puerta1);
                }
            }

            if (numDentro == 0)
            {
                alarm.Stop();
                lleno = false;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }

    }

    int ContarNPCs()
    {
        int agentes_dentro = generarDefinitivo.agentesDentro.Count;
        return agentes_dentro;
    }

    void Vaciar_Entrando()
    {
        foreach (GameObject npc in generarDefinitivo.agentesFuera)
        {
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

            MovimientoFuera mf = npc.GetComponent<MovimientoFuera>();
            if(mf != null)
            {
                Destroy(mf);
            }

            agent.ResetPath();
            npc.AddComponent<MovimientoFuera>();
        }
    }
}
