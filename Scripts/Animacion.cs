using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Animacion : MonoBehaviour
{
    private Animator animator;
    private string[] animacionesWalk;
    private string[] animacionesIdle;
    private int index;

    private Vector3 posicion_anterior;
    private bool idle = false;
    private bool ini = false;

    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponent<Animator>();

        animacionesWalk = new string[] { "Walking", "Female Tough Walk", "Happy Walk", "Strut Walking", "Standard Walk" };
        animacionesIdle = new string[] { "Happy Idle", "Dwarf Idle", "Offensive Idle", "Sad Idle", "Idle", "Bored", "Shrugging", "Shaking Head No", "Cocky Head Turn", "Head Nod Yes", "Neck Stretching", "Arm Gesture", "Looking Down"};

        Invoke(nameof(Iniciar), 0.5f);

    }

    void Update()
    {
        if (ini)
        {
            bool seMueve = EstaMoviendose();

            if (seMueve && idle)
            {
                AnimacionWalk();
                idle = false;
            }
            else if (!seMueve && !idle)
            {
                AnimacionIdle();
                idle = true;
            }
        } 

    }

    void Iniciar()
    {
        ini = true;

        if (EstaMoviendose())
        {
            AnimacionWalk();
            idle = false;
        }
        else
        {
            AnimacionIdle();
            idle = true;
        }
    }

    void AnimacionWalk()
    {
        int index = UnityEngine.Random.Range(0, animacionesWalk.Length);
        string animacion = animacionesWalk[index];

        if (animator.HasState(0, Animator.StringToHash(animacion)))
        {
            StartCoroutine(PlayAnimacion(animacion));
        }
        else
        {
            Debug.Log("Animación walk no encontrada" + animacion);
        }
    }

    void AnimacionIdle()
    {
        int index = UnityEngine.Random.Range(0, animacionesIdle.Length);
        string animacion = animacionesIdle[index];

        if (animator.HasState(0, Animator.StringToHash(animacion)))
        {
            StartCoroutine(PlayAnimacion(animacion));
        }
        else
        {
            Debug.Log("Animación idle no encontrada" + animacion);
        }
    }


    bool EstaMoviendose()
    {
        return agente.velocity.sqrMagnitude > 0.01f && !agente.pathPending;
    }

    IEnumerator PlayAnimacion(string animacion)
    {
        float retraso = UnityEngine.Random.Range(0f, 0.3f);
        yield return new WaitForSeconds(retraso);

        float tiempoAnimacion = UnityEngine.Random.Range(0f, 1f); 
        animator.Play(animacion, 0, tiempoAnimacion);
    }

}
