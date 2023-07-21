using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAwakeningState : FSMState<EnemyController>
    {
        private float tiempoTranscurrido;

        public EnemyAwakeningState(EnemyController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si se Activa el Flag de Correr...
                    return mController.MAnimator.GetBool("IsWalking");
                },

                getNextState: () => {
                    //Ingresmaos al Estado RUNNING
                    return new EnemyRunningState(mController);
                }));

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player entró a la zona de Ataque Especial, y no estamos atacando
                    return mController.PlayerSpecialAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Attacking
                    return new EnemySpecialAttack(mController);
                }));

            //- - - - - - - - - - - - - - -- - - - - - - - - -- - - - - - 
            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player esta en Rango de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado ATTACKING
                    return new EnemyAttackingState(mController);
                }));
            //- - - - - - - - - - - - - - -- - - - - - - - - -- - - - - - 
            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player no esta en Rango de Deteccion ni de Ataque, y no estamos atacando
                    return mController.PlayerSpecialAttackCollider == null && mController.PlayerDetectionCollider == null && mController.PlayerAttackCollider == null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new EnemyIdleState(mController);
                }));

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo se queda sin vida...
                    return mController.Health <= 0;
                },

                getNextState: () => {
                    //Ingresmaos al Estado De Dying
                    return new EnemyDyingState(mController);
                }));
        }

        public override void OnEnter()
        {
            mController.transform.LookAt(mController.PlayerDetectionCollider.transform);

            mController.MAudioSource.PlayOneShot(mController.clipsDespertando[Random.Range(0, mController.clipsDespertando.Length)], 0.3f);

            //Disparamos el Flag de Animacion de Alerta
            mController.MAnimator.SetTrigger("Alert");

            //Inicializamos el tiempo Transcurrido en 0
            tiempoTranscurrido = 0;
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            //Si ya transcurrieron 2.20 segundos
            if (tiempoTranscurrido >= 1.10f)
            {
                //Activamose el Flag de animacion para correr
                mController.MAnimator.SetBool("IsWalking", true);
            }
            //Sino
            else
            {
                //Mantenemos su flag de animacion desactivado
                mController.MAnimator.SetBool("IsWalking", false);


                //Incrementamos el tiempo transcurrido
                tiempoTranscurrido += deltaTime;
            }

        }

    }
}
