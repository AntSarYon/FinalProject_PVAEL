using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStyle
{
    Basic,
    Combat
}

//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Referencias")]
    //Orientacion del Player
    [SerializeField] private Transform playerOrientation;

    //Transform y componentes del GO Player
    [SerializeField] private Transform player;
    private PlayerMovement playerController;
    private Rigidbody rbPlayer;

    //Transform del Cuerpo del Personaje
    [SerializeField] private Transform playerBody;
    private Animator playerAnimator;

    [Header("Alternando modos de camara")]
    private CameraStyle camCurrentStyle;
    public Transform CombatLookAt;

    [Header("Tipos de Camaras")]
    public GameObject thirdPersonCam;
    public GameObject combatCam;

    //Velocidad de Rotacion
    public float rotationSpeed;

    public Transform PlayerOrientation { get => playerOrientation; set => playerOrientation = value; }
    public Transform Player { get => player; set => player = value; }
    public Transform PlayerBody { get => playerBody; set => playerBody = value; }
    public CameraStyle CamCurrentStyle { get => camCurrentStyle; set => camCurrentStyle = value; }

    //-------------------------------------------------------------------------

    private void Start()
    {
        //Iniciamos con la camara en 3era persona
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(true);

        //Obtenemos el Controller, el RB, y el Animator del Player
        playerController = player.GetComponent<PlayerMovement>();
        rbPlayer = player.GetComponent<Rigidbody>();
        playerAnimator = playerBody.GetComponent<Animator>();

        //Hacemos que el Cursor no sea visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //-------------------------------------------------------------------------

    private void Update()
    {
        ControlarCambioDeCamara();

        //Calculamos la direccion FRONTAL del personaje (en base a la camara)
        //--> Los movimientos hacia los lados siempre corresponderan con la direccion de la camara

        ObtenerDireccionFrontalDeCamara();

        //Si el estilo de Camara actual es el básico...
        if (camCurrentStyle == CameraStyle.Basic)
        {
            //ROTAMOS la direccion frontal del Player --> Mirará siempre hacia la direccion de movimiento
            RotarFrenteHaciaMovimiento();
        }

        //Si el estilo de Camara actual es el de Comvbatye...
        else if (camCurrentStyle == CameraStyle.Combat)
        {
            //El personaje siempre mirará hacia el frente
            MantenerMirandoAlFrente();
        }
    }



    //--------------------------------------------------------------------------------------

    private void ControlarCambioDeCamara()
    {
        //Si oprimimos el boton T
        if (Input.GetKeyDown(KeyCode.V))
        {
            
        
        //Cambiamos el modo de camara
        if (camCurrentStyle == CameraStyle.Basic)
            {
                //Pasamos a TPose brevemente para hacer el cambio
                playerController.PasarAModoCombate();

                SwitchCameraStyle(CameraStyle.Combat);
            }
            else
            {
                playerController.PasarATpose();
                SwitchCameraStyle(CameraStyle.Basic);
            }

        }
    }

    //-----------------------------------------------------

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        //Iniciamos con todos los modos de camara desactivados
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        //Activamos el modo de entrada
        if (newStyle == CameraStyle.Basic)
        {
            thirdPersonCam.SetActive(true);
            playerController. CombatMode = false;
            playerAnimator.SetBool("CombatMode", false);

            GameManager.Instance.BattleMusic.Stop();
            GameManager.Instance.PacificMusic.Play();

        }

        if (newStyle == CameraStyle.Combat)
        {
            //Reiniciamos el Numero de Clicks para que no haya Bugs en los ataques
            SwordCombo swcScript = null;
            MeleeCombo mcScript= null;

            if (playerBody.TryGetComponent<SwordCombo>(out swcScript))
            {
                swcScript.CantidadClicks = 0;
            }

            else if (playerBody.TryGetComponent<MeleeCombo>(out mcScript))
            {
                mcScript.CantidadClicks = 0;
            }

            combatCam.SetActive(true);
            playerController.CombatMode = true;
            playerAnimator.SetBool("CombatMode", true);
        }


        //Asignamos el nuevo modo como modo actual
        camCurrentStyle = newStyle;

    }

    //--------------------------------------------------------------------------------------

    private void ObtenerDireccionFrontalDeCamara()
    {
        Vector3 playerViewDirection =
            player.position -
            new Vector3(
                transform.position.x,
                player.position.y, //Usamos la altura del Player, en lugar de la de la camara
                transform.position.z
                );

        //Asignamnos la direccion frontal (normalizada) al Transform de Orientation
        playerOrientation.forward = playerViewDirection.normalized;
    }

    //--------------------------------------------------------------------------------------

    private void RotarFrenteHaciaMovimiento()
    {
        //Si el PLAYER NO ESTA HACIENDO WALL RUNNING, o no esta Trepando
        if (!playerController.WallRunning || !playerController.Climbing)
        {
            //Capturamos los Inputs de Direccion
            float verticalInput = Input.GetAxis("Vertical");
            //En el caso del Input Horizontal, solo lo haremos cuando no se este haciendo WallRunning o escalando
            float horizontalInput = 0;
            if (!playerController.WallRunning && !playerController.Climbing) horizontalInput = Input.GetAxis("Horizontal");

            //Definimos la Direccion relativa de los Inputs (en base a la perspectiva de la camara)
            Vector3 inputDirection = playerOrientation.forward * verticalInput + playerOrientation.right * horizontalInput;

            //Si existe Input de Movimiento
            if (inputDirection != null)
            {
                //ROTAMOS la direccion frontal del Player --> Girará hacia la direccion de movimiento,
                //considerando la direccion de la camara
                playerBody.forward = Vector3.Lerp(
                    playerBody.forward,
                    inputDirection.normalized,
                    rotationSpeed * Time.deltaTime  // Girará con la velocidad de rotacion que le especifiquemos
                    );
            }
        }
    }

    //--------------------------------------------------------------------------------------
    
    private void MantenerMirandoAlFrente()
    {
        //Calculamos la direccion FRONTAL en que base al punto de Combate
        Vector3 dirToCombatLookAt =
            CombatLookAt.position -
            new Vector3(
                transform.position.x,
                CombatLookAt.position.y, //Usamos la altura del Player, en lugar de la de la camara
                transform.position.z
                );

        //Asignamnos la Orientacion frontal (normalizada) al Transform del punto de Combate
        playerOrientation.forward = dirToCombatLookAt.normalized;

        //Asignamnos que el cuerpo del personaje apunte a la direccion de combate de Combate
        playerBody.forward = dirToCombatLookAt.normalized;
    }
}
