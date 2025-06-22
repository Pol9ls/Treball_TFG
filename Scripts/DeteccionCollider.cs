using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeteccionCollider : MonoBehaviour
{
    public List<GameObject> agentesDentro = new List<GameObject>();
    public List<GameObject> agentesFuera = new List<GameObject>();

    public GenerarDefinitivo generarDefinitivo;
    public MoverDefinitivo moverDefinitivo;
    public float esperadas =110;
    public float reales=0;

    public PruebaTemporizador pruebaTemporizador;

    private void OnTriggerEnter(Collider other)
    {

            GameObject npc = other.gameObject;
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            
            if(generarDefinitivo.agentesDentro.Count == 110)
            {
                Vector3 destino = new Vector3(102, 0, 73);
                agent.SetDestination(destino);
                return;
            }

            if (!generarDefinitivo.agentesDentro.Contains(npc))
            {
                generarDefinitivo.agentesDentro.Add(npc);
                EstadoNPC estado = other.GetComponent<EstadoNPC>();
                if (estado != null)
                {
                    estado.entrando = false;
                    estado.saliendo = false;
                    estado.estabadentro = true;
                }

                if (estado.empiezafuera)
                {
                    estado.empiezafuera = false;
                }
                
                if (generarDefinitivo.agentesFuera.Contains(npc))
                {
                    generarDefinitivo.agentesFuera.Remove(npc);
                }

                MovimientoFuera mf = npc.GetComponent<MovimientoFuera>();
                if (mf != null)
                {
                    Destroy(mf);
                }
            }
        
    }

    private void OnTriggerExit(Collider other)
    {
            GameObject npc = other.gameObject;

            EstadoNPC estado = other.GetComponent<EstadoNPC>();
            if (estado != null)
            {
                MovimientoFuera mf = npc.GetComponent<MovimientoFuera>();
                if(mf == null)
                {
                    if (estado.estabadentro)
                    {

                        estado.entrando = false;
                        estado.saliendo = true;
                        estado.estabadentro = false;
                    }

                }
            }

            if (generarDefinitivo.agentesDentro.Contains(npc))
            {
                generarDefinitivo.agentesDentro.Remove(npc);
                if (!generarDefinitivo.agentesFuera.Contains(npc))
                {
                    generarDefinitivo.agentesFuera.Add(npc);
                }
            }
    }

    public void Actualizar_Personas(float personas_esperadas, float personas_reales)
    {
        esperadas = personas_esperadas;
        reales = personas_reales;
    }

}
