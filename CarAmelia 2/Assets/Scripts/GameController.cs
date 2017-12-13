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

    // La caméra générale
    public Camera cameraView;

    // Les voitures
    public GameObject ExtCarBlue;
    public GameObject ExtCarOrange;
    public GameObject ExtCarWhite;
    public GameObject IntCarGreen;
    public GameObject IntCarRed;
    private List<ExtCarController> listExtCars = new List<ExtCarController>();
    private List<IntCarController> listIntCars = new List<IntCarController>();

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
    private Button playDecButton;
    private Button playAccButton;
    private Text timeDisplay;
    private float startTime;
    private int speedGame;
    // Cavenas de l'interface de fin
    private Canvas stopCanvas;
    private Button stopRestartButton;

    /// <summary>
    /// "Constructeur" pour initialiser les paramètres de la simulation
    /// </summary>
    public void Start()
    {
        speedGame = 1;
        Time.timeScale = speedGame;

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
        playDecButton = playCanvas.GetComponentsInChildren<Button>()[4];
        playDecButton.onClick.AddListener(DecOnClick);
        playAccButton = playCanvas.GetComponentsInChildren<Button>()[5];
        playAccButton.onClick.AddListener(AccOnClick);
        timeDisplay = playCanvas.GetComponentInChildren<Text>();

        // Initialisation de l'interface de fin
        stopCanvas = this.GetComponentsInChildren<Canvas>()[2];
        stopCanvas.enabled = false;
        stopRestartButton = stopCanvas.GetComponentsInChildren<Button>()[0];
        stopRestartButton.onClick.AddListener(RestartOnClick);
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
            InputField[] listInputFields = startCanvas.GetComponentsInChildren<InputField>();
            GameObject[] listImages = GameObject.FindGameObjectsWithTag("Cross");
            listImages[0].GetComponent<Image>().enabled = false;
            listImages[1].GetComponent<Image>().enabled = false;
            listImages[2].GetComponent<Image>().enabled = false;

            // Vérification qu'aucun champs n'est vide
            for (int i = 0; i < listInputFields.Length; i++)
            {
                if (listInputFields[i].text == "")
                {
                    ready = false;
                }
            }

            // Initialisation des paramètres si tout est ok
            if (ready)
            {
                // Pour les voitures vertes
                listInputFields[0].enabled = false;
                nbGreenExit0 = Convert.ToInt32(listInputFields[1].text);
                nbGreenExit1 = Convert.ToInt32(listInputFields[2].text);
                nbGreenExit2 = Convert.ToInt32(listInputFields[3].text);
                nbGreenExit3 = Convert.ToInt32(listInputFields[4].text);
                nbGreen = nbGreenExit0 + nbGreenExit1 + nbGreenExit2 + nbGreenExit3;
                listInputFields[0].text = nbGreen.ToString();

                // Pour les voitures rouges
                listInputFields[5].enabled = false;
                nbRedExit0 = Convert.ToInt32(listInputFields[6].text);
                nbRedExit1 = Convert.ToInt32(listInputFields[7].text);
                nbRedExit2 = Convert.ToInt32(listInputFields[8].text);
                nbRedExit3 = Convert.ToInt32(listInputFields[9].text);
                nbRed = nbRedExit0 + nbRedExit1 + nbRedExit2 + nbRedExit3;
                listInputFields[5].text = nbRed.ToString();

                // Pour les voitures bleues
                listInputFields[10].enabled = false;
                nbBlueTrust1 = Convert.ToInt32(listInputFields[11].text);
                nbBlueTrust2 = Convert.ToInt32(listInputFields[12].text);
                nbBlueTrust3 = Convert.ToInt32(listInputFields[13].text);
                nbBlueTrust4 = Convert.ToInt32(listInputFields[14].text);
                nbBlueTrust5 = Convert.ToInt32(listInputFields[15].text);
                nbBlueGenerosity1 = Convert.ToInt32(listInputFields[16].text);
                nbBlueGenerosity2 = Convert.ToInt32(listInputFields[17].text);
                nbBlueGenerosity3 = Convert.ToInt32(listInputFields[18].text);
                nbBlueGenerosity4 = Convert.ToInt32(listInputFields[19].text);
                nbBlueGenerosity5 = Convert.ToInt32(listInputFields[20].text);
                if (nbBlueTrust1 + nbBlueTrust2 + nbBlueTrust3 + nbBlueTrust4 + nbBlueTrust5 != nbBlueGenerosity1 + nbBlueGenerosity2 + nbBlueGenerosity3 + nbBlueGenerosity4 + nbBlueGenerosity5)
                {
                    listImages[0].GetComponent<Image>().enabled = true;
                }
                else
                {
                    nbBlue = nbBlueTrust1 + nbBlueTrust2 + nbBlueTrust3 + nbBlueTrust4 + nbBlueTrust5;
                    listInputFields[10].text = nbBlue.ToString();
                }

                // Pour les voitures orange
                listInputFields[21].enabled = false;
                nbOrangeTrust1 = Convert.ToInt32(listInputFields[22].text);
                nbOrangeTrust2 = Convert.ToInt32(listInputFields[23].text);
                nbOrangeTrust3 = Convert.ToInt32(listInputFields[24].text);
                nbOrangeTrust4 = Convert.ToInt32(listInputFields[25].text);
                nbOrangeTrust5 = Convert.ToInt32(listInputFields[26].text);
                nbOrangeGenerosity1 = Convert.ToInt32(listInputFields[27].text);
                nbOrangeGenerosity2 = Convert.ToInt32(listInputFields[28].text);
                nbOrangeGenerosity3 = Convert.ToInt32(listInputFields[29].text);
                nbOrangeGenerosity4 = Convert.ToInt32(listInputFields[30].text);
                nbOrangeGenerosity5 = Convert.ToInt32(listInputFields[31].text);
                if (nbOrangeTrust1 + nbOrangeTrust2 + nbOrangeTrust3 + nbOrangeTrust4 + nbOrangeTrust5 != nbOrangeGenerosity1 + nbOrangeGenerosity2 + nbOrangeGenerosity3 + nbOrangeGenerosity4 + nbOrangeGenerosity5)
                {
                    listImages[1].GetComponent<Image>().enabled = true;
                }
                else
                {
                    nbOrange = nbOrangeTrust1 + nbOrangeTrust2 + nbOrangeTrust3 + nbOrangeTrust4 + nbOrangeTrust5;
                    listInputFields[21].text = nbOrange.ToString();
                }

                // Pour les voitures blanches
                listInputFields[32].enabled = false;
                nbWhiteTrust1 = Convert.ToInt32(listInputFields[33].text);
                nbWhiteTrust2 = Convert.ToInt32(listInputFields[34].text);
                nbWhiteTrust3 = Convert.ToInt32(listInputFields[35].text);
                nbWhiteTrust4 = Convert.ToInt32(listInputFields[36].text);
                nbWhiteTrust5 = Convert.ToInt32(listInputFields[37].text);
                nbWhiteGenerosity1 = Convert.ToInt32(listInputFields[38].text);
                nbWhiteGenerosity2 = Convert.ToInt32(listInputFields[39].text);
                nbWhiteGenerosity3 = Convert.ToInt32(listInputFields[40].text);
                nbWhiteGenerosity4 = Convert.ToInt32(listInputFields[41].text);
                nbWhiteGenerosity5 = Convert.ToInt32(listInputFields[42].text);
                if (nbWhiteTrust1 + nbWhiteTrust2 + nbWhiteTrust3 + nbWhiteTrust4 + nbWhiteTrust5 != nbWhiteGenerosity1 + nbWhiteGenerosity2 + nbWhiteGenerosity3 + nbWhiteGenerosity4 + nbWhiteGenerosity5)
                {
                    listImages[2].GetComponent<Image>().enabled = true;
                }
                else
                {
                    nbWhite = nbWhiteTrust1 + nbWhiteTrust2 + nbWhiteTrust3 + nbWhiteTrust4 + nbWhiteTrust5;
                    listInputFields[32].text = nbWhite.ToString();
                }

                // Vérification que toutes les voitures intérieures peuvent avoir une position initiale et que l'utilisateur sait compter
                if (nbGreen + nbRed <= 114 &&
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

        if (playCanvas.enabled)
        {
            float currentTime = Time.time - startTime;
            string minutes = ((int)currentTime / 60).ToString("00");
            string seconds = ((int)currentTime % 60).ToString("00");
            timeDisplay.text = minutes + ":" + seconds;

            // Vérification que le jeu est fini
            bool notFinished = false;
            foreach (ExtCarController extCar in listExtCars)
            {
                // Si une des voitures extérieures n'a pas atteint sa sortie, le jeu n'est pas fini
                if (!extCar.Finish)
                {
                    notFinished = true;
                }
            }

            // Cas où il n'y a que des voitures intérieures ==> on continue la simulation
            if (listExtCars.Count == 0)
            {
                notFinished = true;
            }

            // Si toutes le jeu est fini
            if (!notFinished)
            {
                DisplayEnd();
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
        startTime = Time.time;
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
        if (Time.timeScale != 0)
        {
            playDecButton.enabled = false;
            playAccButton.enabled = false;
            Time.timeScale = 0;
            playBreakButton.GetComponentInChildren<Text>().text = "Reprendre";
        }
        else
        {
            playDecButton.enabled = true;
            playAccButton.enabled = true;
            Time.timeScale = speedGame;
            playBreakButton.GetComponentInChildren<Text>().text = "Pause";
        }
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Stop`
    /// </summary>
    private void StopOnClick()
    {
        DisplayEnd();
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Redémarrer`
    /// </summary>
    private void RestartOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Ralentir` (on ne peut pas ralentir au-dessous de 1 ==> pause)
    /// </summary>
    private void DecOnClick()
    {
        speedGame -= 1;
        if(speedGame < 1)
        {
            speedGame = 1;
        }
        Time.timeScale = speedGame;
    }

    /// <summary>
    /// Méthode appelée quand on clique sur le bouton `Accélérer`
    /// </summary>
    private void AccOnClick()
    {
        speedGame += 1;
        Time.timeScale = speedGame;
    }

    /// <summary>
    /// Méthode pour ajouter les voitures extérieures d'une même couleur
    /// </summary>
    /// <param name="car">Préfab de la voiture extérieure</param>
    /// <param name="nbCars">Nombre de voitures à ajouter</param>
    /// <param name="numberExit">Numéro de la position de la sortie des voitures de même couleur</param>
    private IEnumerator AddExtColorCar(GameObject car, int nbCars, int numberExit, int nbTrust1, int nbTrust2, int nbTrust3, int nbTrust4, int nbTrust5, int nbGenerosity1, int nbGenerosity2, int nbGenerosity3, int nbGenerosity4, int nbGenerosity5)
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
            ExtCarController extCar = newCar.GetComponent<ExtCarController>();
            extCar.Exit = nodes[numberExit];
            extCar.Path = path;
            extCar.CameraView = cameraView;

            // TRUST
            // alea.Next(5) car la taille des listes Trust est de 5
            // On vérifie que le nombre de la liste est différent de 0
            int trust = alea.Next(5);
            while (nbTrust[trust] == 0)
            {
                trust = alea.Next(5);
            }
            // On associe le paramètre "Trust" à la voiture
            extCar.Trust = trust + 1;
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
            extCar.Generosity = generosity + 1;
            // On dés-incrémente le nombre dans la liste
            nbGenerosity[generosity] -= 1;

            listExtCars.Add(extCar);

            yield return new WaitForSeconds(wait);
        }
    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures extérieures à intervalle de temps régulier
    /// </summary>
    private IEnumerator AddExtCars()
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
    private IEnumerator AddIntCar(GameObject car, int nbCars, bool sincerity, int nbExit0, int nbExit1, int nbExit2, int nbExit3)
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
                    if (positionTakenInt[j] == positionTakenIndex || positionTakenIndex == 111 || positionTakenIndex == 110 || positionTakenIndex == 97 || positionTakenIndex == 96)
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
            IntCarController intCar = newCar.GetComponent<IntCarController>();
            intCar.Sincerity = sincerity;
            intCar.Path = path;
            intCar.CameraView = cameraView;

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
            intCar.ExitsKnown = exitsKnown;

            listIntCars.Add(intCar);

            yield return new WaitForSeconds(0f);
        }
    }

    /// <summary>
    /// Méthode pour ajouter toutes les voitures (intérieures + extérieures) de la simulation
    /// </summary>
    private IEnumerator AddCars()
    {
        StartCoroutine(AddIntCar(IntCarGreen, nbGreen, true, nbGreenExit0, nbGreenExit1, nbGreenExit2, nbGreenExit3));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddIntCar(IntCarRed, nbRed, false, nbRedExit0, nbRedExit1, nbRedExit2, nbRedExit3));
        yield return new WaitForSeconds(0f);
        StartCoroutine(AddExtCars());
    }

    /// <summary>
    /// Méthode qui affiche l'interface de fin
    /// </summary>
    private void DisplayEnd()
    {
        // On arrête le temps
        Time.timeScale = 0;
        playCanvas.enabled = false;
        cameraView.enabled = true;

        // Récupération des textes
        Text[] textStopCanvas = stopCanvas.GetComponentsInChildren<Text>();

        // Pour les voitures intérieures vertes
        textStopCanvas[2].text += nbGreen.ToString();
        textStopCanvas[3].text += CalculationProperty(0).ToString("0.00");

        // Pour les voitures intérieures rouges
        textStopCanvas[4].text += nbRed.ToString();
        textStopCanvas[5].text += CalculationProperty(1).ToString("0.00");

        // Pour les voitures extérieures bleues
        textStopCanvas[7].text += SuccessCars(0).ToString() + " sur " + nbBlue.ToString();
        textStopCanvas[8].text += CalculationProperty(2).ToString("0.00");
        textStopCanvas[9].text += CalculationProperty(3).ToString("0.00");
        textStopCanvas[11].text += ((int)CalculationProperty(4) / 60).ToString("00") + ":" + ((int)CalculationProperty(4) % 60).ToString("00");
        textStopCanvas[12].text += ((int)CalculationProperty(5) / 60).ToString("00") + ":" + ((int)CalculationProperty(5) % 60).ToString("00");
        textStopCanvas[13].text += ((int)CalculationProperty(6) / 60).ToString("00") + ":" + ((int)CalculationProperty(6) % 60).ToString("00");
        textStopCanvas[15].text += ExtCarController.nbSuccessExchangeInt[0].ToString() + " sur " + ExtCarController.nbExchangeInt[0].ToString();
        textStopCanvas[16].text += ExtCarController.nbSuccessExchangeExt[0].ToString() + " sur " + ExtCarController.nbExchangeExt[0].ToString();

        // Pour les voitures extérieures orange
        textStopCanvas[18].text += SuccessCars(1).ToString() + " sur " + nbOrange.ToString();
        textStopCanvas[19].text += CalculationProperty(7).ToString("0.00");
        textStopCanvas[20].text += CalculationProperty(8).ToString("0.00");
        textStopCanvas[22].text += ((int)CalculationProperty(9) / 60).ToString("00") + ":" + ((int)CalculationProperty(9) % 60).ToString("00");
        textStopCanvas[23].text += ((int)CalculationProperty(10) / 60).ToString("00") + ":" + ((int)CalculationProperty(10) % 60).ToString("00");
        textStopCanvas[24].text += ((int)CalculationProperty(11) / 60).ToString("00") + ":" + ((int)CalculationProperty(11) % 60).ToString("00");
        textStopCanvas[26].text += ExtCarController.nbSuccessExchangeInt[1].ToString() + " sur " + ExtCarController.nbExchangeInt[1].ToString();
        textStopCanvas[27].text += ExtCarController.nbSuccessExchangeExt[1].ToString() + " sur " + ExtCarController.nbExchangeExt[1].ToString();

        // Pour les voitures extérieures blanches
        textStopCanvas[29].text += SuccessCars(2).ToString() + " sur " + nbWhite.ToString();
        textStopCanvas[30].text += CalculationProperty(12).ToString("0.00");
        textStopCanvas[31].text += CalculationProperty(13).ToString("0.00");
        textStopCanvas[33].text += ((int)CalculationProperty(14) / 60).ToString("00") + ":" + ((int)CalculationProperty(14) % 60).ToString("00");
        textStopCanvas[34].text += ((int)CalculationProperty(15) / 60).ToString("00") + ":" + ((int)CalculationProperty(15) % 60).ToString("00");
        textStopCanvas[35].text += ((int)CalculationProperty(16) / 60).ToString("00") + ":" + ((int)CalculationProperty(16) % 60).ToString("00");
        textStopCanvas[37].text += ExtCarController.nbSuccessExchangeInt[2].ToString() + " sur " + ExtCarController.nbExchangeInt[2].ToString();
        textStopCanvas[38].text += ExtCarController.nbSuccessExchangeExt[2].ToString() + " sur " + ExtCarController.nbExchangeExt[2].ToString();

        stopCanvas.enabled = true;
    }

    /// <summary>
    /// Méthode pour calculer des valeurs de propriété sur les voitures 
    /// </summary>
    /// <param name="property">0 pour les sorties connues par les voitures sincères, 1 pour pour les sorties connues les non-sincères, 
    /// 2 pour la générosité des voitures bleues, 3 pour la confiance des voitures bleues, 4 pour temps min des voitures bleues, 5 pour temps max des voitures bleues, 6 pour le temps des voitures bleues,
    /// 7 pour la générosité des voitures orange, 8 pour la confiance des voitures orange, 9 pour temps min des voitures orange, 10 pour temps max des voitures orange, 11 pour le temps des voitures orange,
    /// 12 pour la générosité des voitures blanches, 13 pour la confiance des voitures blanches, 14 pour temps min des voitures blanches, 15 pour temps max des voitures blanches, 16 pour le temps des voitures blanches
    /// </param>
    /// <returns>La valeur de la propriété qui nous intéresse</returns>
    private float CalculationProperty(int property)
    {
        float val = 0;

        // Pour les voitures sincères
        if (property == 0 && nbGreen != 0)
        {
            for (int i = 0; i < listIntCars.Count; i++)
            {
                if (listIntCars[i].Sincerity)
                {
                    val += listIntCars[i].ExitsKnown.Count;
                }
            }
            return val / nbGreen * 1.0f;
        }

        // Pour les voitures non-sincères
        else if (property == 1 && nbRed != 0)
        {
            for (int i = 0; i < listIntCars.Count; i++)
            {
                if (!listIntCars[i].Sincerity)
                {
                    val += listIntCars[i].ExitsKnown.Count;
                }
            }
            return val / nbRed * 1.0f;
        }

        // Pour les voitures bleues - Générosité
        else if (property == 2 && nbBlue != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[113])
                {
                    val += extCar.Generosity;
                }
            }
            return val / nbBlue * 1.0f;
        }

        // Pour les voitures bleues - Confiance
        else if (property == 3 && nbBlue != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[113])
                {
                    val += extCar.Trust;
                }
            }
            return val / nbBlue * 1.0f;
        }

        // Pour les voitures bleues - Temps min
        else if (property == 4)
        {
            // Initialisation de la valeur minimale
            if (nbBlue != 0)
            {
                int i = 0;
                while (i < listExtCars.Count && (listExtCars[i].Exit != nodes[113] || !listExtCars[i].Finish))
                {
                    i++;
                }
                if (i != listExtCars.Count)
                {
                    val = listExtCars[i].ExitTime - startTime;
                }
            }

            // Parcours des voitures bleues
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[113] && extCar.Finish && val > extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures bleues - Temps max
        else if (property == 5)
        {
            // Parcours des voitures bleues
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[113] && val < extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures bleues - Temps
        else if (property == 6 && nbBlue != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[113] && extCar.Finish)
                {
                    val += extCar.ExitTime - startTime;
                }
            }
            if (SuccessCars(0) != 0)
            {
                return val / SuccessCars(0) * 1.0f;
            }
        }

        // Pour les voitures orange - Générosité
        else if (property == 7 && nbOrange != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[115])
                {
                    val += extCar.Generosity;
                }
            }
            return val / nbOrange * 1.0f;
        }

        // Pour les voitures orange - Confiance
        else if (property == 8 && nbOrange != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[115])
                {
                    val += extCar.Trust;
                }
            }
            return val / nbOrange * 1.0f;
        }

        // Pour les voitures orange - Temps min
        else if (property == 9)
        {
            // Initialisation de la valeur minimale
            if (nbOrange != 0)
            {
                int i = 0;
                while (i < listExtCars.Count && (listExtCars[i].Exit != nodes[115] || !listExtCars[i].Finish))
                {
                    i++;
                }
                if (i != listExtCars.Count)
                {
                    val = listExtCars[i].ExitTime - startTime;
                }
            }

            // Parcours des voitures orange
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[115] && extCar.Finish && val > extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures orange - Temps max
        else if (property == 10)
        {
            // Parcours des voitures orange
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[115] && val < extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures orange - Temps
        else if (property == 11 && nbOrange != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[115] && extCar.Finish)
                {
                    val += extCar.ExitTime - startTime;
                }
            }
            if (SuccessCars(1) != 0)
            {
                return val / SuccessCars(1) * 1.0f;
            }
        }

        // Pour les voitures blanches - Générosité
        else if (property == 12 && nbWhite != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[117])
                {
                    val += extCar.Generosity;
                }
            }
            return val / nbWhite * 1.0f;
        }

        // Pour les voitures blanches - Confiance
        else if (property == 13 && nbWhite != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[117])
                {
                    val += extCar.Trust;
                }
            }
            return val / nbWhite * 1.0f;
        }

        // Pour les voitures blanches - Temps min
        else if (property == 14)
        {
            // Initialisation de la valeur minimale
            if (nbWhite != 0)
            {
                int i = 0;
                while (i < listExtCars.Count && (listExtCars[i].Exit != nodes[117] || !listExtCars[i].Finish))
                {
                    i++;
                }
                if (i != listExtCars.Count)
                {
                    val = listExtCars[i].ExitTime - startTime;
                }
            }

            // Parcours des voitures blanches
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[117] && extCar.Finish && val > extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures blanches - Temps max
        else if (property == 15)
        {
            // Parcours des voitures blanches
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[117] && val < extCar.ExitTime - startTime)
                {
                    val = extCar.ExitTime - startTime;
                }
            }
            return val;
        }

        // Pour les voitures blanches - Temps
        else if (property == 16 && nbWhite != 0)
        {
            foreach (ExtCarController extCar in listExtCars)
            {
                if (extCar.Exit == nodes[117] && extCar.Finish)
                {
                    val += extCar.ExitTime - startTime;
                }
            }
            if (SuccessCars(2) != 0)
            {
                return val / SuccessCars(2) * 1.0f;
            }
        }
        return val;
    }

    /// <summary>
    /// Méthode qui renvoie le nombre de voitures extérieures d'une certaine couleur ayant trouvé leur sortie
    /// </summary>
    /// <param name="property">0 pour les voitures bleues, 1 pour les voitures orange, 2 pour les voitures blanches</param>
    /// <returns>Le nombre de voitures ayant trouvé leur sortie</returns>
    private int SuccessCars(int property)
    {
        int nbSuccess = 0;

        foreach (ExtCarController extCar in listExtCars)
        {
            // Pour les voitures bleues
            if (property == 0 && extCar.Exit == nodes[113] && extCar.Finish)
            {
                nbSuccess += 1;
            }

            // Pour les voitures orange
            if (property == 1 && extCar.Exit == nodes[115] && extCar.Finish)
            {
                nbSuccess += 1;
            }

            // Pour les voitures blanches
            if (property == 2 && extCar.Exit == nodes[117] && extCar.Finish)
            {
                nbSuccess += 1;
            }
        }

        return nbSuccess;
    }
}