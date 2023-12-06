using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyController : MonoBehaviour
{
    private EnemySimpleIA mIA;

    //------------------------------------------------
    void Awake()
    {
        mIA = GetComponentInParent<EnemySimpleIA>();
    }

    public void ThrowProjectile()
    {
        mIA.ThrowProjectile();
    }

    public void ResetAttack()
    {
        mIA.ResetAttack();
    }

    public void AnnounceAttack()
    {
        mIA.AnnounceAttack();
    }
}
