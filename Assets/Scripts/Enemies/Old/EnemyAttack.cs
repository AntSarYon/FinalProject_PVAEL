using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //Referencia al componente de ENEMY CONTROLLER
    [SerializeField]
    private EnemyController controller;

    //--------------------------------------------------------------
    //Función para iniciar el Ataque --> Llamada al EnemyController
    public void StartAtack()
    {
        controller.StartAtack();
    }

    //-----------------------------------------------
    //Función para Detener el Ataque  --> Llamada al EnemyController
    public void StopAttack()
    {
        controller.StopAttack();
        //Internamente estamos desactivando el HitBox
    }

    //---------------------------------------------
    //Función para Activar el Hitbox

    public void EnableHitbox()
    {
        controller.EnableHitbox();
    }

    public void ReproducirSalto()
    {
        controller.RepdorucirSalto();
    }
}
