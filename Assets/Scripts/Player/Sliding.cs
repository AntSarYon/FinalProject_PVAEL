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
    private bool sliding;

    private float slideCCRadius = 0.6363066f;
    private float slideCCHeight = 1.372868f;
    private Vector3 slideCCCenter = new Vector3(0f, 0.670045912f, 0.276306629f);

    private float originalCCRadius;
    private float originalCCHeight;
    private Vector3 originalCCCenter;

    //------------------------------------------------------------------------------------

    private void Awake()
    {
        //Inicializamos el Flag de DESLIZANDOSE en falso
        sliding = false;
    }

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
        //Si se oprime la tecla Control, y se esta recibiendo Input en alguna direccion
        if (Input.GetKeyDown(KeyCode.LeftControl) && 
            (playerController.inputHorizontal !=0 || playerController.inputVertical != 0))
        {
            //Empezamos el Deslizamiento
            StartSlide();
        }

        //En caso se suelte la tecla Control, y ya estemos deslizandonos
        if (Input.GetKeyUp(KeyCode.LeftControl) && sliding)
        {
            //Terminamos el Deslizamiento
            StopSlide();
        }
    }

    //----------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Si ya estamos deslizandonos
        if (sliding)
        {
            //Ejecutamos el Movimiento de Deslizamiento
            SlidingMovement();
        }
    }

    //------------------------------------------------------------------------------------

    private void StartSlide()
    {
        //Activamos el Flag de "DESLIZANDOSE"
        sliding = true;

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
            playerOrientation.forward * playerController.inputVertical +
            playerOrientation.right * playerController.inputHorizontal;

        //Aplicamos la Fuerza de Deslizamiento en la direccion respectiva
        mRB.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

        //Disminuimos el Timer de Deslizamiento
        slideTimer -= Time.deltaTime;

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
        sliding = false;

        //Asignamos los valores del CapsuleCollider de vuelta a la normalidad
        playerBody.GetComponent<CapsuleCollider>().center = originalCCCenter;
        playerBody.GetComponent<CapsuleCollider>().height = originalCCHeight;
        playerBody.GetComponent<CapsuleCollider>().radius = originalCCRadius;
    }

}
