using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    public GameObject extCar;
    public float wait;
    public int nbCars = 4;

    void Start()
    {
        StartCoroutine(StartExtCar());
    }

    IEnumerator StartExtCar()
    {
        for (int i = 0; i < nbCars; i++)
        {
            Vector3 spawnPosition = new Vector3(-8.3f, 0, -122.7f);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(extCar, spawnPosition, spawnRotation);
            yield return new WaitForSeconds(wait);
        }
    }
}
