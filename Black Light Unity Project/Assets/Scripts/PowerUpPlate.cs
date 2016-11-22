using UnityEngine;
using System.Collections;

public class PowerUpPlate : MonoBehaviour
{

    //Power up type
    string type;

    float lastTimeUsed;

    public float cooldown;

    float originalParticleSpeed;

    ParticleSystem particleSystem;

    PlayerController playerController;

    void Start()
    {
        type = Utils.POWER_UP_SPEED;
        particleSystem = transform.Find("Particles").gameObject.GetComponent<ParticleSystem>();
        originalParticleSpeed = particleSystem.startSpeed;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (Time.time - lastTimeUsed > cooldown)
        {
            if (col.gameObject.tag == "Player")
            {
                playerController.ReceivePowerUp(type);
                gameObject.GetComponent<AudioSource>().Play();
                lastTimeUsed = Time.time;
                StartCoroutine(StartCooldown());
            }
        }
    }

    IEnumerator StartCooldown()
    {
     //   float result = (cooldown - (Time.time - lastTimeUsed)) * originalParticleSpeed;
        float tempCooldown = 0;
        float tempRelation = 0;
        float waitingTime = 0.2f;
        while(tempCooldown < 5)
        {
            tempRelation = (tempCooldown / cooldown) * originalParticleSpeed;
            particleSystem.startSpeed = tempRelation;
            yield return new WaitForSeconds(waitingTime);
            tempCooldown += waitingTime;
        }
    }

    public void SetType(string newType)
    {
        //Change type to that of the parameter. For this to work correctly, the parameter must be one of the constants in Utils
        type = newType;
    }
}
