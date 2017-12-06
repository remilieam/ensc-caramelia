using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class IntCarController : CarController
{
    // Liste des positions des sorties connues
    private List<Position> exitsKnown;

    // `true` si la voiture est honnête (dit toujours la vérité lors des échanges d'informations)
    // et `false` sinon
    private bool sincerity;

    public List<Position> ExitsKnown
    {
        get { return exitsKnown; }
        set { exitsKnown = value; }
    }

    public bool Sincerity
    {
        get { return sincerity; }
        set { sincerity = value; }
    }

    /// <summary>
    /// "Constructeur" pour initialiser les paramètres de la voiture intérieure
    /// </summary>
    public void Start()
    {
        StartCar();

        // Récupération de sa position initiale
        float positionX = this.transform.position.x;
        float positionZ = this.transform.position.z;
        position = new Position(FindNode(positionX, positionZ));

        // Détermination aléatoire de sa position objectif
        target = new Position(alea.Next(nodesTable.GetLength(1)));
        // Vérification que la position objectif n'est pas sa position initiale
        while (target.SamePosition(position))
        {
            target = new Position(alea.Next(nodesTable.GetLength(1)));
        }
        // Calcul du chemin pour atteindre la position objectif
        PathCalculation();

        // Détermination de la prochaine position
        nextPosition = nodesToCross[indexNode].name;
    }

    /// <summary>
    /// C'est clair !
    /// </summary>
    public void Update()
    {
        SensorsObstacle();
        WriteInformation();
    }

    /// <summary>
    /// Méthode pour trouver le numéro du noeud qui correspond à la position de la voiture
    /// </summary>
    /// <param name="x">Coordonnées de la voiture sur l'axe x</param>
    /// <param name="z">Coordonnées de la voiture sur l'axe z</param>
    /// <returns></returns>
    private int FindNode(float x, float z)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (x == nodes[i].transform.position.x && z == nodes[i].transform.position.z)
            {
                return i;
            }
        }

        return 0;
    }

    /// <summary>
    /// Méthode pour actualiser la position de la voiture
    /// </summary>
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
        if (Vector3.Distance(transform.position, nodes[nextPosition.Number].position) < brakeDistance)
        {
            isBraking = true;

            // Dès que la voiture est assez proche de sa destination, on lui définit une nouvelle destination 
            if (Vector3.Distance(transform.position, nodes[nextPosition.Number].position) < changeNodeDistance)
            {
                position = nextPosition;

                // Cas où la voiture est toujours sur le chemin pour atteindre sa position objectif
                if (indexNode < nodesToCross.Count)
                {
                    // On définit la prochaine position que la voiture doit atteindre
                    nextPosition = nodesToCross[indexNode].name;
                    indexNode++;
                }

                // Cas où la voiture a atteint sa position objectif
                else
                {
                    indexNode = 1;
                    // On re définit une position objectif aléatoire
                    target = new Position(alea.Next(nodesTable.GetLength(1)));
                    PathCalculation();
                    // On définit la prochaine position que la voiture doit atteindre
                    nextPosition = nodesToCross[indexNode].name;
                    indexNode++;
                }
            }
        }
        else
        {
            isBraking = false;
        }
    }

    /// <summary>
    /// Méthode pour écrire les informations dans le canevas de la voiture
    /// </summary>
    public override void WriteInformation()
    {
        // Définition du texte à afficher dans le canevas (informations relatives à la voiture intérieure)
        string message = "";
        for (int j = 0; j < exitsKnown.Count; j++)
        {
            if (exitsKnown[j].Number == 113)
            {
                message += "\n- Sortie Bleue";
            }
            if (exitsKnown[j].Number == 115)
            {
                message += "\n- Sortie Orange";
            }
            if (exitsKnown[j].Number == 117)
            {
                message += "\n- Sortie Blanche";
            }
        }
        textCanvas.text = "Les sorties connues : " + message; // + "\nTarget : " + target.ToString() + ", Noeuds : " + nodesToCross.Count.ToString();
    }
}
