using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCombo : MonoBehaviour
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
        if (CantidadClicks == 1)
        {
            mAnimator.SetInteger("SwordAttack", 1);
        }
    }

    public void VerificarCombo()
    {
        puedoDarClick = false;

        //Si estoy en el ataque 1, y la cantidad de Clicks totales fue igual a 1
        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1") && CantidadClicks == 1)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick=true;
            CantidadClicks=0;
        }
        //Si estoy en el ataque 1, y la cantidad de Clicks totales fue igual o mayor a 2
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit1") && CantidadClicks >= 2)
        {
            //Me voy a la Animacion de Ataque 2
            mAnimator.SetInteger("SwordAttack", 2);
            puedoDarClick=true;
        }
        //Si estoy en el ataque 2, y la cantidad de Clicks totales fue igual a 2
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2") && CantidadClicks == 2)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        //Si estoy en el ataque 2, y la cantidad de Clicks totales fue igual o mayor a 3
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit2") && CantidadClicks >= 3)
        {
            //Me voy a la Animacion de Ataque 3
            mAnimator.SetInteger("SwordAttack", 3);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 3, y la cantidad de Clicks totales fue igual a 3
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3") && CantidadClicks == 3)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit3") && CantidadClicks >= 4)
        {
            //Me voy a la Animacion de Ataque 4
            mAnimator.SetInteger("SwordAttack", 4);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 4, y la cantidad de Clicks totales fue igual a 4
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4") && CantidadClicks == 4)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit4") && CantidadClicks >= 5)
        {
            //Me voy a la Animacion de Ataque 5
            mAnimator.SetInteger("SwordAttack", 5);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 5, y la cantidad de Clicks totales fue igual a 5
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5") && CantidadClicks == 5)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit5") && CantidadClicks >= 6)
        {
            //Me voy a la Animacion de Ataque 6
            mAnimator.SetInteger("SwordAttack", 6);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 6, y la cantidad de Clicks totales fue igual a 6
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit6") && CantidadClicks == 6)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit6") && CantidadClicks >= 7)
        {
            //Me voy a la Animacion de Ataque 7
            mAnimator.SetInteger("SwordAttack", 7);
            puedoDarClick = true;
        }
        //Si estoy en el ataque 7 (FINAL)
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit7") && CantidadClicks == 7)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
        else if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit7") && CantidadClicks > 7)
        {
            //Regresamos a IDLE
            mAnimator.SetInteger("SwordAttack", 0);
            puedoDarClick = true;
            CantidadClicks = 0;
        }
    }
}
