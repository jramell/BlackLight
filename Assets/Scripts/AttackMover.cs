using UnityEngine;
using System.Collections;

public class AttackMover : MonoBehaviour
{

    public float speed;

    public int damage;

    private Vector3 playerDirection;

    void Update()
    {
        transform.Translate(0, 0, speed);
        StartCoroutine(TurnLightOff());
    }

   IEnumerator TurnLightOff()
    {
        yield return new WaitForSeconds(0.2f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").SendMessage("TakeDamage", damage);
            Destroy(gameObject);
        }

        else if (col.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}