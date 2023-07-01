using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform playerOrientation;
    [SerializeField] private Transform playerBody;
    private Rigidbody mRB;
    private PlayerMovement playerController;

    [Header("Deslizamiento")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float slideTimer;

    private float slideCCRadius = 0.3036255f;
    private float slideCCHeight = 2.043422f;
    private Vector3 slideCCCenter = new Vector3(0, 0.311746418f, 0.101016998f);

    private float originalCCRadius;
    private float originalCCHeight;
    private Vector3 originalCCCenter;

    //----------------------------------------------------------------------------------------

    private void Start()
    {
        mRB = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerMovement>();

        //Capturamos la informacion del CapsuleCollider del Personaje
        originalCCCenter = playerBody.GetComponent<CapsuleCollider>().center;
        originalCCHeight = playerBody.GetComponent<CapsuleCollider>().height;
        originalCCRadius = playerBody.GetComponent<CapsuleCollider>().radius;
    }

    //------------------------------------------------------------------------------------

    private void Update()
    {
        //Si se oprime la tecla Control, y se esta recibiendo Input en alguna direccion, y estamos CAMINANDO, o CORRIENDO
        if (Input.GetKeyDown(KeyCode.LeftControl) && 
            (playerController.InputHorizontal !=0 || playerController.InputVertical != 0) && 
            (playerController.State == MovementState.walking || playerController.State == MovementState.sprinting))
        {
            //Empezamos el Deslizamiento
            StartSlide();
        }

        //En caso se suelte la tecla Control, y ya estemos deslizandonos
        if (Input.GetKeyUp(KeyCode.LeftControl) && playerController.Sliding)
        {
            //Terminamos el Deslizamiento
            StopSlide();
        }
    }

    //----------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Si ya estamos deslizandonos
        if (playerController.Sliding)
        {
            //Ejecutamos el Movimiento de Deslizamiento
            SlidingMovement();
        }
    }

    //------------------------------------------------------------------------------------

    private void StartSlide()
    {
        //Activamos el Flag de "DESLIZANDOSE"
        playerController.Sliding = true;

        //Disparamos el Trigger de Animacion para el Deslizamiento
        playerController.BodyAnimator.SetTrigger("Slide");

        //Convertimos la Direccion del CapsuleCollider al EjeZ
        playerBody.GetComponent<CapsuleCollider>().direction = 2;
        //Asignamos los nuevos valores al CapsuleCollider
        playerBody.GetComponent<CapsuleCollider>().center = slideCCCenter;
        playerBody.GetComponent<CapsuleCollider>().height = slideCCHeight;
        playerBody.GetComponent<CapsuleCollider>().radius = slideCCRadius;

        //Asignamos el maximo valor al Timer para empezar la cuenta atras
        slideTimer = maxSlideTime;
    }

    //------------------------------------------------------------------------------------

    private void SlidingMovement()
    {
        //Obtenemos la direccion en la que ocurrira el deslizamiento
        Vector3 inputDirection = 
            playerOrientation.forward * playerController.InputVertical +
            playerOrientation.right * playerController.InputHorizontal;

        //Si el Jugador NO ESTA en una pendiente o tiene velocidad vertical CASI NULA
        if (!playerController.EnPendiente() || mRB.velocity.y > -0.1f)
        {
            //DESLIZAMIENTO NORMAL!
            //Aplicamos la Fuerza de Deslizamiento en la direccion respectiva
            mRB.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            //Disminuimos el Timer de Deslizamiento
            slideTimer -= Time.deltaTime;
        }
        //Si el jugador esta en una pendiente
        else 
        {
            //Aplicamos fuerza en la direccion de Descenso de la Pendiente
            mRB.AddForce(playerController.ObtenerDireccionDePendiente(inputDirection).normalized * slideForce, ForceMode.Force);
            //OJO -< AQUI El timer no se esta restando, por lo que podemos deslizarnos infinitamente
        }


        //Si el timer llega a 0
        if (slideTimer <= 0)
        {
            //Detenemos el Deslizamiento
            StopSlide();
        }
    }

    //------------------------------------------------------------------------------------

    private void StopSlide()
    {
        //Desactivamos el Flag de "DESLIZANDOSE"
        playerController.Sliding = false;

        //Restauramos la Direccion del CapsuleCollider al EjeY
        playerBody.GetComponent<CapsuleCollider>().direction = 1;
        //Asignamos los valores del CapsuleCollider de vuelta a la normalidad
        playerBody.GetComponent<CapsuleCollider>().center = originalCCCenter;
        playerBody.GetComponent<CapsuleCollider>().height = originalCCHeight;
        playerBody.GetComponent<CapsuleCollider>().radius = originalCCRadius;
    }

}
