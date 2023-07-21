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

    //Variable para almacenar el �ltimo Transform
    private Transform playerLastTransform;

    // Start is called before the first frame update
    void Start()
    {
        camaraPrincipal = transform.Find("CameraHolder").Find("PlayerCamera").GetComponent<ThirdPersonCam>();
        thirdPersonCamera = transform.Find("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
        combatCamera = transform.Find("CombatCamera").GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
