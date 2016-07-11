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
            col.gameObject.SendMessage("receivePowerUp", type);
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    public void SetType(string newType)
    {
        //Change type to that of the parameter. For this to work correctly, the parameter must be one of the constants in Utils
        type = newType;
    }
}
