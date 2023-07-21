using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyDyingState : FSMState<EnemyController>
    {
        private float tiempoMuerte;

        public EnemyDyingState(EnemyController controller) : base(controller)
        {

        }

        public override void OnEnter()
        {
            //Desactivamos el Flag de animacion para el movimiento
            mController.MAnimator.SetBool("IsWalking", false);

            //Asignamos velocidad a 0
            mController.MRb.velocity = Vector3.zero;

            //Detenemos al NavMeshAgent
            mController.NavMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            mController.NavMeshAgent.ResetPath();

            //Activamos el Flag de animacion para la Muerte
            mController.MAnimator.SetTrigger("Dying");

            //Inicializamos el Tiempo de muerte en 0
            tiempoMuerte = 0;

            mController.MAudioSource.PlayOneShot(mController.clipsMuerte[Random.Range(0, mController.clipsMuerte.Length)], 0.30f);
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            //Si el tiempo de animacion de muerte ya apso los 5 segundos...
            if (tiempoMuerte >= 5F)
            {
                //Destruimos el Zombie
                GameObject.Destroy(mController.gameObject);
            }

            //Sino
            else
            {
                //Incrementamos el Tiempo de Muerte
                tiempoMuerte += deltaTime;
            }
        }
    }
}

