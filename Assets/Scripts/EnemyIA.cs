using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{

    #region ENUMS
    public enum Estados
    {
        Esperar,
        Patrular,
        Perseguir,
        Attack,
        Death
    }
    #endregion

    [Header("Waypoints - Patrula")]
    public Vector3 waypointAtual;
    public Transform waypoint;
    private Transform alvo;

    [Header("Componentes - AI")]
    private NavMeshAgent navMeshAgent;
    public Animator animController;

    [Header("HP")]
    public int hp = 2;

    private Estados estadoAtual;

    [Header("Variáveis - Controle")]
    public float tempoEsperar = 2f;
    public float tempoEsperarVolta = 2f;
    private float tempoEsperando;
    public PlayerController player;
    public float distanceBetweenPlayer;
    public Rigidbody enemyrb;
    public float knockbackForce = 10f;
    private bool canTakeDamege = true;
    private bool isAttacking = false;

    [Header("Distancias")]
    public float distanciaMinimaWaypoint = 0.15f;
    private float distanciaWaypointAtual;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyrb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void Start()
    {
        Esperar();
    }
    void Update()
    {

        ChecarStatus();
    }

    private void ChecarStatus()
    {
        switch (estadoAtual)
        {
            case Estados.Esperar:
                if (EsperouTempoSuficiente())
                {
                    Patrulhar();
                }
                else
                {
                    alvo = transform;
                }

                break;
            case Estados.Patrular:
                if (Vector3.Distance(transform.position, player.transform.position) <= distanceBetweenPlayer)
                {
                    Perseguir();
                }
                else
                {
                    Debug.DrawLine(transform.position, waypoint.position, Color.blue);
                }
                if (PertoWaypointAtual() || EsperouTempoSuficiente())
                {
                    Esperar();
                }
                break;
            case Estados.Perseguir:
                if (Vector3.Distance(transform.position, player.transform.position) <= distanceBetweenPlayer)
                {
                    alvo = player.transform;
                    Debug.DrawLine(transform.position, player.transform.position, Color.green);
                    print(Vector3.Distance(transform.position, player.transform.position));

                    if (Vector3.Distance(transform.position, player.transform.position) <= 2f && isAttacking == false)
                    {
                        Attack();
                    }
                }
                else
                {
                    Patrulhar();
                    Debug.DrawLine(transform.position, player.transform.position, Color.red,1f);
                }
                break;
            case Estados.Attack:
                if (Vector3.Distance(transform.position, player.transform.position) <= 2f)
                {
                    if (isAttacking)
                    {
                        isAttacking = false;
                        StartCoroutine(WaitToAttack());
                    }

                }
                else
                {
                    Patrulhar();
                    Debug.DrawLine(transform.position, player.transform.position, Color.red, 1f);
                }
                break;
            case Estados.Death:
                alvo = transform;
                break;
            default:
                break;
        }
        print(estadoAtual);
        navMeshAgent.destination = alvo.position;
    }

    //Estados
    void Esperar()
    {
        estadoAtual = Estados.Esperar;
        tempoEsperando = Time.time;
        animController.SetBool("Waiting", true);
        animController.SetBool("Patrolling", false);
        animController.SetBool("Chasing", false);
    }
    void Patrulhar()
    {
        animController.SetBool("Waiting", false);
        animController.SetBool("Patrolling", true);
        animController.SetBool("Chasing", false);
        estadoAtual = Estados.Patrular;
        AlterarWaypoint();
        alvo = waypoint;
        tempoEsperando = Time.time;
    }
    void Perseguir()
    {
        estadoAtual = Estados.Perseguir;
        animController.SetBool("Chasing", true);
    }
    void Attack()
    {
        estadoAtual = Estados.Attack;
        animController.SetTrigger("Attack");
        isAttacking = true;
    }

    IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(1f);
        Esperar();

    }

    void Dano()
    {
        //Knockback();
        hp--;
        print(hp);
        if (hp <= 0)
        {
            Morrer();
        }
    }

    public void Morrer()
    {
        estadoAtual = Estados.Death;
        
        canTakeDamege = false;
        animController.SetTrigger("Death");
        Destroy(this.gameObject, 2f);
    }
    void Knockback()
    {
        navMeshAgent.Warp(transform.position + player.pivot.forward * knockbackForce);
        navMeshAgent.Warp(transform.position + transform.up);
    }
    //Métodos Comuns
    private bool EsperouTempoSuficiente()
    {
        return tempoEsperando + tempoEsperar < Time.time;
    }

    private bool PertoWaypointAtual()
    {
        distanciaWaypointAtual = Vector3.Distance(transform.position, waypoint.position);
        return distanciaWaypointAtual < distanciaMinimaWaypoint;
    }

    private void AlterarWaypoint()
    {
        float posX = Random.Range(transform.position.x - 2, transform.position.x + 2);
        float posY = transform.position.y;
        float posZ = Random.Range(transform.position.z - 2, transform.position.z + 2);
        waypointAtual = new Vector3(posX,posY,posZ);
        waypoint.position = waypointAtual;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SwordHit"))
        {
            if (canTakeDamege)
            {
                Dano();
            }
        }
    }
}

