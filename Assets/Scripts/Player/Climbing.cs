using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Climbing : MonoBehaviour
{
    [Header("Referencias")]
    private PlayerMovement playerController;
    [SerializeField] private Transform playerOrientation;
    [SerializeField] private Transform playerBody;
    private Animator bodyAnimator;
    private Rigidbody mRb;
    [SerializeField] private LayerMask wallLayer;

    [Header("Trepar")]
    [SerializeField] private float climbSpeed;
    [SerializeField] private float maxClimbTime;
    private float climbTimer;

    //FLAG
    private bool climbing;

    [Header("ClimbJumping")]
    [SerializeField] private float climbJumpUpForce;
    [SerializeField] private float climbJumpBackForce;

    [SerializeField] private int climbJumps;
    private int climbJumpsLeft;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    [SerializeField] private float minWallNormalAngleChange;

    [Header("Deteccion de Pared")]
    [SerializeField] private float detectionLength;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    [Header("Saliendo del Muro")]
    [SerializeField] private float exitWallTime;
    private float exitWallTimer;

    //FLAG
    private bool exitingWall;

    public bool ExitingWall { get => exitingWall; set => exitingWall = value; }

    //------------------------------------------------------------------

    private void Awake()
    {
        //Obtenemos referencia a Componentes
        playerController = GetComponent<PlayerMovement>();
        mRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bodyAnimator = playerBody.GetComponent<Animator>();
    }

    //------------------------------------------------------------------

    private void Update()
    {
        //SI NO ESTAMOS EN MODO COMBATE
        if (!playerController.CombatMode)
        {
            //Invocamos a la revision de Pared
            WallCheck();

            //Invocamos a laMaquina de control de Estados
            StateMachine();

            //Si el Estado CLIMBING se encuentra activo, y NO estamos saliendo de la pared
            if (climbing && !exitingWall) ClimbingMovement(); // --> Hacemos el movimiento de trepar
        }
        
    }

    //-------------------------------------------------------------------
    //Funcion para comprobar si hay una pared al frente
    private void WallCheck()
    {
        //Obtenemos Flag de Pared al frente dependiendo de si detectamos una pared al frente, o no
        wallFront = Physics.SphereCast(
            transform.position, 
            sphereCastRadius, 
            playerOrientation.forward, 
            out frontWallHit, 
            detectionLength, 
            wallLayer
            );

        //Controlaremos que el Jugador esté mirando directamente hacia la pared; si esta ligeramente girado
        //hacia tra direccion, la Escalada no ocurrirá --> SIN ESTO; TODO SERIA UN DESASTRE

        //Obtenemos el angulo de la Pared respecto a nuestra orientacion
        wallLookAngle = Vector3.Angle(playerOrientation.forward, -frontWallHit.normal);

        //Inicializamos un Booleano para saber si hemos entrado en contacto con una nueva Pared
        //Para saber esto, simplemente comparamos el ANgulo de la NORMAL de la ultima Pared con la de la Nueva
        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        //Si entramos en contacto con una nueva pared, o estamos en el suelo...
        if ((wallFront && newWall) || playerController.Grounded)
        {
            //Reiniciamos el Timer para controlar el tiempo de escalamiento disponible
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    //----------------------------------------------------------------------

    private void StateMachine()
    {
        //ESTADO 1 - TREPANDO

        //Si hay una pared al frente perfectamente posicionada, y estamos moviendonos hacia el frente, y NO Estamos saliendo de la pared
        if (wallFront && playerController.InputVertical > 0 && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            //Si no estamos trepando, y el Timer indica que hay tiempo de Escalada disponibl
            if (!climbing && climbTimer > 0)
            {
                StartClimbing(); //Empezamos a trepar
            }


            //De ser posible, reducimos el Timer cada frame
            if (climbTimer > 0) climbTimer -= Time.deltaTime;

            if (climbTimer <= 0)
            {
                //Disparamos el Trigger de Salto de Pared
                bodyAnimator.SetTrigger("ClimbJump");

                StopClimbing(); //Dejamos de Trepar
            }
        }

        //ESTADO 2 - SALIENDO DE PARED

        //Si el Flag de Saliendo de Pared esta activo
        else if (exitingWall) 
        {
            //Si estabamos trepando
            if (climbing)
            {
                //Dejamos de Trepar
                StopClimbing();
            }

            //CONTROLAMOS EL TIEMPO LIMITE PARA SALIR DE LA PARED

            //Si el Timer de Salida es mayor a 0
            if (exitWallTimer > 0)
            {
                //Lo reducimos
                exitWallTimer -= Time.deltaTime;
            }
            //Si el Timer de Salida ya llego a 0
            if (exitWallTimer <= 0)
            {
                //Desactivamos el Flag de Saliendo de Pared
                exitingWall = false;
            }
        }

        //ESTADO 3 - NO TREPANDO - DEJANDO DE TREPAR

        //Si las condiciones para escalar no se cumplen; dejamos de trepar.
        else
        {
            //Si estabamos trepando
            if (climbing)
            {
                //Disparamos el Trigger de Salto de Pared
                //bodyAnimator.SetTrigger("ClimbJump");

                StopClimbing();
            }
        }

        //ESTADO 4 - SALTANDO DE PARED 

        //Si hay una pred al frente, y el jugador pulsa espacio, y nos quedan mas saltos...
        if (wallFront && Input.GetKeyDown(KeyCode.Space) && climbJumpsLeft > 0)
        {
            //Hacemos el Salto en Pared
            ClimbJump();
        }
    }

    //--------------------------------------------------------------------
    //FUNCION PARA EMPEZAR A TREPAR
    private void StartClimbing()
    {
        //Activamos el Flag de TREPANDO, en ambos Scripts
        climbing = true;
        playerController.Climbing = true;

        bodyAnimator.SetBool("Climbing", true);

        //Controlamos la deteccion de nuevas paredes si es que se hizo un SALTO antes

        //Almacenamos la informacion de la ultima pared, principalmente su NORMAL
        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

    }

    //--------------------------------------------------------------------
    //FUNCION PARA EL ESTADO DE TREPAR

    private void ClimbingMovement()
    {
        //NOTA: En este caso no es necesario utilizar ninguna FUERZA; sino modificar directamente la VELOCIDAD

        //Modificamos la velocidad Vertical del Player
        mRb.velocity = new Vector3(
            mRb.velocity.x, 
            climbSpeed, // <-- Solo seteamos la velocidad en Y
            mRb.velocity.z
            );
    }

    //--------------------------------------------------------------------
    //FUNCION PARA DEJAR DE TREPAR

    private void StopClimbing()
    {
        //Desactivamos el Flag de TREPANDO
        climbing = false;
        playerController.Climbing = false;

        //Desactivamos la animcacion de CLIMBING tras un breve delay
        //Esto permitirá pasar de Climbing a WalRunning directamente
        Invoke(nameof(DescativarAnimacionDeClimbing), 0.30f);
        
    }

    //---------------------------------------------------------------

    private void DescativarAnimacionDeClimbing()
    {
        bodyAnimator.SetBool("Climbing", false);
    }

    //------------------------------------------------------
    private void ClimbJump()
    {
        //Activamos el Flag de Saliendo de Pared
        exitingWall = true;

        //Iniciamos el Timer
        exitWallTimer = exitWallTime;

        //Disparamos el Trigger de Salto de Pared
        bodyAnimator.SetTrigger("ClimbJump");

        //Saltamos tras un breve Delay
        Invoke(nameof(Saltar), 0.5f);

        //Disminuimos la cantidad de Saltos disponibles
        climbJumpsLeft--;
    }
    
    //- - - - - - - - - - - - - - - - - - - -

    private void Saltar()
    {
        //Calculamos la fuerza a Aplicar en base a la fuerza hacia arriba y la normal de la pared
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        //Reseteamos la velocidad en Y
        mRb.velocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

        //aplicamos la fuerza a modo de IMPULSO
        mRb.AddForce(forceToApply, ForceMode.Impulse);
    }


}
