using UnityEngine;
using System.Collections;

public class PowerUpPlateSpawner : MonoBehaviour
{

    public GameObject powerUpPlatePrefab;

    //private bool spawned;

    void Start()
    {
       // spawned = false;
    }

    public void SpawnPowerUpPlate(bool spawn)
    {
     //   if (spawn)
        //{
            transform.FindChild("PowerUpPlate").gameObject.SetActive(spawn);
            //spawned = true;
        //}
    }
}
