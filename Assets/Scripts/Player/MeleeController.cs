using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    private PlayerMovement playerController;
    private float tiempoEnfriamiento = 2f;
    private float tiempoProximoAtaque = 0f;
    private static int numClicks = 0;
    private float tiempoUltimoClick = 0;
    public float delayDeComboMaximo = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //Controlamos si ya se terminaron de ejecutar las animaciones de ataque
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1"))
        {
            playerController.BodyAnimator.SetBool("Melee1", false);
        }
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2"))
        {
            playerController.BodyAnimator.SetBool("Melee2", false);
        }
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3"))
        {
            playerController.BodyAnimator.SetBool("Melee3", false);
        }
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4"))
        {
            playerController.BodyAnimator.SetBool("Melee4", false);
        }
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5"))
        {
            playerController.BodyAnimator.SetBool("Melee5", false);
        }
        if (playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
            playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit6"))
        {
            playerController.BodyAnimator.SetBool("Melee6", false);

            //Devolvemos el contador de clicks a 0
            numClicks = 0;
        }
        if (Time.time - tiempoUltimoClick > delayDeComboMaximo)
        {
            //Reiniciamos el numero de Clicks a 0
            numClicks = 0;
        }
        if (Time.time > tiempoProximoAtaque)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }
    }

    //Funcion llamada cuando se hace Click
    public void OnClick()
    {
        //Calculamos el momento (Tiempo) en que se hizo click por ultima vez
        tiempoUltimoClick = Time.time;

        //Incrementamos el numer de clicks
        numClicks++;

        //Si se detecta que es el 1er Click
        if (numClicks == 1)
        {
            //Activamos el Flag de Atacando en el PlayerController
            //playerController.Attacking = true;

            //Activamos el Bool del Primer Ataque
            playerController.BodyAnimator.SetBool("Melee1", true);
        }

        //Limitamos el numero de clicks entre 0 a 6
        numClicks = Mathf.Clamp(numClicks, 0, 6);

        if (numClicks >= 2 && 
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1"))
        {
            playerController.BodyAnimator.SetBool("Melee1", false);
            playerController.BodyAnimator.SetBool("Melee2", true);
        }
        if (numClicks >= 3 &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2"))
        {
            playerController.BodyAnimator.SetBool("Melee2", false);
            playerController.BodyAnimator.SetBool("Melee3", true);
        }
        if (numClicks >= 4 &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3"))
        {
            playerController.BodyAnimator.SetBool("Melee3", false);
            playerController.BodyAnimator.SetBool("Melee4", true);
        }
        if (numClicks >= 5 &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4"))
        {
            playerController.BodyAnimator.SetBool("Melee4", false);
            playerController.BodyAnimator.SetBool("Melee5", true);
        }
        if (numClicks >= 6 &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f &&
             playerController.BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5"))
        {
            playerController.BodyAnimator.SetBool("Melee5", false);
            playerController.BodyAnimator.SetBool("Melee6", true);
        }
    }
}
