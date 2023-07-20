using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombo : MonoBehaviour
{
    private Animator mAnimator;

    int cantidadClicks;
    bool puedoDarClick;

    public int CantidadClicks { get => cantidadClicks; set => cantidadClicks = value; }

    //----------------------------------------------------------

    private void OnEnable()
    {
        mAnimator = GetComponent<Animator>();
        cantidadClicks = 0;
        puedoDarClick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IniciarCombo();
        }
    }

    public void IniciarCombo()
    {
        if (puedoDarClick)
        {
            cantidadClicks++;
        }
        if (cantidadClicks == 1)
        {
            mAnimator.SetInteger("MeleeAttack", 1);
        }
    }

    public void VerificarCombo()
    {
        puedoDarClick = false;

        //Si estoy en el ataque 1, y la cantidad de Clicks totales fue igual a 1
        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1") && cantidadClicks == 1)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        //Si estoy en el ataque 1, y la cantidad de Clicks totales fue igual o mayor a 2
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1") && cantidadClicks >= 2)
        {
            //Me voy a la Animacion de Ataque 2
            mAnimator.SetInteger("MeleeAttack", 2);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 2, y la cantidad de Clicks totales fue igual a 2
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2") && cantidadClicks == 2)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        //Si estoy en el ataque 2, y la cantidad de Clicks totales fue igual o mayor a 3
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2") && cantidadClicks >= 3)
        {
            //Me voy a la Animacion de Ataque 3
            mAnimator.SetInteger("MeleeAttack", 3);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 3, y la cantidad de Clicks totales fue igual a 3
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3") && cantidadClicks == 3)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3") && cantidadClicks >= 4)
        {
            //Me voy a la Animacion de Ataque 4
            mAnimator.SetInteger("MeleeAttack", 4);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 4, y la cantidad de Clicks totales fue igual a 4
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4") && cantidadClicks == 4)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4") && cantidadClicks >= 5)
        {
            //Me voy a la Animacion de Ataque 5
            mAnimator.SetInteger("MeleeAttack", 5);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 5, y la cantidad de Clicks totales fue igual a 5
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5") && cantidadClicks == 5)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5") && cantidadClicks >= 6)
        {
            //Me voy a la Animacion de Ataque 6
            mAnimator.SetInteger("MeleeAttack", 6);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 6, y la cantidad de Clicks totales fue igual a 6
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit6") && cantidadClicks == 6)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit6") && cantidadClicks > 6) 
        { 
            //Regresamos a IDLE
            mAnimator.SetInteger("MeleeAttack", 0);
            puedoDarClick = true;
            cantidadClicks = 0;
        }
    }
}
