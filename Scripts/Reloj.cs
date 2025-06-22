using System;
using TMPro;
using UnityEngine;

public class Reloj : MonoBehaviour
{

    public TMP_Text reloj;
    public TMP_Text dia;
    public int duracion;
    private int duracion_segundos;
    private int hora = 0;
    private int diaSemana = 0;

    private bool alarma;

    private string[] semana = { "DILLUNS","DIMARTS","DIMERCRES","DIJOUS","DIVENDRES","DISSABTE","DIUMENGE"};

    public GenerarDefinitivo generarDefinitivo;

    void Start()
    {
        alarma = false;

        if(duracion != 0)
        {
            duracion_segundos = duracion * 60;
        }
        else
        {
            duracion_segundos = 60;
        }

        DateTime horaActual = DateTime.Now;

        hora = horaActual.Hour; 

        if (hora < 10)
        {
            string relojString = "0" + hora.ToString() + ":00";
            reloj.text = relojString;
        }
        else
        {
            string relojString = hora.ToString() + ":00";
            reloj.text = relojString;

        }

        diaSemana  = (int)DateTime.Now.DayOfWeek;


        if(diaSemana == 0)
        {
            diaSemana = 6;
        }
        else
        {
            diaSemana -= 1;
        }

        dia.text = semana[diaSemana];

        InvokeRepeating("Actualizar_Hora", duracion_segundos, duracion_segundos);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (hora == 23)
            {
                hora = 0;

                string relojString = "00:00";

                reloj.text = relojString;

                if (diaSemana == 6)
                {
                    diaSemana = 0;
                }
                else
                {
                    diaSemana += 1;
                }

                dia.text = semana[diaSemana];

            }
            else
            {
                hora += 1;

                if(hora < 10)
                {
                    string relojString = "0" + hora.ToString() + ":00";
                    reloj.text = relojString;
                }
                else
                {
                    string relojString = hora.ToString() + ":00";
                    reloj.text = relojString;

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (hora == 0)
            {
                hora = 23;

                string relojString = "23:00";

                reloj.text = relojString;

                if (diaSemana == 0)
                {
                    diaSemana = 6;
                }
                else
                {
                    diaSemana -= 1;
                }

                dia.text = semana[diaSemana];
            }
            else
            {
                hora -= 1;

                if (hora < 10)
                {
                    string relojString = "0" + hora.ToString() + ":00";
                    reloj.text = relojString;
                }
                else
                {
                    string relojString = hora.ToString() + ":00";
                    reloj.text = relojString;

                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (diaSemana == 0)
            {
                diaSemana = 6;
            }
            else
            {
                diaSemana -= 1;
            }

            dia.text = semana[diaSemana];
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (diaSemana == 6)
            {
                diaSemana = 0;
            }
            else
            {
                diaSemana += 1;
            }
            dia.text = semana[diaSemana];
        }

        CancelInvoke("Actualizar_Hora");
        InvokeRepeating("Actualizar_Hora", duracion_segundos, duracion_segundos);
    }

    void Actualizar_Hora()
    {
        
        if(alarma)
        {
            return;
        }

        if (hora == 23)
        {
            hora = 0;

            string relojString = "00:00";

            reloj.text = relojString;

            if (diaSemana == 6)
            {
                diaSemana = 0;
            }
            else
            {
                diaSemana += 1;
            }

            dia.text = semana[diaSemana];

        }
        else
        {
            hora += 1;

            if (hora < 10)
            {
                string relojString = "0" + hora.ToString() + ":00";
                reloj.text = relojString;
            }
            else
            {
                string relojString = hora.ToString() + ":00";
                reloj.text = relojString;

            }
        }
    }

    public void Alarma_Sonando(bool booleano)
    {
        if (booleano)
        {
            alarma = true;
        }
        else{
            alarma = false;
        }
        
    }
}
