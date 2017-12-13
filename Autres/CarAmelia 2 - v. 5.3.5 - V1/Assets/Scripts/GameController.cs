using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarYellow;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;
    public int nbBlue, nbOrange, nbYellow, nbGreen, nbRed;
    public Transform path;
    public float wait;

    private new List<Transform> nodes;
    private System.Random alea;
    private Transform[] pathTransforms;

    void Start()
    {
        // Récupération des noeuds
        pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        alea = new System.Random();

        StartCoroutine(AddExtCars());
        StartCoroutine(AddIntCars());
    }

    IEnumerator AddExtCar(GameObject car, int nbCars)
    {
        for (int i = 0; i < nbCars; i++)
        {
            Vector3 spawnPosition = new Vector3(-8.3f, 0, -122.7f);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(car, spawnPosition, spawnRotation);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator AddExtCars()
    {
        StartCoroutine(AddExtCar(ExtCarBlue, nbBlue));
        yield return new WaitForSeconds(2 * wait);
        StartCoroutine(AddExtCar(ExtCarOrange, nbOrange));
        yield return new WaitForSeconds(2 * wait);
        StartCoroutine(AddExtCar(ExtCarYellow, nbYellow));
        yield return new WaitForSeconds(2 * wait);
    }

    IEnumerator AddIntCar(GameObject car, int nbCars)
    {
        Transform node;
        for (int i = 0; i < nbCars; i++)
        {
            node = nodes[alea.Next(pathTransforms.Length)];
            Vector3 spawnPosition = new Vector3(node.transform.position.x, 0, node.transform.position.z);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(car, spawnPosition, spawnRotation);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator AddIntCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen));
        yield return new WaitForSeconds(2 * wait);
        StartCoroutine(AddIntCar(IntCarRed, nbRed));
        yield return new WaitForSeconds(2 * wait);
    }
}
