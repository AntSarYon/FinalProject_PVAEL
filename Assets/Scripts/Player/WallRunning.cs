using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Capas para el WallRunning")]
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private LayerMask capaMuros;

    [Header("Camara")]
    [SerializeField] private ThirdPersonCam camara;

    [SerializeField] private float wallRunForce;
    [SerializeField] private float maxWallRunTime;
    private float wallRunTimer;

    [SerializeField] private float wallClimbSpeed;

    [Header("Salto de Pared")]
    [SerializeField] private float wallJumpUpForce;
    [SerializeField] private float wallJumpSideForce;

    [Header("Gravedad")]
    public bool useGravity;
    public float gravityCounterFource;
    
    [Header("Detección")]
    private float wallCheckDistance = 0.65f;
    private float minJumpHeight = 0.7f;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool wallLeft;
    private bool wallRight;

    [Header("SALIENDO DE PARED")]
    private bool exitingWall;
    [SerializeField] private float exitWallTime;
    private float exitWallTIMER;

    [Header("Referencias")]
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform playerBody;
    private PlayerMovement playerController;
    private Rigidbody mRb;
    private Animator bodyAnimator;

    //-----------------------------------------------------------
    private void Awake()
    {
        //Obtenemos referencia a Componentes
        playerController = GetComponent<PlayerMovement>();
        mRb = GetComponent<Rigidbody>();
    }

    //------------------------------------------------------------------

    private void Start()
    {
        
        bodyAnimator = playerBody.GetComponent<Animator>();
    }

    //-------------------------------------------------------------

    private void RevisarParedes()
    {
        //Lanzamos Raycasta hacia ambos lados para detectar si hay Paredes cerca
        //En caso de que las haya, almacenamos la información en un Hit.
        wallRight = Physics.Raycast(playerOrientation.position, playerOrientation.right, out rightWallHit, wallCheckDistance, capaMuros);
        wallLeft = Physics.Raycast(playerOrientation.position, -playerOrientation.right, out leftWallHit, wallCheckDistance, capaMuros);

    }

    //--------------------------------------------------------------------------------

    private bool RevisarDistanciaDelSuelo()
    {
        //Mediante un Raycast, detectamos si estamos lo suficientemence por encima del suelo
        //Considerar que, queremos que nos devuelva Falso, si está muy cerca del suelo (hace contacto)
        return !Physics.Raycast(playerBody.position, Vector3.down, minJumpHeight, capaSuelo);
    }

    //-----------------------------------------------------------------------------------

    private void Update()
    {
        //Si no estamos en Modo Combate
        if (!playerController.CombatMode)
        {
            RevisarParedes();

            StateMachine();
        }
    }

    //-----------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Si el jugador esta haciendo Wall Running...
        if (playerController.WallRunning)
        {
            //Aplicamos la Fuyerza de movimiento en Pared
            WallRunningMovement();
        }
    }

    //-----------------------------------------------------------------------------------------

    private void StateMachine()
    {
        //Entrar a ESTADO 1 - WALL RUN

        //Si hay una pared cerca,
        //y nos estamos moviendo hacia adelante,
        //Y estams a una distancia elevada del suelo
        //Y no estamos saliendo (saltando) de la pared
        if ((wallLeft || wallRight) && playerController.InputVertical > 0 && RevisarDistanciaDelSuelo() && !exitingWall)
        {
            //Si el jugador NO ESTA HACIENDO WALL RUNNING
            if (!playerController.WallRunning)
            {
                //Iniciamos el Wall Running
                StartWallRun();
            }

            //Si el Timer de WallRun tiene tiempo
            if (wallRunTimer > 0)
            {
                //Reducimos el Timer
                wallRunTimer -= Time.deltaTime;
            }
            //Si el Timer se quedó sin tiempo, y seguimos pegados a la pared
            if (wallRunTimer <= 0 && playerController.WallRunning)
            {
                //Activamos el Flag para salir de la Pared
                //exitingWall = true;
                //exitWallTIMER = exitWallTime;

                FallingFromWall();
            }

            //Si el jugador pulsa espacio
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Disparamos el Trigger de animacion de Salto
                bodyAnimator.SetTrigger("Jump");
                //Hacemos un salto
                WallJump();
            }
        }

        //Entrar a Estado 2 -- Saliendo de Pared (por Salto)
        else if (exitingWall)
        {
            if (playerController.WallRunning)
            {
                StopWallRun();
            }
            if (exitWallTIMER > 0)
            {
                exitWallTIMER -= Time.deltaTime;
            }
            if (exitWallTIMER <= 0)
            {
                exitingWall = false;
            }
        }

        //Entrar a Estado 3 -- Normal RUN
        //Caso contrario
        else
        {
            //Si el jugador se encontraba haciendo Wall Running...
            if (playerController.WallRunning)
            {
                //Detenemos el Wall Run
                StopWallRun();
            }
        }

    }

    //--------------------------------------------------------------------------------------------

    private void StartWallRun()
    {
        //Activamos el Flag de Wall Running en el PlayerController
        playerController.WallRunning = true;

        //CONTROLAMOS UN TIEMPO MAXIMO PARA EL WALL RUNING

        //Asignamos el Tiempo completo al Timer
        wallRunTimer = maxWallRunTime;

        //Quitamos toda velocidad en el Eje Y
        mRb.velocity = new Vector3(mRb.velocity.x, 0, mRb.velocity.z);

        //Activamos el Flag de WallRun respectivo
        if (wallLeft) bodyAnimator.SetBool("LeftWallRun", true);
        if (wallRight) bodyAnimator.SetBool("RightWallRun", true);

        //Ajustamos la camara para dar el Efecto de Inclinacion
        //camara.DoFov(75f);

        //if (wallLeft) camara.DoTilt(-5f);
        //if (wallRight) camara.DoTilt(5f);
    }

    //------------------------------------------------------------------------

    private void WallRunningMovement()
    {
        //Desactivamos la gravedad
        mRb.useGravity = useGravity;



        //Obtenemos la normal de la Pared con la que entramos en contacto
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        //Obtenemos la dirección en que aplicar fuerza para mantenernos pegados a la pared
        Vector3 wallForward = Vector3.Cross(wallNormal, playerOrientation.up);

        //Controlamos la dirección de la Fuerza que nos brinda la pared; sino se hace esto,
        //dependiendo de desde donde toquemos la pared, nos botará hacia adelante o atrás
        if ((playerOrientation.forward - wallForward).magnitude > (playerOrientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //Aplicamos la Fuerza que nos brinda la pared para avanzar pegados a ella
        mRb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //Fuerza de escalada o descenso
        if (Input.GetKey(KeyCode.LeftShift))
        {
            mRb.velocity = new Vector3(mRb.velocity.x, wallClimbSpeed, mRb.velocity.z);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            mRb.velocity = new Vector3(mRb.velocity.x, -wallClimbSpeed, mRb.velocity.z);
        }

        //Aplicamos fuerza para mantenernos pegados a la pared
        //Esto es SOLO si el jugador lo demanda oprimiendo las teclas <- o ->

        if (!(wallLeft && playerController.InputHorizontal > 0) && !(wallRight && playerController.InputHorizontal < 0))
        {
            mRb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        //Debilitar la Gravedad
        mRb.AddForce(transform.up * gravityCounterFource, ForceMode.Force);

    }

    //------------------------------------------------------------------------------------------------

    private void StopWallRun()
    {
        //Desactivamos el Flag de WallRunning del Player
        playerController.WallRunning = false;

        //Desactivamos los Flag de Animacion para el WallRunning
        bodyAnimator.SetBool("LeftWallRun", false);
        bodyAnimator.SetBool("RightWallRun", false);

        //Activamos la gravedad nuevamente
        mRb.useGravity = true;

        //Reseteamos las propiedades de la camara
        //camara.DoFov(60f);
        //camara.DoTilt(0f);


    }

    //-----------------------------------------------------------------------------------------

    private void WallJump()
    {
        //Entramos al estado de SALIENDO DE PARED
        exitingWall = true;
        exitWallTIMER = exitWallTime;

        //Obtenemos la normal de la Pared con la que entramos en contacto
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        //Calculamos la fuerza a aplicar
        Vector3 fuerzaAAplicar = (transform.up * wallJumpUpForce) + (wallNormal * wallJumpSideForce);

        //Reiniciamos la velocidad vertical a 0
        mRb.velocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

        //Añadimos fuerza como un impulso
        mRb.AddForce(fuerzaAAplicar, ForceMode.Impulse);
    }

    private void FallingFromWall()
    {
        //Entramos al estado de SALIENDO DE PARED
        exitingWall = true;
        exitWallTIMER = exitWallTime;

        //Obtenemos la normal de la Pared con la que entramos en contacto
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        //Calculamos la fuerza a aplicar
        Vector3 fuerzaAAplicar = (transform.up * 2) + (wallNormal * 4);

        //Reiniciamos la velocidad vertical a 0
        mRb.velocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

        //Añadimos fuerza como un impulso
        mRb.AddForce(fuerzaAAplicar, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(playerOrientation.position, playerOrientation.right * wallCheckDistance);
        Gizmos.DrawRay(playerOrientation.position, -playerOrientation.right * wallCheckDistance);
    }
}
