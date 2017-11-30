using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarWhite;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;
    private int nbBlue, nbOrange, nbWhite, nbGreen, nbRed;
    public Transform path;
    private float wait = 5;

    private List<int> positionTakenInt = new List<int>(); 

    private List<Transform> nodes;
    private System.Random alea;
    private Transform[] pathTransforms;

    private Canvas canvas;
    private Button startButton;

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

        canvas = this.GetComponentsInChildren<Canvas>()[0];
        startButton = canvas.GetComponentsInChildren<Button>()[0];
        startButton.onClick.AddListener(TaskOnClick);
    }

    protected void TaskOnClick()
    {
        try
        {
            Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[0].text);
            Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[1].text);
            Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[2].text);
            Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[3].text);
            Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[4].text);
        }
        catch (Exception e)
        {
            
        }
        nbGreen = Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[0].text);
        nbRed = Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[1].text);
        nbBlue = Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[2].text);
        nbOrange = Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[3].text);
        nbWhite = Convert.ToInt32(canvas.GetComponentsInChildren<InputField>()[4].text);

        canvas.enabled = false;

        StartCoroutine(AddCars());
    }

    IEnumerator AddExtColorCar(GameObject car, int nbCars)
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
        StartCoroutine(AddExtColorCar(ExtCarBlue, nbBlue));
        yield return new WaitForSeconds(nbBlue * wait);
        StartCoroutine(AddExtColorCar(ExtCarOrange, nbOrange));
        yield return new WaitForSeconds(nbOrange * wait);
        StartCoroutine(AddExtColorCar(ExtCarWhite, nbWhite));
        yield return new WaitForSeconds(nbWhite * wait);
    }

    IEnumerator AddIntCar(GameObject car, int nbCars)
    {
        
        Transform node;

        for (int i = 0; i < nbCars; i++)
        {
            int positionTakenIndex = 0;
            bool positionNotTakenBool = true;
            bool notFound = true;


            while (notFound)
            {
                positionTakenIndex = alea.Next(nodes.Count);
                positionNotTakenBool = true;

                // Pour chaque position des voitures intérieures
                for (int j = 0; j < positionTakenInt.Count; j++)
                {
                    // Si position choisie aléatoirement déjà occupée par une voiture intérieure ou extérieure ou à côté d'une voiture ext
                    if(positionTakenInt[j] == positionTakenIndex || positionTakenIndex == 111 || positionTakenIndex == 110)
                    {
                        positionNotTakenBool = false;
                    }
                }
                
                // Si position pas prise
                if(positionNotTakenBool)
                {
                    notFound = false;
                }

            }
            positionTakenInt.Add(positionTakenIndex);
            node = nodes[positionTakenIndex];
                
            Vector3 spawnPosition = new Vector3(node.transform.position.x, 0, node.transform.position.z);

            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, node.transform.eulerAngles.y, 0);
            Instantiate(car, spawnPosition, rotation);
            yield return new WaitForSeconds(0f);
        }
    }

    IEnumerator AddCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddIntCar(IntCarRed, nbRed));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddExtCars());
    }
}
