using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExtCarController : CarController {

    // Correspond aux noeuds qu'a parcourus la voiture
    private List<Position> crossedNodes;
	// Correspond à son degré de confiance (1 pas confiant dans les infos qu'il reçoit
	// et 5 confiance aveugle) et de générosité (1 pas généreux dans le partage
	// d'informations avec une voiture de même couleur et 5 très généreux)
	public int trust;
	public int generosity;

    // Permet de définir si la voiture a un noeud objectif ou si elle roule aléatoirement dans la carte
    bool aleaMode;

    public Transform exit;

    // nodesToCross; : limité à 6 noeuds car la voiture a une petite mémoire
        
    public void Start()
	{
		StartCar ();

		// Voiture démarre à l'entrée de la map et de manière aléatoire
		position = new Position (111);
		aleaMode = true;

		// On définit la future position
		nextPosition = new Position (SuccessorAlea (position).Row);
        
		// On ajoute cette première position aux positions que la voiture a parcourues
		crossedNodes = new List<Position> ();
		crossedNodes.Add (position);

		// Confiance & générosité
		trust = alea.Next(1,6);
		generosity = alea.Next (1, 6);
	}

    // Permet d'actualiser le noeud
    protected override void CheckWaypoint()
    {
        // On fait freiner la voiture avant l'arrivée sur le point
		if (Vector3.Distance (transform.position, nodes [nextPosition.Row].position) < distance_frein) {
			isBraking = true;
			if (Vector3.Distance (transform.position, nodes [nextPosition.Row].position) < distance_chgt) {
				// Si la voiture a atteint son objectif (même par hasard), elle a gagné ! (Et donc est détruite, c'est très logique.)
				if (nodes [nextPosition.Row] == exit) {
					Destroy (gameObject);
				}

				position = nextPosition;
				crossedNodes.Add (position);

				if (aleaMode) {                    
					nextPosition = SuccessorAlea (position);
				} else {
					if (indexNode < nodesToCross.Count) {
						nextPosition = nodesToCross [indexNode].name;
						indexNode++;
					}
				}                
			}
		} else {
			isBraking = false;

		}
    }

	private void ReceivingInformation(CarController car)
	{
		if (car is ExtCarController) {
			ExtCarController extCar = (ExtCarController)car;
			int prob = extCar.alea.Next (1, 5);
			if (prob >= extCar.generosity && !extCar.aleaMode)
			{
				// Je reçois de l'info que si la voiture extérieure que je crois
				// a assez généreuse et n'est pas en mode aléatoire
				this.target = car.target;
			}
		}
		if (car is IntCarController)
		{
			IntCarController intCar = (IntCarController)car;
			if (intCar.sincerity)
			{
				foreach (Position exitKnown in intCar.exitsKnown)
				{
					if (nodes [exitKnown.Row] == exit)
					{
						Position exitPosition = new Position(0);
						for (int i = 0; i < nodes.Count; i++) {
							if (nodes [i] == exit) {
								exitPosition = new Position (i);
							}
						}
						target = exitPosition;
					}
				}
			}
		}
	}

	public override void Stop (GameObject hitCar)
    {
		ExtCarController extCarHit =hitCar.gameObject.GetComponent<ExtCarController> ();
		IntCarController intCarHit =hitCar.gameObject.GetComponent<IntCarController> ();

		// Pas de target
		if (aleaMode)
		{
			if (hitCar.tag == "IntCar")
			{
				isBraking = true;
				intCarHit.isBraking = true;
			}
			else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit) {
				isBraking = true;
				extCarHit.isBraking = true;
				ReceivingInformation (extCarHit);
			}
		}

		// A une target
		else
		{
			int prob = alea.Next (1, 5);
			if (prob >= trust) 
			{
				// Réception d'infos ==> pas confiance
				if (hitCar.tag == "IntCar")
				{
					isBraking = true;
				}
				else if (hitCar.tag == "ExtCar" && extCarHit.exit == this.exit)
				{
					isBraking = true;
					extCarHit.isBraking = true;
					ReceivingInformation (extCarHit);
				}
			}
			else
			{
				// Pas de réception d'infos ==> confiance
			}
		}
    }





}
