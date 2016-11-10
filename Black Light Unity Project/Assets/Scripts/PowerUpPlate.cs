using UnityEngine;
using System.Collections;

public class PowerUpPlate : MonoBehaviour
{

    //Power up type
    string type;

    bool used;

    void Start()
    {
        type = Utils.POWER_UP_SPEED;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!used)
        {
            if (col.gameObject.tag == "Player")
            {
                col.gameObject.GetComponent<PlayerController>().GetComponent<PlayerController>().ReceivePowerUp(type);
                gameObject.GetComponent<AudioSource>().Play();
                //used = true;
               // Destroy(gameObject, gameObject.GetComponent<AudioSource>().clip.length);
            }
        }
    }

    public void SetType(string newType)
    {
        //Change type to that of the parameter. For this to work correctly, the parameter must be one of the constants in Utils
        type = newType;
    }
}
