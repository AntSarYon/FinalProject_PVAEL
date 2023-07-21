using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackingState : FSMState<EnemyController>
    {
        public EnemyAttackingState(EnemyController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo no esta en la zona de Deteccion, ni en la de ataque, y no estamos atacando
                    return mController.PlayerSpecialAttackCollider == null && mController.PlayerDetectionCollider == null && mController.PlayerAttackCollider == null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new EnemyIdleState(mController);
                }));

            //- - - - - - - - - - - - - - - - - - - - - - - - - - -

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo esta en la zona de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado ATACCKING otra vez
                    return new EnemyAttackingState(mController);
                }));

            // - - - - - - - - - - - - - - - - - - -- - - - - - - - - - - - -- 

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo esta en la zona de Deteccion, y no estamos atacando
                    return mController.PlayerDetectionCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado IDLE
                    return new EnemyRunningState(mController);
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
            //Volteamos en direccion al Player
            mController.transform.LookAt(mController.PlayerAttackCollider.transform);

            //Desactivamos el Flag de Animacion para Caminar
            mController.MAnimator.SetBool("IsWalking", false);

            //Disparamos el Trigger de Ataque
            mController.MAnimator.SetTrigger("Attack");

            mController.MAudioSource.PlayOneShot(mController.clipsAtacando[Random.Range(0, mController.clipsAtacando.Length)], 0.225f);
        }

        public override void OnExit()
        {
            mController.MAudioSource.Stop();
        }

        public override void OnUpdate(float deltaTime)
        {
            //Nos detenemos asignando la velocididad en 0
            mController.MRb.velocity = new Vector3(
                0f,
                0f,
                0f
            );

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;
        }

    }
}