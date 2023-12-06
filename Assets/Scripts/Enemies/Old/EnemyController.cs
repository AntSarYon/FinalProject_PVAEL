using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //Distancias de deteccion y ataque
    private float AwakeRadio = 8f;
    private float AttackRadio = 1.20f;
    private float SpecialAttackRadio = 3f;

    //Salud
    public float Health = 10f;

    //Referencia a HitBoxes
    public GameObject hitboxRight;
    public GameObject hitboxLeft;

    //Referencia a componentes
    private Animator mAnimator;
    private Rigidbody mRb;

    //Referencia al NavMeshAgent para controlar la zona de movimiento
    private NavMeshAgent navMeshAgent;

    //Vector de dirección para el movimiento
    private Vector2 mDirection; // XZ

    //Flag de Ataque en curso -> Inicializado en Falso
    private bool mIsAttacking = false;

    private Collider playerDetectionCollider;
    private Collider playerAttackCollider;
    private Collider playerSpecialAttackCollider;

    //Tendremos una Maquina de Estados Finita (FSM)
    private FSM<EnemyController> mFSM;

    private AudioSource mAudioSource;

    public AudioClip[] clipsDespertando;
    public AudioClip[] clipsJugadorCerca;
    public AudioClip[] clipsAtacando;
    public AudioClip[] clipsSaltando;
    public AudioClip[] clipsMuerte;

    public Animator MAnimator { get => mAnimator; set => mAnimator = value; }
    public Rigidbody MRb { get => mRb; set => mRb = value; }
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }
    public Vector2 MDirection { get => mDirection; set => mDirection = value; }
    public Collider PlayerDetectionCollider { get => playerDetectionCollider; set => playerDetectionCollider = value; }
    public Collider PlayerAttackCollider { get => playerAttackCollider; set => playerAttackCollider = value; }
    public bool MIsAttacking { get => mIsAttacking; set => mIsAttacking = value; }
    public Collider PlayerSpecialAttackCollider { get => playerSpecialAttackCollider; set => playerSpecialAttackCollider = value; }
    public AudioSource MAudioSource { get => mAudioSource; set => mAudioSource = value; }

    //-----------------------------------------------------------------------

    private void Start()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        mAnimator = transform.GetComponentInChildren<Animator>(false); //<-- Le decimos que considere a los GO hijos desactivados
        navMeshAgent = GetComponent<NavMeshAgent>();
        MAudioSource = GetComponent<AudioSource>();

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //Creamos un FSM indicando este Script como principal componente
        mFSM = new FSM<EnemyController>(

            //El Estado inicial será...
            new Enemy.EnemyIdleState(this)
            );

        // Activamos la máquina de estados
        mFSM.Begin();
    }

    //-------------------------------------------------------------------------

    private void Update()
    {
        //Detectamos el collider del Player dentro de la zona de Ataque Especial
        playerSpecialAttackCollider = IsPlayerInSpecialAttackArea();

        //Detectamos el collider del Player dentro de la zona de Ataque
        playerAttackCollider = IsPlayerInAttackArea();

        //Detectamos el collider del Player dentro de la zona de Detección
        playerDetectionCollider = IsPlayerNearby();

    }

    //----------------------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Le pasamos el FixedDeltaTime para no Afectar el rendimiento
        mFSM.Tick(Time.fixedDeltaTime);
    }


    //---------------------------------------------------------------------------------------
    //Función para detectar si el jugador está cerca
    private Collider IsPlayerNearby()
    {
        //Creamos una esfera alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            AwakeRadio, //Radio de Detección
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en el área; lo retornamos
        if (colliders.Length == 1) return colliders[0];

        //Caso contrario, retornamos un Nulo
        else return null;
    }

    //------------------------------------------------------------------------------------------
    //Función para detectar si el jugador está en la zona de Ataque

    private Collider IsPlayerInAttackArea()
    {
        //Creamos una esfera pequeña alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            AttackRadio, //radio de ataque
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en la zona de ataque; lo retornamos
        if (colliders.Length == 1) return colliders[0];

        //Caso contrario, retornamos nulo
        else return null;
    }

    //------------------------------------------------------------------------------------------
    //Función para detectar si el jugador está en la zona de Ataque Especial

    private Collider IsPlayerInSpecialAttackArea()
    {
        //Creamos una esfera pequeña alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            SpecialAttackRadio, //radio de ataque
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en la zona de ataque; lo retornamos
        if (colliders.Length == 1) return colliders[0];

        //Caso contrario, retornamos nulo
        else return null;
    }

    //-----------------------------------------------------------------------------------------
    //Función para Iniciar el Ataque
    public void StartAtack()
    {
        //Activamos el FLAG de ATACANDO
        mIsAttacking = true;
    }

    //----------------------------------------------------------------------
    //Función para habilitar los objetos con los HitBoxes
    public void EnableHitbox()
    {
        //Activamos los Hitboxes
        hitboxLeft.SetActive(true);
        hitboxRight.SetActive(true);
    }

    //---------------------------------------------------------------------------------
    //Función para DETENER EL ATAQUE
    public void StopAttack()
    {
        //Desactivamos el Flag de ATACANDO
        mIsAttacking = false;

        //Desactivamos los Objetos con HitBox
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);
    }

    //---------------------------------------------------------------------------------------------
    //Función para el Recibimiento de DAÑO

    public void TakeDamage(float damage)
    {
        //Reducimos a la Salud el daño recibido por parte del jugador
        Health -= damage;
    }

    public void RepdorucirSalto()
    {
        MAudioSource.PlayOneShot(clipsSaltando[Random.Range(0, clipsSaltando.Length)], 0.7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox"))
        {
            TakeDamage(0.25f);
        }
        
    }

}
