using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject imagenMotionMan;
    [SerializeField] private GameObject imagenSwordMan;

    [SerializeField] private GameObject modoRecorrido;
    [SerializeField] private GameObject modoCombate;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CamaraPrincipal.CamCurrentStyle == CameraStyle.Combat)
        {
            modoRecorrido.SetActive(false);
            modoCombate.SetActive(true);
        }
        else if (GameManager.Instance.CamaraPrincipal.CamCurrentStyle == CameraStyle.Basic)
        {
            modoRecorrido.SetActive(true);
            modoCombate.SetActive(false);
        }

        if (GameManager.Instance.IndicePersonaje == 0)
        {
            imagenMotionMan.SetActive(true);
            imagenSwordMan.SetActive(false);
        }
        else if (GameManager.Instance.IndicePersonaje == 1)
        {
            imagenMotionMan.SetActive(false);
            imagenSwordMan.SetActive(true);
        }
    }
}
