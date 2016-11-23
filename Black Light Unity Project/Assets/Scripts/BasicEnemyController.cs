using UnityEngine;
using System.Collections;

public class BasicEnemyController : MonoBehaviour
{

    public int healthPoints;

    public GameObject attackObject;

    public float attackCooldownStart;

    public float attackCooldownEnd;

    private float lastAttackCooldown;

    public GameObject attackSpawner;

    public int health;

    public float attackCooldown;

    private GameObject player;

    private float lastAttack;

    private bool isDead;

    private Rigidbody rigidbody;

    //Starts attacking when player is closer than this distance
    public float distanceToAttack;

    //Unused something
    private bool seriousFight;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isDead = false;
        seriousFight = true;
    }

    void Update()
    {
        if(!isDead)
        {
        transform.LookAt(player.transform.position);
        Attack();

        }
    }

    void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distanceToAttack)
        {
            if (Time.time - lastAttack > lastAttackCooldown)
            {
                if (seriousFight)
                {
                    lastAttackCooldown = Random.Range(attackCooldownStart, attackCooldownEnd);
                }

                else
                {
                    lastAttackCooldown = attackCooldown;
                }

                lastAttack = Time.time;
                Object instantiated = Instantiate(attackObject, attackSpawner.transform.position, transform.rotation);
                AudioSource audioCopy = GetComponent<AudioSource>();
                audioCopy.pitch = Random.Range(0.8f, 1.2f);
                audioCopy.Play();
                Destroy(instantiated, 3);
            }
        }

    }

    void Die()
    {
        isDead = true;
        PlayerController.InformEnemyDeath();
        Destroy(gameObject);
    }

    public void TakeDamageWithEffect(Vector3 force, int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DieWithEffect(force);
        }
    }

    void DieWithEffect(Vector3 force)
    {
        isDead = true;
        PlayerController.InformEnemyDeath();
        rigidbody.freezeRotation = false;
        rigidbody.AddForce(force, ForceMode.Acceleration);
        Destroy(gameObject, 1f);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
}
