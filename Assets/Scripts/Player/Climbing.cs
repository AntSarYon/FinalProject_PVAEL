using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool climbing;

    [Header("Deteccion de Pared")]
    [SerializeField] private float detectionLength;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

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
        //Invocamos a la revision de Pared
        WallCheck();

        //Invocamos a laMaquina de control de Estados
        StateMachine();

        //Si el Estado CLIMBING se encuentra activo...
        if (climbing) ClimbingMovement(); // --> Hacemos el movimiento de trepar
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

        //Si estamos en el suelo...
        if (playerController.Grounded)
        {
            //Reiniciamos el Timer para controlar el tiempo de escalamiento disponible
            climbTimer = maxClimbTime;
        }
    }

    //----------------------------------------------------------------------

    private void StateMachine()
    {
        //ESTADO 1 - TREPANDO

        //Si hay una pared al frente perfectamente posicionada, y estamos moviendonos hacia el frente...
        if (wallFront && playerController.InputVertical > 0 && wallLookAngle < maxWallLookAngle)
        {
            //Si no estamos trepando, y el Timer indica que hay tiempo de Escalada disponible
            if (!climbing && climbTimer > 0) StartClimbing(); //Empezamos a trepar

            //De ser posible, reducimos el Timer cada frame
            if (climbTimer > 0) climbTimer-= Time.deltaTime;

            if (climbTimer <= 0) StopClimbing(); //Dejamos de Trepar
        }

        //ESTADO 2 - NO TREPANDO - DEJANDO DE TREPAR
        
        //Si las condiciones para escalar no se cumplen; dejamos de trepar.
        else 
        {
            //Si estabamos trepando
            if (climbing) StopClimbing();
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

        bodyAnimator.SetBool("Climbing", false);
    }


}
