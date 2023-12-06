using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
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
    }
}
