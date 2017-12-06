﻿using UnityEngine;
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

	// La caméra
	public Camera cameraView;

    // Les voitures
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarWhite;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;

    // Les paramètres des voitures vertes
    private int nbGreen;
    private int nbGreenExit0, nbGreenExit1, nbGreenExit2, nbGreenExit3;
    // Les paramètres des voitures rouges
    private int nbRed;
    private int nbRedExit0, nbRedExit1, nbRedExit2, nbRedExit3;
    // Les paramètres des voitures bleues
    private int nbBlue;
    private int nbBlueTrust1, nbBlueTrust2, nbBlueTrust3, nbBlueTrust4, nbBlueTrust5;
    private int nbBlueGenerosity1, nbBlueGenerosity2, nbBlueGenerosity3, nbBlueGenerosity4, nbBlueGenerosity5;
    // Les paramètres des voitures orange
    private int nbOrange;
    private int nbOrangeTrust1, nbOrangeTrust2, nbOrangeTrust3, nbOrangeTrust4, nbOrangeTrust5;
    private int nbOrangeGenerosity1, nbOrangeGenerosity2, nbOrangeGenerosity3, nbOrangeGenerosity4, nbOrangeGenerosity5;
    // Les paramètres des voitures blanches
    private int nbWhite;
    private int nbWhiteTrust1, nbWhiteTrust2, nbWhiteTrust3, nbWhiteTrust4, nbWhiteTrust5;
    private int nbWhiteGenerosity1, nbWhiteGenerosity2, nbWhiteGenerosity3, nbWhiteGenerosity4, nbWhiteGenerosity5;    

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
    private Button playStopButton;
    private Button playRestartButton;
    // Cavenas de l'interface de fin
    private Canvas stopCanvas;

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

		// Initialisation de la caméra générale
		cameraView.enabled = true;
        
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
        playStopButton = playCanvas.GetComponentsInChildren<Button>()[2];
        playStopButton.onClick.AddListener(StopOnClick);
        playRestartButton = playCanvas.GetComponentsInChildren<Button>()[3];
        playRestartButton.onClick.AddListener(RestartOnClick);
        
        // Initialisation de l'interface de fin
        stopCanvas = this.GetComponentsInChildren<Canvas>()[2];
        stopCanvas.enabled = false;
    }

    /// <summary>
    /// Très clair !
    /// (Uniquement utilisée pour initialiser le nombre de voitures de chaque couleur pour l'instant)
    /// </summary>
    public void Update()
    {
        if (startCanvas.enabled)
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
                // Pour les voitures vertes
                nbGreen = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[0].text);
                nbGreenExit0 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[1].text);
                nbGreenExit1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[2].text);
                nbGreenExit2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[3].text);
                nbGreenExit3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[4].text);

                // Pour les voitures rouges
                nbRed = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[5].text);
                nbRedExit0 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[6].text);
                nbRedExit1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[7].text);
                nbRedExit2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[8].text);
                nbRedExit3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[9].text);

                // Pour les voitures bleues
                nbBlue = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[10].text);
                nbBlueTrust1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[11].text);
                nbBlueTrust2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[12].text);
                nbBlueTrust3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[13].text);
                nbBlueTrust4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[14].text);
                nbBlueTrust5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[15].text);
                nbBlueGenerosity1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[16].text);
                nbBlueGenerosity2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[17].text);
                nbBlueGenerosity3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[18].text);
                nbBlueGenerosity4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[19].text);
                nbBlueGenerosity5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[20].text);

                // Pour les voitures orange
                nbOrange = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[21].text);
                nbOrangeTrust1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[22].text);
                nbOrangeTrust2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[23].text);
                nbOrangeTrust3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[24].text);
                nbOrangeTrust4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[25].text);
                nbOrangeTrust5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[26].text);
                nbOrangeGenerosity1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[27].text);
                nbOrangeGenerosity2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[28].text);
                nbOrangeGenerosity3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[29].text);
                nbOrangeGenerosity4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[30].text);
                nbOrangeGenerosity5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[31].text);

                // Pour les voitures blanches
                nbWhite = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[32].text);
                nbWhiteTrust1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[33].text);
                nbWhiteTrust2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[34].text);
                nbWhiteTrust3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[35].text);
                nbWhiteTrust4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[36].text);
                nbWhiteTrust5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[37].text);
                nbWhiteGenerosity1 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[38].text);
                nbWhiteGenerosity2 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[39].text);
                nbWhiteGenerosity3 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[40].text);
                nbWhiteGenerosity4 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[41].text);
                nbWhiteGenerosity5 = Convert.ToInt32(startCanvas.GetComponentsInChildren<InputField>()[42].text);

                // Vérification que toutes les voitures intérieures peuvent avoir une position initiale et que l'utilisateur sait compter
                if (nbGreen + nbRed <= 116 &&
                    nbGreenExit0 + nbGreenExit1 + nbGreenExit2 + nbGreenExit3 == nbGreen &&
                    nbRedExit0 + nbRedExit1 + nbRedExit2 + nbRedExit3 == nbRed &&
                    nbBlueTrust1 + nbBlueTrust2 + nbBlueTrust3 + nbBlueTrust4 + nbBlueTrust5 == nbBlue &&
                    nbBlueGenerosity1 + nbBlueGenerosity2 + nbBlueGenerosity3 + nbBlueGenerosity4 + nbBlueGenerosity5 == nbBlue &&
                    nbOrangeTrust1 + nbOrangeTrust2 + nbOrangeTrust3 + nbOrangeTrust4 + nbOrangeTrust5 == nbOrange &&
                    nbOrangeGenerosity1 + nbOrangeGenerosity2 + nbOrangeGenerosity3 + nbOrangeGenerosity4 + nbOrangeGenerosity5 == nbOrange &&
                    nbWhiteTrust1 + nbWhiteTrust2 + nbWhiteTrust3 + nbWhiteTrust4 + nbWhiteTrust5 == nbWhite &&
                    nbWhiteGenerosity1 + nbWhiteGenerosity2 + nbWhiteGenerosity3 + nbWhiteGenerosity4 + nbWhiteGenerosity5 == nbWhite)
                {
                    startButton.enabled = true;
                }
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
    /// Méthode appelée quand on clique sur le bouton `Stop`
    /// </summary>
    private void StopOnClick()
    {
        Time.timeScale = 0;
        stopCanvas.enabled = true;
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
    IEnumerator AddExtColorCar(GameObject car, int nbCars, int numberExit, int nbTrust1, int nbTrust2, int nbTrust3, int nbTrust4, int nbTrust5, int nbGenerosity1, int nbGenerosity2, int nbGenerosity3, int nbGenerosity4, int nbGenerosity5)
    {
        // Liste des nbTrust
        List<int> nbTrust = new List<int> { nbTrust1, nbTrust2, nbTrust3, nbTrust4, nbTrust5 };
        // Liste des nbGenerosity
        List<int> nbGenerosity = new List<int> { nbGenerosity1, nbGenerosity2, nbGenerosity3, nbGenerosity4, nbGenerosity5 };
        
        for (int i = 0; i < nbCars; i++)
        {
            // La position initiale des voitures extérieures est toujours la position numéro 111
            Vector3 spawnPosition = new Vector3(nodes[111].position.x, 0, nodes[111].position.z);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, nodes[111].transform.eulerAngles.y, 0);
			GameObject newCar = Instantiate(car, spawnPosition, rotation);
            newCar.GetComponent<ExtCarController>().Exit = nodes[numberExit];
			newCar.GetComponent<ExtCarController>().Path = path;
            newCar.GetComponent<ExtCarController>().CameraView = cameraView;

            // TRUST
            // alea.Next(5) car la taille des listes Trust est de 5
            // On vérifie que le nombre de la liste est différent de 0
            int trust = alea.Next(5);
            while (nbTrust[trust] == 0)
            {
                trust = alea.Next(5);
            }
            // On associe le paramètre "Trust" à la voiture
            newCar.GetComponent<ExtCarController>().Trust = trust + 1;
            // On dés-incrémente les nombres dans la liste
            nbTrust[trust] -= 1;

            // GENEROSITY
            // alea.Next(5) car la taille des listes Generosity est de 5
            // On vérifie que le nombre de la liste est différent de 0
            int generosity = alea.Next(5);
            while (nbGenerosity[generosity] == 0)
            {
                generosity = alea.Next(5);
            }
            // On associe le paramètre "Generosity" à la voiture
            newCar.GetComponent<ExtCarController>().Generosity = generosity + 1;
            // On dés-incrémente le nombre dans la liste
            nbGenerosity[generosity] -= 1;
            
            yield return new WaitForSeconds(wait);
        }
    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures extérieures à intervalle de temps régulier
    /// </summary>
    IEnumerator AddExtCars()
    {
        StartCoroutine(AddExtColorCar(ExtCarBlue, nbBlue, 113, nbBlueTrust1, nbBlueTrust2, nbBlueTrust3, nbBlueTrust4, nbBlueTrust5, nbBlueGenerosity1, nbBlueGenerosity2, nbBlueGenerosity3, nbBlueGenerosity4, nbBlueGenerosity5));
        yield return new WaitForSeconds(nbBlue * wait);
        StartCoroutine(AddExtColorCar(ExtCarOrange, nbOrange, 115, nbOrangeTrust1, nbOrangeTrust2, nbOrangeTrust3, nbOrangeTrust4, nbOrangeTrust5, nbOrangeGenerosity1, nbOrangeGenerosity2, nbOrangeGenerosity3, nbOrangeGenerosity4, nbOrangeGenerosity5));
        yield return new WaitForSeconds(nbOrange * wait);
        StartCoroutine(AddExtColorCar(ExtCarWhite, nbWhite, 117, nbWhiteTrust1, nbWhiteTrust2, nbWhiteTrust3, nbWhiteTrust4, nbWhiteTrust5, nbWhiteGenerosity1, nbWhiteGenerosity2, nbWhiteGenerosity3, nbWhiteGenerosity4, nbWhiteGenerosity5));
        yield return new WaitForSeconds(nbWhite * wait);
    }

    /// <summary>
    /// Méthode pour ajouter les voitures intérieures d'une même couleur
    /// </summary>
    /// <param name="car">Préfab de la voiture intérieure</param>
    /// <param name="nbCars">Nombre de voitures à ajouter</param>
    /// <param name="sincerity">Honnête ou non</param>
    IEnumerator AddIntCar(GameObject car, int nbCars, bool sincerity, int nbExit0, int nbExit1, int nbExit2, int nbExit3)
    {
        // Liste des nbExit
        List<int> nbExit = new List<int> { nbExit0, nbExit1, nbExit2, nbExit3 };
        // Un petit incrément pour parcourir la liste
        int increment = 0;
        
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
                    if (positionTakenInt[j] == positionTakenIndex || positionTakenIndex == 111 || positionTakenIndex == 110)
                    {
                        positionNotTakenBool = false;
                    }
                }

                // Si la position n'est pas prise
                if (positionNotTakenBool)
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
            newCar.GetComponent<IntCarController>().Path = path;
            newCar.GetComponent<IntCarController>().CameraView = cameraView;

            // On vérifie que le nombre de la liste est différent de 0
            while (nbExit[increment] == 0)
            {
                increment += 1;
            }

            // Liste des sorties possibles
            List<int> exits = new List<int> { 113, 115, 117 };
            int exitKnown;
            // On crée la liste de sorties connues pour la voiture
            List<Position> exitsKnown = new List<Position>();
            for (int j = 0; j < increment; j++)
            {
                exitKnown = alea.Next(exits.Count);
                // On ajoute une sortie à la liste des sorties connues
                exitsKnown.Add(new Position(exits[exitKnown]));
                // On enlève l'élément de la liste des sorties possibles pour éviter de l'avoir en double dans la liste des sortie connues
                exits.RemoveAt(exitKnown);
            }
            // On dés-incrémente le nombre dans la liste
            nbExit[increment] -= 1;
            // On associe la liste à la voiture
            newCar.GetComponent<IntCarController>().ExitsKnown = exitsKnown;

            yield return new WaitForSeconds(0f);
        }

    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures (intérieures + extérieures) de la simulation
    /// </summary>
    IEnumerator AddCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen, true, nbGreenExit0, nbGreenExit1, nbGreenExit2, nbGreenExit3));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddIntCar(IntCarRed, nbRed, false, nbRedExit0, nbRedExit1, nbRedExit2, nbRedExit3));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddExtCars());
    }
}
