using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoundsController : MonoBehaviour
{
    //Clips de Sonidos de Golpe
    [SerializeField] private AudioClip clipGolpe1;
    [SerializeField] private AudioClip clipGolpePotente;
    [SerializeField] private AudioClip clipGolpeSeco;

    //Referencia al AudioSource del Body
    private AudioSource mAudiosource;

    //----------------------------------------------------------

    private void OnEnable()
    {
        mAudiosource = GetComponent<AudioSource>();
    }

    //----------------------------------------------------------

    public void ReproducirGolpe()
    {
        mAudiosource.PlayOneShot(clipGolpe1, 0.75f);
    }

    public void ReproducirGolpeFuerte()
    {
        mAudiosource.PlayOneShot(clipGolpePotente, 0.7f);
    }

    public void ReproducirGolpeSeco()
    {
        mAudiosource.PlayOneShot(clipGolpeSeco, 0.7f);
    }

}
