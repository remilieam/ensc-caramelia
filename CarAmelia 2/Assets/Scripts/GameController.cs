using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // La map dans Unity
    public Transform path;
    private List<Transform> nodes = new List<Transform>(); // Liste des noeuds de la map de Unity

    // Chiffre aléatoire
    protected System.Random alea = new System.Random();

    // Les voitures
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarWhite;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;
    private int nbBlue, nbOrange, nbWhite, nbGreen, nbRed;
    private float wait = 5;

    // Liste des positions initiales des voitures intérieures
    private List<int> positionTakenInt = new List<int>(); 
    
    // Canevas de l'interface de départ
    private Canvas startCanvas;
    private Button startButton;
    // Cavenas de l'interface de simulation
    private Canvas playCanvas;
    private Button playInstructionsButton;
    private GameObject panelInstructions;
    private Button playBreakButton;
    private Button playRestartButton;

    /// <summary>
    /// "Constructeur" pour initialiser les paramètres de la simulation
    /// </summary>
    void Start()
    {
        // Création de la liste contenant les noeuds de la map
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        
        // Initialisation de l'interface de départ
        startCanvas = this.GetComponentsInChildren<Canvas>()[0];
        startButton = startCanvas.GetComponentsInChildren<Button>()[0];
        startButton.onClick.AddListener(StartOnClick);

        // Initialisation de l'interface de simulation
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

    /// <summary>
    /// Très clair !
    /// (Uniquement utilisée pour initialiser le nombre de voitures de chaque couleur pour l'instant)
    /// </summary>
    public void Update()
    {
        bool ready = true;
        startButton.enabled = false;

        // Vérification qu'aucun champs n'est vide
        for (int i = 0; i < startCanvas.GetComponentsInChildren<InputField>().Length; i++)
        {
            if (startCanvas.GetComponentsInChildren<InputField>()[i].text == "")
            {
                ready = false;
            }
        }

        // Initialisation des paramètres si tout est ok
        if (ready)
        {
            nbGreen = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[0].text);
            nbRed = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[1].text);
            nbBlue = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[2].text);
            nbOrange = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[3].text);
            nbWhite = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[4].text);

            // Vérification que toutes les voitures intérieures peuvent avoir une position initiale
            if (nbGreen + nbRed <= 116)
            {
                startButton.enabled = true;
            }
        }
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Commencer`
    /// </summary>
    private void StartOnClick()
    {
        startCanvas.enabled = false;
        playCanvas.enabled = true;
        StartCoroutine(AddCars());
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Consigne`
    /// </summary>
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

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Pause`
    /// </summary>
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

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Redémarrer`
    /// </summary>
    private void RestartOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Méthode pour ajouter les voitures extérieures d'une même couleur
    /// </summary>
    /// <param name="car">Préfab de la voiture extérieure</param>
    /// <param name="nbCars">Nombre de voitures à ajouter</param>
    /// <param name="numberExit">Numéro de la position de la sortie des voitures de même couleur</param>
    IEnumerator AddExtColorCar(GameObject car, int nbCars, int numberExit)
    {
        for (int i = 0; i < nbCars; i++)
        {
            // La position initiale des voitures extérieures est toujours la position numéro 111
            Vector3 spawnPosition = new Vector3(nodes[111].position.x, 0, nodes[111].position.z);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, nodes[111].transform.eulerAngles.y, 0);
			GameObject newCar = Instantiate(car, spawnPosition, rotation);
            newCar.GetComponent<ExtCarController>().Exit = nodes[numberExit];
            yield return new WaitForSeconds(wait);
        }
    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures extérieures à intervalle de temps régulier
    /// </summary>
    IEnumerator AddExtCars()
    {
        StartCoroutine(AddExtColorCar(ExtCarBlue, nbBlue, 113));
        yield return new WaitForSeconds(nbBlue * wait);
        StartCoroutine(AddExtColorCar(ExtCarOrange, nbOrange, 115));
        yield return new WaitForSeconds(nbOrange * wait);
        StartCoroutine(AddExtColorCar(ExtCarWhite, nbWhite, 117));
        yield return new WaitForSeconds(nbWhite * wait);
    }

    /// <summary>
    /// Méthode pour ajouter les voitures intérieures d'une même couleur
    /// </summary>
    /// <param name="car">Préfab de la voiture intérieure</param>
    /// <param name="nbCars">Nombre de voitures à ajouter</param>
    /// <param name="sincerity">Honnête ou non</param>
    IEnumerator AddIntCar(GameObject car, int nbCars, bool sincerity)
    {
        for (int i = 0; i < nbCars; i++)
        {
            // Numéro de la position initiale
            int positionTakenIndex = 0;
            // Le numéro de la position initiale peut-il être attribué à la voiture intérieure ?
            bool notFound = true;

            while (notFound)
            {
                // Choix aléatoire d'une position initiale
                positionTakenIndex = alea.Next(nodes.Count);

                // La position a-t-elle déjà été attribuée à une voiture intérieure ?
                bool positionNotTakenBool = true;

                // Parcours des positions déjà prises par les voitures intérieures
                for (int j = 0; j < positionTakenInt.Count; j++)
                {
                    // Si la position choisie aléatoirement est déjà occupée par une voiture intérieure ou à côté d'une voiture extérieure
                    if(positionTakenInt[j] == positionTakenIndex || positionTakenIndex == 111 || positionTakenIndex == 110)
                    {
                        positionNotTakenBool = false;
                    }
                }
                
                // Si la position n'est pas prise
                if(positionNotTakenBool)
                {
                    notFound = false;
                }

            }

            // Ajout de la position à la liste des positions initiales déjà prises
            positionTakenInt.Add(positionTakenIndex);

            // Initialisation de la position initiale de la voiture intérieure
            Transform node = nodes[positionTakenIndex];
            Vector3 spawnPosition = new Vector3(node.transform.position.x, 0, node.transform.position.z);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, node.transform.eulerAngles.y, 0);
            GameObject newCar = Instantiate(car, spawnPosition, rotation);
            newCar.GetComponent<IntCarController>().Sincerity = sincerity;
            yield return new WaitForSeconds(0f);
        }
    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures (intérieures + extérieures) de la simulation
    /// </summary>
    IEnumerator AddCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen, true));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddIntCar(IntCarRed, nbRed, false));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddExtCars());
    }
}
