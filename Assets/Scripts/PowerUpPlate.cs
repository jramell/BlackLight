using UnityEngine;
using System.Collections;

public class PowerUpPlate : MonoBehaviour {

    //Power up type
    string type;

    void Start()
    {
        type = Utils.POWER_UP_SPEED;
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            GameObject.Find("Player").SendMessage("receivePowerUp", type);
        }
    }
}
