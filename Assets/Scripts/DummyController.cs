using UnityEngine;
using System.Collections;

public class DummyController : MonoBehaviour {

    public int health;

	// Use this for initialization
	void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }
	
	// Update is called once per frame
	void Die()
    {
        Destroy(gameObject);
        GameObject.Find("Baroth").GetComponent<TutorialController>().RegisterDummyDefeat();
    }
}
