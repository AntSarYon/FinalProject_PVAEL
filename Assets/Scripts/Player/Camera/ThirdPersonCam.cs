using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform playerOrientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Rigidbody rbPlayer;

    public float rotationSpeed;

    private void Start()
    {
        //Hacemos que el Cursor no sea visible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
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

        //Capturamos los Inputs de Direccion
        float horizontalInput = Input.GetAxis("Horizontal");
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
