using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Graph
{
    public List<Node> openNodes;
    public List<Node> closeNodes;

    /// <summary>
    /// Permet de compter le nombre de nœuds ouverts
    /// </summary>
    /// <returns>Le nombre de nœuds ouverts</returns>
    public int CountOpenNodes()
    {
        return openNodes.Count;
    }

    /// <summary>
    /// Permet de compter le nombre de nœuds fermés
    /// </summary>
    /// <returns>Le nombre de nœuds fermés</returns>
    public int CountCloseNodes()
    {
        return closeNodes.Count;
    }

    /// <summary>
    /// Permet de savoir si un nœud est parmi les nœuds fermés
    /// </summary>
    /// <param name="searchNode">Nœud à rechercher parmi les nœuds fermés</param>
    /// <returns>Nœud s’il est dans les nœuds fermés</returns>
    private Node FindInCloseNodes(Node searchNode)
    {
        int i = 0;
        while (i < closeNodes.Count)
        {
            if (closeNodes[i].IsTheSame(searchNode))
                return closeNodes[i];
            i++;
        }
        return null;
    }

    /// <summary>
    /// Permet de savoir si un nœud est parmi les nœuds ouverts
    /// </summary>
    /// <param name="searchNode">Nœud à rechercher parmi les nœuds ouverts</param>
    /// <returns>Nœud s’il est dans les nœuds ouverts</returns>
    private Node FindInOpenNodes(Node searchNode)
    {
        int i = 0;
        while (i < openNodes.Count)
        {
            if (openNodes[i].IsTheSame(searchNode))
                return openNodes[i];
            i++;
        }
        return null;
    }

    /// <summary>
    /// Permet de rechercher le plus court chemin
    /// </summary>
    /// <param name="initialNode">Nœud de départ</param>
    /// <param name="xFinal">Ligne du nœud d’arrivée</param>
    /// <param name="yFinal">Colonne du nœud d’arrivée</param>
    /// <returns>Liste des nœuds pour aller du départ à l’arrivée</returns>
    public List<Node> FindPath(Node initialNode)
    {
        openNodes = new List<Node>();
        closeNodes = new List<Node>();

        // Le premier nœud évalué est le nœud initial
        Node evaluateNode = initialNode;

        // On ajoute le nœud de départ aux ouverts
        openNodes.Add(initialNode);

        // Tant que le nœud n’est pas terminal
        // et que la liste des ouverts n’est pas vide
        while (openNodes.Count != 0 && evaluateNode.CheckEnd() == false)
        {
            // Le meilleur nœud des ouverts est supposé être placé
            // en tête de liste des fermés
            openNodes.Remove(evaluateNode);
            closeNodes.Add(evaluateNode);

            // Il faut trouver les nœuds successeurs
            this.UpdateSuccessors(evaluateNode);
            // Inutile de retrier car les insertions ont été faites en respectant l’ordre

            // On prend le meilleur, donc celui en position 0, pour continuer
            // à explorer les états, à condition qu’il existe bien sûr
            if (openNodes.Count > 0)
            {
                evaluateNode = openNodes[0];
            }

            else
            {
                evaluateNode = null;
            }
        }

        // A* terminé
        // On retourne le chemin qui va du nœud initial au nœud final sous forme de liste
        // Le chemin est retrouvé en partant du nœud final et en accédant aux parents de manière
        // itérative jusqu’à ce qu’on tombe sur le nœud initial
        List<Node> path = new List<Node>();
        if (evaluateNode != null)
        {
            path.Add(evaluateNode);

            while (evaluateNode != initialNode)
            {
                evaluateNode = evaluateNode.Parent();
                path.Insert(0, evaluateNode);  // On insère en position 1
            }
        }

        return path;
    }

    /// <summary>
    /// Permet de trouver les successeurs du dernier nœud fermé
    /// </summary>
    /// <param name="evaluateNode">Nœud dont on cherche les successeurs</param>
    private void UpdateSuccessors(Node evaluateNode)
    {
        // On récupère les successeurs du nœud évalué
        List<Node> listSuccessors = evaluateNode.GetSuccessors();

        // Pour chaque nœud de la liste des successeurs du nœud évalué
        foreach (Node successorNode in listSuccessors)
        {
            // On vérifie s’il n’est pas une copie d’un nœud déjà vu
            // et placé dans la liste des fermés
            Node findNode = FindInCloseNodes(successorNode);

            // Cas où le successeur n’est pas dans les nœuds fermés
            if (findNode == null)
            {
                // On vérifie également s’il n’est pas une copie d’un nœud déjà vu
                // et placé dans la liste des ouverts
                findNode = FindInOpenNodes(successorNode);

                // Cas où le successeur est dans les nœuds ouverts
                if (findNode != null)
                {
                    // Si le nouveau chemin passant par evaluateNode est meilleur
                    // (pour cela on ajoute au coût du nœud évalué le coût pour aller au prochain nœud)
                    if (evaluateNode.GCost + evaluateNode.GetCost(successorNode) < findNode.GCost)
                    {
                        // Mise à jour du nœud trouvé
                        findNode.GCost = evaluateNode.GCost + evaluateNode.GetCost(successorNode);

                        // HCost pas recalculé car toujours bon
                        findNode.EvaluateTotalCost();

                        // Mise à jour de la famille...
                        findNode.RemoveRelationsParentChildren();
                        findNode.Parent(evaluateNode);

                        // Mise à jour des ouverts
                        openNodes.Remove(findNode);
                        this.AddInOpenNodes(findNode);
                    }

                    // Sinon on ne fait rien, car le nouveau chemin est moins bon
                }

                // Cas où le successeur n’est pas dans les nœuds ouverts.
                else
                {
                    // Le nœud est nouveau. Il faut mettre à jour le nœud et l’insérer dans la liste des ouverts
                    successorNode.GCost = evaluateNode.GCost + evaluateNode.GetCost(successorNode);
                    successorNode.EvaluateHCost();
                    successorNode.Parent(evaluateNode);
                    successorNode.EvaluateTotalCost();
                    this.AddInOpenNodes(successorNode);
                }
            }

            // Cas où le successeur est dans les nœuds fermés : on ne fait rien,
            // car on a déjà trouvé le plus court chemin pour aller vers du nœud évalué à son successeur
        }
    }

    /// <summary>
    /// Permet d’insérer un nœud dans la liste des nœuds ouverts sachant que cette liste
    /// est triée par ordre croissant du coût total des nœuds
    /// </summary>
    /// <param name="newNode">Nœud à insérer dans la liste des ouverts</param>
    public void AddInOpenNodes(Node newNode)
    {
        // Cas où il n’y a pas encore de nœuds ouverts
        if (this.openNodes.Count == 0)
        {
            openNodes.Add(newNode);
        }

        // Cas où il y a déjà des nœuds ouverts
        else
        {
            // Récupération du premier nœud ouvert (celui ayant le plus petit coût)
            Node evaluateOpenNode = openNodes[0];

            bool findCorrectPosition = false;
            int i = 0;

            do
            {
                // Cas où le coût du nœud à ajouter est inférieur à celui du nœud ouvert
                // que l’on évalue actuellement
                if (newNode.TotalCost < evaluateOpenNode.TotalCost)
                {
                    // On ajoute notre nœud juste avant le nœud évalué
                    openNodes.Insert(i, newNode);
                    findCorrectPosition = true;
                }

                // Cas où le coût du nœud à ajouter est supérieur à celui du nœud ouvert
                // que l’on évalue actuellement
                else
                {
                    i++;

                    // Cas où on a évalué tous les nœuds ouverts et qu’ils ont
                    // tous un coût supérieur au nœud à ajouter
                    if (openNodes.Count == i)
                    {
                        // Il n’y a plus aucun nœud à évaluer
                        evaluateOpenNode = null;

                        // On ajoute notre nœud à la fin de la liste
                        openNodes.Insert(i, newNode);
                    }

                    // Cas où il reste des nœuds à évaluer parmi les nœuds ouverts
                    else
                    {
                        // Le nœud à évaluer devient le suivant dans la liste des nœuds ouverts
                        evaluateOpenNode = openNodes[i];
                    }
                }
            }
            while ((evaluateOpenNode != null) && (findCorrectPosition == false));
        }
    }
}