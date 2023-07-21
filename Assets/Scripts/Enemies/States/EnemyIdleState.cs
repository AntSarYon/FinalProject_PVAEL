using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyIdleState : FSMState<EnemyController>
    {
        public EnemyIdleState(EnemyController controller) : base(controller)
        {

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player entró a la zona de Ataque, y no estamos atacando
                    return mController.PlayerAttackCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado Attacking
                    return new EnemyAttackingState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Player entró a la zona de Deteccion, y no estamos atacando
                    return mController.PlayerDetectionCollider != null && !mController.MIsAttacking;
                },

                getNextState: () => {
                    //Ingresmaos al Estado De Awakening
                    return new EnemyAwakeningState(mController);
                }));

            //- - - - - -- - - - - - - - - - - - - - - - -- - - - -  -

            Transitions.Add(new FSMTransition<EnemyController>(
                isValid: () => {
                    //Si el Flag de Animacion de IsWalking esta funcionando
                    return mController.MAnimator.GetBool("IsWalking");
                },

                getNextState: () => {
                    //Ingresmaos al Estado De Awakening
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
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            mController.NavMeshAgent.ResetPath();
        }
    }
}
