using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private Canvas startCanvas;
    private Button startButton;

    private Canvas playCanvas;
    private Button playInstructionsButton;
    private Button playBreakButton;
    private Button playRestartButton;
    private GameObject panelInstructions;

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

        startCanvas = this.GetComponentsInChildren<Canvas>()[0];
        startButton = startCanvas.GetComponentsInChildren<Button>()[0];
        startButton.onClick.AddListener(StartOnClick);

        playCanvas = this.GetComponentsInChildren<Canvas>()[1];
        playCanvas.enabled = false;
        playInstructionsButton = playCanvas.GetComponentsInChildren<Button>()[0];
        playInstructionsButton.onClick.AddListener(InstructionsOnClick);
        panelInstructions = GameObject.FindGameObjectWithTag("PanelInstructions");
        panelInstructions.SetActive(false);
        playBreakButton = playCanvas.GetComponentsInChildren<Button>()[1];
        playBreakButton.onClick.AddListener(BreakOnClick);
        playRestartButton = playCanvas.GetComponentsInChildren<Button>()[2];
        playRestartButton.onClick.AddListener(RestartOnClick);
    }

    public void Update()
    {
        bool ready = true;
        startButton.enabled = false;
        for (int i = 0; i < startCanvas.GetComponentsInChildren<InputField>().Length; i++)
        {
            if (startCanvas.GetComponentsInChildren<InputField>()[i].text == "")
            {
                ready = false;
            }
        }
        if (ready)
        {
            nbGreen = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[0].text);
            nbRed = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[1].text);
            nbBlue = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[2].text);
            nbOrange = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[3].text);
            nbWhite = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[4].text);
            if (nbGreen + nbRed <= 116)
            {
                startButton.enabled = true;
            }
        }
    }

    private void StartOnClick()
    {
        startCanvas.enabled = false;
        playCanvas.enabled = true;
        StartCoroutine(AddCars());
    }

    private void InstructionsOnClick()
    {
        if (panelInstructions.activeSelf)
        {
            panelInstructions.SetActive(false);
        }
        else
        {
            panelInstructions.SetActive(true);
        }
    }

    private void BreakOnClick()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            playBreakButton.GetComponentInChildren<Text>().text = "Reprendre";
        }
        else
        {

            Time.timeScale = 1;
            playBreakButton.GetComponentInChildren<Text>().text = "Pause";
        }
    }

    private void RestartOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
