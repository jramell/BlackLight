﻿using UnityEngine;
using System.Collections;

public class DummyController : MonoBehaviour {

    public int health;

    private bool punched = false;

	// Use this for initialization
	void TakeDamage(int damage)
    {
        if (!punched)
        {
            GameObject.Find("BlueP").GetComponent<TutorialController>().RegisterDummyPunch();
            punched = true;
        }

        GameObject.Find("Player").GetComponent<PlayerController>().RegisterDummyPunch();
    }
	
	// Update is called once per frame
	void Die()
    {
        Destroy(gameObject);
    }
}
