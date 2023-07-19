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
    public Transform combatLookAt;

    [Header("Tipos de Camaras")]
    public GameObject thirdPersonCam;
    public GameObject combatCam;

    //Velocidad de Rotacion
    public float rotationSpeed;

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
        //Si oprimimos el boton V
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Pasamos a TPose brevemente para hacer el cambio
            playerController.PasarATpose();

            //Alternamos el modo de camara
            if (camCurrentStyle == CameraStyle.Basic) SwitchCameraStyle(CameraStyle.Combat);
            else SwitchCameraStyle(CameraStyle.Basic);
        }

        //Calculamos la direccion FRONTAL en que deberá mirar el personaje (en base a la camara)
        //--> Los movimientos hacia los lados siempre corresponderan con la direccion de la camara
        Vector3 playerViewDirection =
            player.position -
            new Vector3(
                transform.position.x,
                player.position.y, //Usamos la altura del Player, en lugar de la de la camara
                transform.position.z
                );

        //Asignamnos la direccion frontal (normalizada) al Transform de Orientation
        playerOrientation.forward = playerViewDirection.normalized;

        if (camCurrentStyle == CameraStyle.Basic)
        {
            //Si el PLAYER NO ESTA HACIENDO WALL RUNNING, y no este Trepando
            if (!playerController.WallRunning || !playerController.Climbing)
            {
                //Capturamos los Inputs de Direccion
                float horizontalInput = 0;
                if (!playerController.WallRunning && !playerController.Climbing) horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

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

        else if (camCurrentStyle == CameraStyle.Combat)
        {
            //Calculamos la direccion FRONTAL en que base al punto de Combate
            Vector3 dirToCombatLookAt =
                combatLookAt.position -
                new Vector3(
                    transform.position.x,
                    combatLookAt.position.y, //Usamos la altura del Player, en lugar de la de la camara
                    transform.position.z
                    );

            //Asignamnos la Orientacion frontal (normalizada) al Transform del punto de Combate
            playerOrientation.forward = dirToCombatLookAt.normalized;

            //Asignamnos que el cuerpo del personaje apunte a la direccion de combate de Combate
            playerBody.forward = dirToCombatLookAt.normalized;
        }
    }

    //-----------------------------------------------------

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        //Iniciamos con todos los modos de camara desactivados
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        //Activamos el modo de entrada
        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);

        //Asignamos el nuevo modo como modo actual
        camCurrentStyle = newStyle;

    }
}
