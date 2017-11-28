using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarYellow;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;
    public int nbBlue, nbOrange, nbYellow, nbGreen, nbRed;
    public Transform path;
    public float wait = 0;

    private List<Transform> nodes;
    private System.Random alea;
    private Transform[] pathTransforms;

    public Canvas canvas;
    private Text text;

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

        StartCoroutine(AddCars());
        text = canvas.GetComponentsInChildren<Text>()[0];
        text.text = nodes.Count.ToString();
    }

    IEnumerator AddExtCar(GameObject car, int nbCars)
    {
        for (int i = 0; i < nbCars; i++)
        {
            Vector3 spawnPosition = new Vector3(nodes[111].position.x, 0, nodes[111].position.z);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, nodes[111].transform.eulerAngles.y, 0);
            Instantiate(car, spawnPosition, rotation);
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
            node = nodes[alea.Next(nodes.Count)];
            Vector3 spawnPosition = new Vector3(node.transform.position.x, 0, node.transform.position.z);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, node.transform.eulerAngles.y, 0);
            Instantiate(car, spawnPosition, rotation);
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator AddCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen));
        yield return new WaitForSeconds(2 * wait);
        StartCoroutine(AddIntCar(IntCarRed, nbRed));
        yield return new WaitForSeconds(2 * wait);
        StartCoroutine(AddExtCars());
    }
}
