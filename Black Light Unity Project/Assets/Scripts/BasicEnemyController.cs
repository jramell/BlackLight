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

    //Unused something
    private bool seriousFight;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isDead = false;
        seriousFight = false;
    }

    void Update()
    {
        transform.LookAt(player.transform.position);
        Attack();
    }

    void Attack()
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
            GetComponent<AudioSource>().Play();
            Destroy(instantiated, 3);
        }
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
}
