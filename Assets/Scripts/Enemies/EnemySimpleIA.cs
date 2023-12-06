using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class EnemySimpleIA : MonoBehaviour
{
    [Header("Nav Mesh Agent")]
    public NavMeshAgent agent;

    [Header("Transform del Jugador")]
    public Transform player;

    [Header("Orientacion")]
    public Transform orientation;
    public Transform shootOrigin;

    [Header("Capas")]
    public LayerMask WhatIsGround;
    public LayerMask WhatIsPlayer;

    [Header("Animator de cuerpo")]
    [SerializeField] private Animator bodyAnimator;
    

    [Header("Patrullaje")]
    public Vector3 walkPoint;
    bool walkPointSet;

    public float walkPointRange;

    [Header("Ataque")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool isAttacking;


    [Header("Rangos")]
    public float sightRange;
    public float attackRange;

    [Header("Estados")]
    public bool playerInSightRange, playerInAttackRange;

    [Header("Salus del enemigo")]
    public float health;

    [Header("Objeto Proyectil")]
    [SerializeField] private GameObject projectile;

    //-----------------------------------------------------------------------------

    private void Awake()
    {
        player = GameObject.Find("PlayerSwordMan").transform;
        agent = GetComponent<NavMeshAgent>();
        isAttacking = false;
    }

    //-----------------------------------------------------------------------------

    void Update()
    {
        //Comprobamos el rango de visión y de Ataque
        playerInSightRange = Physics.CheckSphere(transform.position,sightRange,WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        if (!isAttacking)
        {
            if (!playerInSightRange && !playerInAttackRange)
            {
                bodyAnimator.SetBool("Throwing", false);
                Patroling();
            }


            if (playerInSightRange && !playerInAttackRange)
            {
                bodyAnimator.SetBool("Throwing", false);
                ChasePlayer();
            }


            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        
    }

    //-----------------------------------------------------------------------------

    private void Patroling()
    {
        //Si no hay un punto de destino; lo buscamos...
        if (!walkPointSet) SearchWalkPoint();
        
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Punto de destino alcanzado

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false; //buscaremos otro punto;
        }
        
    }

    //-----------------------------------------------------------------------------
    private void SearchWalkPoint()
    {
        //Calculamos una posicion aleatoria dentro de un rango

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
            );

        //Comprobamos que ese punto cuarde contacto con el suelo del escenario
        if (Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsGround))
        {
            walkPointSet = true;
        }
    }

    //-----------------------------------------------------------------------------

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    //-----------------------------------------------------------------------------

    private void AttackPlayer()
    {
        //Hacemos que no se mueva
        agent.SetDestination(transform.position);

        //Que mire al jugador
        transform.LookAt(player);

        //Si aun no ha atacado
        if (!alreadyAttacked)
        {
            bodyAnimator.SetBool("Throwing",true);
        }
    }

    public void ThrowProjectile()
    {
        Rigidbody rb = Instantiate(projectile, shootOrigin.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(shootOrigin.forward * 30f, ForceMode.Impulse);
        rb.AddForce(shootOrigin.up * -3.5f, ForceMode.Impulse);

        //Activamos el Flag de Ya atacó
        alreadyAttacked = true;
    }

    public void AnnounceAttack()
    {
        isAttacking = true;
    }

    //------------------------------------------------------------------------
    //Llamar a esta funcion cuando se pueda iniciar otro ataque

    public void ResetAttack()
    {
        alreadyAttacked = false;
        isAttacking = false;
    }

    public void TakeDamage(int Damage)
    {
        health -= Damage;

        if (health <= 0) Invoke(nameof(KillEnemy), 0.5f);
    }

    private void KillEnemy()
    {
        //Codigo de Ragdoll Activado

        Destroy(gameObject);
    }

    //-----------------------------------------------------------------------------



}
