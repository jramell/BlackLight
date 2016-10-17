using UnityEngine;
using System.Collections;

public class PowerUpPlateSpawner : MonoBehaviour
{

    public GameObject powerUpPlatePrefab;

    private bool spawned;

    void Start()
    {
        spawned = false;
    }

    public void SpawnPowerUpPlate()
    {
        if (!spawned)
        {
            Instantiate(powerUpPlatePrefab, transform.position, Quaternion.identity);
            spawned = true;
        }
    }
}
