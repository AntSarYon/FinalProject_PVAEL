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

    //Arrego de GO con perosnajes disponibles para jugar
    public GameObject[] arrPersonajes = new GameObject[2];
    private int indicePersonaje;

    //Variable para almacenar el último
    private GameObject ultimoPersonaje;
    private Transform playerLastTransform;

    //private AudioSource mAudioSource;
    [SerializeField] private AudioClip clipCambio;

    public GameObject UltimoPersonaje { get => ultimoPersonaje; set => ultimoPersonaje = value; }
    public ThirdPersonCam CamaraPrincipal { get => camaraPrincipal; set => camaraPrincipal = value; }

    private void Awake()
    {
        //mAudioSource = GetComponent<AudioSource>();
        playerLastTransform = null;

        //Asignamos esta instancia del GameManager
        Instance = this;

        //El indice empieza en 0 -> apuntando al MotionMan
        indicePersonaje = 1;
        
    }

    //---------------------------------------------------------------------------------------
    void Start()
    {
        ObtenerPersonajesJugables();

        ObtenerCamarasDeEscena();

        AsignarPersonajeACamara();

        ActivarSoloPersonajeJugable();

        
    }

    //---------------------------------------------------------------------------------------

    private void ObtenerCamarasDeEscena()
    {
        CamaraPrincipal = GameObject.Find("PlayerCamera").GetComponent<ThirdPersonCam>();
        thirdPersonCamera = CamaraPrincipal.thirdPersonCam.GetComponent<CinemachineFreeLook>();
        combatCamera = CamaraPrincipal.combatCam.GetComponent<CinemachineFreeLook>();
    }

    //---------------------------------------------------------------------------------------

    private void ObtenerPersonajesJugables()
    {
        //Agregamos los Perosnajes jugables
        arrPersonajes[0] = GameObject.Find("Player");
        arrPersonajes[1] = GameObject.Find("PlayerSwordMan");
    }

    //-------------------------------------------------------------

    private void ActivarSoloPersonajeJugable()
    {
        for (int i = 0; i< arrPersonajes.Length; i++)
        {
            if (i == indicePersonaje)
            {
                //Activamos el perosnaje
                arrPersonajes[i].gameObject.SetActive(true);

                //Lo asignamos como ultimo personaje
                ultimoPersonaje = arrPersonajes[i].gameObject;

                //i estamos cambnianod de perosnaje (No es el inicio del jeuog)
                if (playerLastTransform != null)
                {
                    //Asignamos el Transform
                    ultimoPersonaje.transform.position = playerLastTransform.position;
                    ultimoPersonaje.transform.rotation = playerLastTransform.rotation;
                    ultimoPersonaje.transform.localScale = playerLastTransform.localScale;

                    //Disparamos el Trigger de TPose
                    ultimoPersonaje.transform.Find("PlayerBody").GetComponent<Animator>().SetTrigger("TPose");
                }
                if (camaraPrincipal.CamCurrentStyle == CameraStyle.Combat)
                {
                    ultimoPersonaje.GetComponent<PlayerMovement>().CombatMode = true;
                }
                else
                {
                    ultimoPersonaje.GetComponent<PlayerMovement>().CombatMode = false;
                }
            }
            else
            {
                //Desactivamos el personaje
                arrPersonajes[i].gameObject.SetActive(false);
            }
        }
    }
    //------------------------------------------------------------------------------
    private void AsignarPersonajeACamara()
    {
        CamaraPrincipal.PlayerOrientation = arrPersonajes[indicePersonaje].transform.transform.Find("Orientation");
        CamaraPrincipal.Player = arrPersonajes[indicePersonaje].transform;
        CamaraPrincipal.PlayerBody = arrPersonajes[indicePersonaje].transform.Find("PlayerBody");
        CamaraPrincipal.CombatLookAt = arrPersonajes[indicePersonaje].transform.Find("Orientation").Find("CombatLookAt");

        thirdPersonCamera.Follow = arrPersonajes[indicePersonaje].transform;
        thirdPersonCamera.LookAt = arrPersonajes[indicePersonaje].transform;

        combatCamera.Follow = arrPersonajes[indicePersonaje].transform;
        combatCamera.LookAt = arrPersonajes[indicePersonaje].transform.Find("Orientation").Find("CombatLookAt");
    }
    //------------------------------------------------------------------------------
    private void ControlarCambioDePersonaje()
    {
        //Almacenamos el Transform dle perosnaje que va a salir
        playerLastTransform = ultimoPersonaje.transform;

        //controlamos el indice
        if (indicePersonaje == 0)
        {
            indicePersonaje = 1;
        }
        else indicePersonaje = 0;

        ActivarSoloPersonajeJugable();
        AsignarPersonajeACamara();
    }
    //------------------------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ControlarCambioDePersonaje();
        }
    }
}
