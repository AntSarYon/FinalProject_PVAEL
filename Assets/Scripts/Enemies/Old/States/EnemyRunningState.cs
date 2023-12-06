using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyRunningState : FSMState<EnemyController>
    {
        public EnemyRunningState(EnemyController controller) : base(controller)
        {
            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player entr� a la zona de Ataque Especial, y no estamos atacando
                    return mController.PlayerSpecialAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Attacking
                    return new EnemySpecialAttack(mController);
                }));

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo entr� a la zona de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Attacking
                    return new EnemyAttackingState(mController);
                }));

            //- - - - - - - - - - - - -  -

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Enemigo entr� a la zona de Deteccion, y no estamos atacando
                    return mController.PlayerDetectionCollider == null && !mController.MIsAttacking;
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
            //La velocidad inicia del NavMeshAgent sera de 3.75
            mController.NavMeshAgent.speed = 3.75f;


        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            //Activamose el Flag de Animaci�n para Caminar
            mController.MAnimator.SetBool("IsWalking", true);

            //Indicamos que el NavMeshAgent est� en movimiento
            mController.NavMeshAgent.isStopped = false;

            //Asignamos que el destino del NavMeshAgent sea la pisici�n del Jugador
            mController.NavMeshAgent.SetDestination(mController.PlayerDetectionCollider.transform.position);


        }

    }
}

