using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class Node
{
    private static Position finalPosition;
    private static int[,] table = new int[110, 110];

    public Position name;
    protected double gCost;              // Coût du chemin du nœud initial jusqu’à ce nœud
    protected double hCost;              // Estimation heuristique du coût pour atteindre le nœud final
    protected double totalCost;          // Coût total (g + h)
    protected Node parent;               // Nœud Parent
    protected List<Node> children;       // Nœuds Enfants

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="position">Position servant de nœud</param>
    public Node(Position position)
    {
        parent = null;
        children = new List<Node>();
        this.name = position;
    }

    /// <summary>
    /// Constructeur du premier nœud
    /// </summary>
    /// <param name="initial">Nœud de départ</param>
    /// <param name="final">Nœud d’arrivée</param>
    /// <param name="tableNoeuds">Matrice des déplacements</param>
    public Node(Position initial, Position final, int[,] table)
    {
        parent = null;
        children = new List<Node>();
        this.name = initial;
        Node.finalPosition = final;
        Node.table = table;
    }

    public double GCost
    {
        get { return gCost; }
        set { gCost = value; }
    }

    public double HCost
    {
        get { return hCost; }
        set { hCost = value; }
    }

    public double TotalCost
    {
        get { return totalCost; }
        set { totalCost = value; }
    }

    public List<Node> Children
    {
        get { return children; }
    }
    
    public Node Parent()
    {
        return parent;
    }

    public void Parent(Node value)
    {
        parent = value;
        value.children.Add(this);
    }

    public void RemoveRelationsParentChildren()
    {
        if (parent == null) return;
        parent.children.Remove(this);
        parent = null;
    }

    public void EvaluateTotalCost()
    {
        totalCost = gCost + hCost;
    }

    /// <summary>
    /// Permet de vérifier si 2 nœuds sont identiques
    /// </summary>
    /// <param name="evaluateNode">Nœud que l’on compare</param>
    /// <returns>true si les nœuds sont les mêmes, false sinon</returns>
    public bool IsTheSame(Node evaluateNode)
    {
        return (this.name.SamePosition(evaluateNode.name));
    }

    /// <summary>
    /// Permet d’obtenir le coût d’un déplacement
    /// </summary>
    /// <param name="nextNode">Nœud vers lequel on se déplace</param>
    /// <returns>Coût du déplacement</returns>
    public double GetCost(Node nextNode)
    {
        return 1;
    }

    /// <summary>
    /// Permet de vérifier si on est arrivé au nœud objectif
    /// </summary>
    /// <returns>Vrai si on a atteint l’objectif et faux sinon</returns>
    public bool CheckEnd()
    {
        return (this.name.SamePosition(Node.finalPosition));
    }

    /// <summary>
    /// Permet d’obtenir les successeurs du nœud
    /// </summary>
    /// <returns>Liste des successeurs</returns>
    public List<Node> GetSuccessors()
    {
        List<Node> listSuccessors = new List<Node>();

        for (int i = 0; i < table.GetLength(1); i++)
        {
            if (table[this.name.Number, i] == 1)
            {
                listSuccessors.Add(new Node(new Position(i)));
            }
        }

        return listSuccessors;
    }

    /// <summary>
    /// Permet de calculer le coût heuristique
    /// </summary>
    public void EvaluateHCost()
    {
        this.HCost = 1;
    }

    /// <summary>
    /// Permet d’obtenir les informations concernant le nœud sous forme de chaîne
    /// </summary>
    /// <returns>Nœud</returns>
    public override string ToString()
    {
        return this.name.Number.ToString();
    }
}