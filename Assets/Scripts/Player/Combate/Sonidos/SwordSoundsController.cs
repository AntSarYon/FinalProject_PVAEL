using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSoundsController : MonoBehaviour
{
    //Clips de Sonidos de Golpe
    [SerializeField] private AudioClip clipCorte;
    [SerializeField] private AudioClip clipAgitandoSuave;
    [SerializeField] private AudioClip clipAgitando;
    [SerializeField] private AudioClip clipAgitandoFuerte;
    [SerializeField] private AudioClip clipGolpeSeco;


    //Referencia al AudioSource del Body
    private AudioSource mAudiosource;

    //----------------------------------------------------------

    private void OnEnable()
    {
        mAudiosource = GetComponent<AudioSource>();
    }

    //----------------------------------------------------------

    public void ReproducirCorte()
    {
        mAudiosource.PlayOneShot(clipCorte, 0.75f);
    }

    public void ReproducirAgiteeFuerte()
    {
        mAudiosource.PlayOneShot(clipAgitandoFuerte, 1);
    }

    public void ReproducirAgiteSuave()
    {
        mAudiosource.PlayOneShot(clipAgitandoSuave, 1);
    }

    public void ReproducirAgite()
    {
        mAudiosource.PlayOneShot(clipAgitando, 1);
    }
    public void ReproducirGolpeSeco()
    {
        mAudiosource.PlayOneShot(clipGolpeSeco, 0.7f);
    }
}
