using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class Node
{
    public int numberOfWinSimulations;
    public int numberOfSimulations;
    public string SimulationText => Mathf.Floor((float) numberOfSimulations / (float) numberOfSimulations) * 100 + "%";
    protected bool isPlayerTurn;
    protected Node parentNode;
    public Dictionary<Node, int> children; //node and selected col
    //debug -> public string name;

    public Node(bool isPlayerTurn = false, Node parentNode = null)
    {
        numberOfWinSimulations = 0;
        numberOfSimulations = 0;
        this.isPlayerTurn = isPlayerTurn;
        this.parentNode = parentNode;
        children = new Dictionary<Node, int>();
        //debug -> name = "Node " + (parentNode == null ? "root" : "filha de " + parentNode.name);
    }

    public void AddChild(Node node, int column)
    {
        children.Add(node, column);
    }

    public Node SelectNodeToExpand(int nbSimulation, Simulation simulation)
    {
        //if cannot proceed return itself
        if (!simulation.ContainsEmptyCell || simulation.SomeoneWon ||
            children.Keys.Count < simulation.GetPossibleMovements().Count) {
            return this;
        }

        //find the best move, simulate it and expand
        Node bestNode = SelectBestChild(nbSimulation);
        simulation.SimulateDrop(children[bestNode]);
        simulation.SwitchPlayer();
        return bestNode.SelectNodeToExpand(nbSimulation, simulation);
    }

    public Node Expand(Simulation simulation)
    {
        //return itself if cannot proceed
        if (!simulation.ContainsEmptyCell || simulation.SomeoneWon) {
            return this;
        }

        //copy the list of possible drops and remove what was already tested
        List<Vector2Int> drops = new List<Vector2Int>(simulation.GetPossibleMovements());
        foreach (int column in children.Values) {
            int index = drops.FindIndex (element => element.y == column);
            if (index >= 0) {
                drops.RemoveAt(index);
            }  
        }

        //select a random possibility and test it
        System.Random r = new System.Random();
        int colToPlay = drops[r.Next(0, drops.Count)].y;
        Node n = new Node(simulation.isPlayersTurn, this);
        AddChild(n, colToPlay);
        simulation.SimulateDrop(colToPlay);
        simulation.SwitchPlayer();
        return n;
    }

    public bool Simulate(Simulation simulation)
    {
        //keep trying random moves until someone wins or it ends in a draw
        if (simulation.SomeoneWon) {
            return !simulation.isPlayersTurn;
        }

        while (simulation.ContainsEmptyCell) {
            int column = simulation.GetRandomMove();
            simulation.SimulateDrop(column);

            if (simulation.SomeoneWon) {
                return simulation.isPlayersTurn;
            }
            simulation.SwitchPlayer();
        }
        return true;
    }

    public void BackPropagate(bool playersVictory)
    {
        //mark all parent nodes that this path leads to victory
        numberOfSimulations++;
        if (isPlayerTurn == playersVictory) {
            numberOfWinSimulations++;
        }
        if (parentNode != null) {
            parentNode.BackPropagate(playersVictory);
        }
    }

    public Node SelectBestChild(int nbSimulation)
    {
        //finds the best node among all possibilities
        double maxValue = -1;
        Node bestNode = null;
        foreach (Node child in children.Keys) {
            double evaluation = (double) child.numberOfWinSimulations / (double) child.numberOfSimulations + Mathf.Sqrt(2 * Mathf.Log((float) nbSimulation) / (float) child.numberOfSimulations);
            if (maxValue < evaluation) {
                maxValue = evaluation;
                bestNode = child;
            }
        }
        return bestNode;
    }

    public int MostSelectedMove()
    {
        //finds the best column among all possibilities
        double maxValue = -1;
        int bestMove = -1;
        foreach (var child in children) {
            if ((double) child.Key.numberOfWinSimulations / (double) child.Key.numberOfSimulations > maxValue) {
                bestMove = child.Value;
                maxValue = (double) child.Key.numberOfWinSimulations / (double) child.Key.numberOfSimulations;
            }
        }
        return bestMove;
    }
}

public class MonteCarloSearchTree
{

    public readonly Simulation startingSimulation;
    public Simulation currentSimulation;
    public Node rootNode;

    public int nbIteration;

    public ManualResetEvent doneEvent;

    public MonteCarloSearchTree(Simulation simulation, ManualResetEvent doneEvent, int nbIteration)
    {
        this.doneEvent = doneEvent;
        startingSimulation = simulation;
        this.nbIteration = nbIteration;
    }
}
