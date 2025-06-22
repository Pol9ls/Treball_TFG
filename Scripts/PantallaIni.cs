using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PantallaIni : MonoBehaviour
{
    public TMP_Text lateral;
    public TMP_Text delantera;
    public TMP_Text ambas;
    public Button confirmar;
    public GameObject canvas;
    public int opcion;
    public GameObject canvasinfo;

    public GameObject puertadelantera;
    public GameObject puertalateral;

    void Start()
    {
        confirmar.interactable = false;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Opcion(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Opcion(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Opcion(3);
        }
    }

    void Opcion(int index)
    {
        lateral.fontStyle = FontStyles.Normal;
        delantera.fontStyle = FontStyles.Normal;
        ambas.fontStyle = FontStyles.Normal;

        if(index == 1)
        {
            lateral.fontStyle = FontStyles.Bold;
        }
        else if(index == 2)
        {
            delantera.fontStyle = FontStyles.Bold;
        }
        else
        {
            ambas.fontStyle = FontStyles.Bold;
        }

        confirmar.interactable = true;
        opcion = index;

    }

    public void Confirmar()
    {
        if (opcion == 1)
        {
            puertadelantera.SetActive(true);
            puertalateral.SetActive(false);
        }
        else if (opcion == 2)
        {
            puertadelantera.SetActive(false);
            puertalateral.SetActive(true);
        }
        else
        {
            puertadelantera.SetActive(false);
            puertalateral.SetActive(false);
        }
        canvas.SetActive(false);
        canvasinfo.SetActive(true);

        Time.timeScale = 1f;
    }
}
