using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Referencia a la Camara Principal
    [SerializeField] private ThirdPersonCam camaraPrincipal;

    //Referencia a las Camaras de cada tipo
    [SerializeField] private CinemachineFreeLook thirdPersonCamera;
    [SerializeField] private CinemachineFreeLook combatCamera;

    private AudioSource mAudioSource;
    [SerializeField] private AudioClip clipCambio;

    [SerializeField] private AudioSource battleMusic;

    public ThirdPersonCam CamaraPrincipal { get => camaraPrincipal; set => camaraPrincipal = value; }
    public AudioSource BattleMusic { get => battleMusic; set => battleMusic = value; }

    private void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();

        //Asignamos esta instancia del GameManager
        Instance = this;
        
    }    
}
