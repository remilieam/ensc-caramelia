using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExtCarController : CarController {

    // Correspond aux noeuds qu'a parcourus la voiture
    private List<Position> crossedNodes;

    // Permet de définir si la voiture a un noeud objectif ou si elle roule aléatoirement dans la carte
    bool aleaMode;

    public Transform exit;

    // nodesToCross; : limité à 6 noeuds car la voiture a une petite mémoire
        
    public void Start() 
    {
        StartCar();

        // Voiture démarre à l'entrée de la map et de manière aléatoire
        position = new Position(111);
        aleaMode = true;

        // On définit la future position
        nextPosition = new Position(SuccessorAlea(position).Row);
        
        // On ajoute cette première position aux positions que la voiture a parcourues
        crossedNodes = new List<Position>();
        crossedNodes.Add(position);
        
    }

    // Permet d'actualiser le noeud
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
        if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_frein)
        {
            isBraking = true;
            if (Vector3.Distance(transform.position, nodes[nextPosition.Row].position) < distance_chgt)
            {
                // Si la voiture a atteint son objectif (même par hasard), elle a gagné ! (Et donc est détruite, c'est très logique.)
                if (nodes[nextPosition.Row] == exit)
                {
                    Destroy(gameObject);
                }

                position = nextPosition;
                crossedNodes.Add(position);

                if (aleaMode)
                {                    
                    nextPosition = SuccessorAlea(position);
                }
                else
                {
                    if (i < nodesToCross.Count)
                    {
                        nextPosition = nodesToCross[i].name;
                        i++;
                    }
                }                
            }
        }
        else
        {
            isBraking = false;

        }
    }

    public override void Stop (GameObject hitCar)
    {
        if(aleaMode)
        {
            if(hitCar.tag == "IntCar")
            {
                isBraking = true;
                
            }
            else if (hitCar.tag == "ExtCar")
            {
                isBraking = true;
            }
        }
    }        
}
