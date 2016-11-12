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
        //Debug.Log("attack entered trigger");
        if (col.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerController>().TakeDamage(damage))
            {
                //Play a certain sound
            }

            else
            {
                GetComponent<MeshRenderer>().enabled = false;
                transform.Find("particles").GetComponent<ParticleSystem>().Play();
            }
            Destroy(gameObject, 0.5f);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}