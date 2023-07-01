using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    walking,
    sprinting,
    inAir,
    crouching,
    wallRunning,
    sliding,
    freeze

};

public class PlayerMovement : MonoBehaviour
{
    //Direccion de Input
    private Vector3 mDirection;

    //Capturamos ls inputs de Direccion
    private float inputVertical;
    private float inputHorizontal;

    //Variable para manejar el Estado de Movimiento
    private MovementState state;

    [Header("Movimiento")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float slideSpeed;

    [Header("Deslizamiento")]
    //Variables para almacenar las proximas velocidades de movimiento deseadas
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private bool sliding;

    [Header("Salto")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    //Flag de ListoParaSaltar, inicializado en TRUE
    private bool readyToJump = true;

    [Header("Agachado")]
    private float crouchCCRadius = 0.6363066f;
    private float crouchCCHeight = 1.372868f;
    private Vector3 crouchCCCenter = new Vector3(0f, 0.670045912f, 0.276306629f);

    private float originalCCRadius;
    private float originalCCHeight;
    private Vector3 originalCCCenter;

    [Header("Comprobar Suelo")]
    //Friccion del suelo
    [SerializeField] private float groundDrag;
    //Distancia de deteccion del sueli
    private float groundDistance = 0.25f;
    //Capa del Suelo a comprobar
    public LayerMask groundMask;
    //Flag para saber si esta, o no, en el suelo
    private bool grounded;

    [Header("Comprobar Pendientes")]
    [SerializeField] private float maximoAnguloDePendiente;
    private RaycastHit pendienteImpactada;
    private bool saliendoDePendiente;

    [Header("Cuerpo")]
    [SerializeField] private Transform body;

    [Header("Orientacion")]
    [SerializeField] private Transform orientation;

    private Rigidbody mRb;
    private AudioSource mAudioSource;
    private Animator bodyAnimator;

    public Animator BodyAnimator { get => bodyAnimator; set => bodyAnimator = value; }
    public MovementState State { get => state; set => state = value; }
    public float InputVertical { get => inputVertical; set => inputVertical = value; }
    public float InputHorizontal { get => inputHorizontal; set => inputHorizontal = value; }
    public bool Sliding { get => sliding; set => sliding = value; }


    //-------------------------------------------------------------

    private void Awake()
    {
        //Obtenemos referencia al componente RigidBody y congelamos su rotacion
        mRb = GetComponent<Rigidbody>();
        mRb.freezeRotation = true;

        //Inicializamos el deslizamiento en Falso
        sliding = false;

        //Obtenemos referencia al componente de audio
        //mAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //Capturamos la informacion del CapsuleCollider del Personaje
        originalCCCenter = body.GetComponent<CapsuleCollider>().center;
        originalCCHeight = body.GetComponent<CapsuleCollider>().height;
        originalCCRadius = body.GetComponent<CapsuleCollider>().radius;

        BodyAnimator = body.GetComponent<Animator>();
    }

    //-------------------------------------------------------------

    private void StateHandler()
    {
        if (sliding)
        {
            //Actualizamos el Estado de Movimiento
            state = MovementState.sliding;

            //Si estamos descendiendo por una pendiente
            if (EnPendiente() && mRb.velocity.y < 0.1f)
            {
                //La velocidad deseada sera la de Deslizamiento
                desiredMoveSpeed = slideSpeed;
            }
            //En caso no estemos descendiendo,
            else
            {
                //La velocidad deseada sera la de Running
                desiredMoveSpeed = sprintSpeed;
            }
        }

        //MODO - AGACHADO
        else if (Input.GetKey(KeyCode.C))
        {
            //Cambiamos el Estado y la velocidad de movimeinto
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            //Activamos el Flag de Animacion de Agachado
            BodyAnimator.SetBool("IsCrouch", true);
            //Desactivamos el Flag de Animacion para Correr
            BodyAnimator.SetBool("IsRunning", false);
        }

        //MODO - SPRINT
        //Si estamos en el suelo, y mantenemos el boton de Sprint
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            //Cambiamos el Estado y la velocidad de movimeinto
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            //Activamos el Flag de Animacion para Correr
            BodyAnimator.SetBool("IsRunning", true);
            //Desactivamos el Flag de Animacion de Agachado
            BodyAnimator.SetBool("IsCrouch", false);
        }

        //MODO - WALK
        //Si solo estamos en el suelo, y mantenemo
        else if (grounded)
        {
            //Cambiamos el Estado y la velocidad de movimeinto
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;

            //Desactivamos el Flag de Animacion para Correr
            BodyAnimator.SetBool("IsRunning", false);
            //Desactivamos el Flag de Animacion de Agachado
            BodyAnimator.SetBool("IsCrouch", false);
        }
        //MODO - EN EL AIRE
        //Si no estamos en el suelo
        else
        {
            //Cambiamos el Estado para indicar que estamos ene el Aire
            state = MovementState.inAir;
        }

        //Comprobamos si la velocidad de Movimiento se modifico muy Bruscamente, y si seguimos moviendonos
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            //Llamamos a la rutina para conservar el Momentum de nuestra ultima velocidad
            StopAllCoroutines();
            StartCoroutine(ConservarMomentum());
        }
        //Casp contrario, asignamos directamente la velocidad deseada; pues esta es mayor
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        //Guardamos la ultima velocidad que habiamos buscamos
        lastDesiredMoveSpeed = desiredMoveSpeed;
        
    }

    //-------------------------------------------------------------

    private void Update()
    {

        //Capturamos ls inputs de Direccion
        inputVertical = Input.GetAxisRaw("Vertical");
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        StateHandler();

        SpeedControl();

        GroundControl();
        CrouchControl();
        

    }

    //-------------------------------------------------------------------------------------------
    private void GroundControl()
    {
        //Revision de Suelo con un Raycast (rayo hacia el suelo con 1 unidad de largo)
        grounded = Physics.Raycast(body.position, Vector3.down, groundDistance, groundMask);
        
        //Seteamos la Friccion dependiendo de si estamos en el suelo, o no
        if (grounded)
        {
            mRb.drag = groundDrag;
        }
        else
        {
            mRb.drag = 0f;
        }
    }

    //--------------------------------------------------------------------------

    public bool EnPendiente()
    {
        //Si detectamos que hay una superficie debajo...
        if (Physics.Raycast(body.position, Vector3.down, out pendienteImpactada, 0.3f))
        {
            //Obtenemos el Angulo de inclinacion de la superficie debajo
            float angulo = Vector3.Angle(Vector3.up, pendienteImpactada.normal);
            //Retornamos TRUE si el angulo diferente a 0, y es menor al maximo establecido
            return angulo < maximoAnguloDePendiente && angulo != 0;
        }

        //Caso contrario, retornamos falso
        return false;
    }

    //------------------------------------------------------------------------------------------

    public Vector3 ObtenerDireccionDePendiente(Vector3 direction)
    {
        //Obtenemos nuestra direccion Horizontal Proyectada en la Pendiente 
        return Vector3.ProjectOnPlane(direction, pendienteImpactada.normal).normalized;
    }

    //-------------------------------------------------------------------------------------

    private void SpeedControl()
    {
        //Si nos encontramos en una pendiente, y no estamos saliendo de ella
        if (EnPendiente() && !saliendoDePendiente)
        {
            //Si la magnitud de la velocidad es mayor al limite
            if (mRb.velocity.magnitude > moveSpeed)
            {
                //La normalizamos
                mRb.velocity = mRb.velocity.normalized * moveSpeed;
            }
        }

        //Si NO ESTAMOS EN una pendiente...
        else
        {
            //Obtenemos la Velocidad Horizontal del Player
            Vector3 flatVelocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

            //Si la velocidad es mayor a la velocidad limite
            if (flatVelocity.magnitude > moveSpeed)
            {
                //Asignamos una nueva velocidad limitada hasta el maximo de movimiento fijado
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;

                //Nota que estamos respetando la velocidad en Y para no afectar los saltos...
                mRb.velocity = new Vector3(limitedVelocity.x, mRb.velocity.y, limitedVelocity.z);
            }
        }

        
    }

    //--------------------------------------------------------------------------------------------

    private void CrouchControl()
    {
        //Si se ORPIME el boton para agacharse...
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Activamos el Flag de Animacion de Agachado
            BodyAnimator.SetBool("IsCrouch", true);

            //Asignamos los nuevos valores al CapsuleCollider
            body.GetComponent<CapsuleCollider>().center = crouchCCCenter;
            body.GetComponent<CapsuleCollider>().height = crouchCCHeight;
            body.GetComponent<CapsuleCollider>().radius = crouchCCRadius;
        }
        //Si se SUELTA el boton para agacharse...
        if (Input.GetKeyUp(KeyCode.C))
        {
            //Desactivamos el Flag de Animacion para levantarnos
            BodyAnimator.SetBool("IsCrouch", false);

            //Asignamos los valores originales al CapsuleCollider
            body.GetComponent<CapsuleCollider>().center = originalCCCenter;
            body.GetComponent<CapsuleCollider>().height = originalCCHeight;
            body.GetComponent<CapsuleCollider>().radius = originalCCRadius;
        }
    }

    //-------------------------------------------------------------------

    private void FixedUpdate()
    {
        Move();
    }

    //----------------------------------------------------------------------
    //Subrutina para conservar el MOMENTUM al cambiar la velocidad
    private IEnumerator ConservarMomentum()
    {
        
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        
        //El valor inicial dera nuestra velocidad actual
        float startValue = moveSpeed;

        //Mientras el tiempo sea menor a la diferencia de velocidades...
        while (time < difference)
        {
            //Usaremos una interpolacion para que el cambio de velocidad sea suave
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            //Incrementamos el Tiempo de Transicion
            time += Time.deltaTime;
            yield return null;

        }
        //Asignamos que la Velocidad de Movimiento sera la deseada
        moveSpeed = desiredMoveSpeed;
    }

    //----------------------------------------------------------------------
    #region InputActions

    private void Move()
    {
        //Asignamos la direccion de movimeinto considerando la orientacion del Player
        mDirection = orientation.forward * inputVertical + orientation.right * inputHorizontal;

        //Si nos encontramos en una Pendiente, 
        if (EnPendiente())
        {
            //Activamos o desactivamos el Flag de animacion de Caminar dependiendo de
            //si nos estamos moviendo, o no
            if (mDirection == Vector3.zero) BodyAnimator.SetBool("IsWalking", false);
            else BodyAnimator.SetBool("IsWalking", true);

            //Si NO estamos saliendo(saltando) de ella
            if (!saliendoDePendiente)
            {
                //Aplicamos una fuerza mayor en la direccion Proyectada 
                mRb.AddForce(ObtenerDireccionDePendiente(mDirection) * moveSpeed * 20f, ForceMode.Force);

                //Si nuestra velocidad en Y se ve afectada (rebotamos mientras subimos)
                if (mRb.velocity.y > 0)
                {
                    mRb.AddForce(Vector3.down * 60f, ForceMode.Force);
                }
            }
        }

        //Si el Player esta tocando el juelo
        else if (grounded)
        {
            //Activamos o desactivamos el Flag de animacion de Caminar dependiendo de
            //si nos estamos moviendo, o no
            if (mDirection != Vector3.zero) BodyAnimator.SetBool("IsWalking", true);
            else BodyAnimator.SetBool("IsWalking", false);

            //Aplicamos una fuerza al RB del Player para moverlo
            mRb.AddForce(mDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        //Si estamos en el Aire
        else
        {
            //Aplicamos una fuerza, incluyendo el multiplicador de Aire
            mRb.AddForce(mDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //Modificamos la Gravedad cuando estamos en la pendiente
        mRb.useGravity = !EnPendiente();

    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    private void OnJump(InputValue value)
    {
        //Si se oprime el boton de Salto
        if (value.isPressed)
        {
            //Si estamos en una pendiente
            if (EnPendiente())
            {
                //Activamos el Flag de Saliendo de Pared
                saliendoDePendiente = true;
            }

            //Si el Player est? en el suelo, y esta listo para saltar
            //O bien esta saliendo de una pendiente
            if (grounded && readyToJump || saliendoDePendiente && readyToJump)
            {
                //Desactivamos el Flag de ListoParaSaltar
                readyToJump = false;

                //Reiniciamos la velocidad en Y
                mRb.velocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

                //Disparamos el Trigger de Animacion para saltar
                bodyAnimator.SetTrigger("Jump");

                //Saltamos A?adiendo una fuerza de impulso
                mRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                //Tras esperar un determinado tiempo (coolDown) podemos volver a saltar
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    //-------------------------------------------------------------------------------------

    private void ResetJump()
    {
        //Activamos flag de Listo para Saltar
        readyToJump = true;
        //Desactivamos el Flag de Saliendo de Pendiente
        saliendoDePendiente = false;
    }

    #endregion
    //------------------------------------------------------------------------


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(body.position, Vector3.down * groundDistance);
    }
}