using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class SmartOpponent : Opponent
{
    [SerializeField] protected int parallelProcesses = 2;
    [SerializeField] protected int mctsIterations = 500;

    private Simulation simulation;

    public override void Play()
    {
        //simulate current board
        simulation = new Simulation(false, Utils.GetGameController().GetBoard());

        // One event is used for each MCTS.
        ManualResetEvent[] doneEvents = new ManualResetEvent[parallelProcesses];
        MonteCarloSearchTree[] trees = new MonteCarloSearchTree[parallelProcesses];

        //set threads
        for (int i = 0; i < parallelProcesses; i++) {
            doneEvents[i] = new ManualResetEvent(false);
            trees[i] = new MonteCarloSearchTree(simulation, doneEvents[i], mctsIterations);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ExpandTree), trees[i]);
        }

        //wait until they all finish
        WaitHandle.WaitAll(doneEvents);

        //regrouping all results
        Node rootNode = new Node();
        for (int i = 0; i < parallelProcesses; i++) {
            //sort children
            var sortedChildren = trees[i].rootNode.children.ToList();
            sortedChildren.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            //get only the first children of each choice
            foreach (var child in sortedChildren) {
                if (!rootNode.children.ContainsValue(child.Value)) {
                    Node rootChild = new Node {
                        numberOfWinSimulations = child.Key.numberOfWinSimulations,
                        numberOfSimulations = child.Key.numberOfSimulations
                    };
                    rootNode.children.Add(rootChild, child.Value);
                }
                else {
                    Node rootChild = rootNode.children.First(p => p.Value == child.Value).Key;
                    rootChild.numberOfWinSimulations += child.Key.numberOfWinSimulations;
                    rootChild.numberOfSimulations += child.Key.numberOfSimulations;
                }
            }
        }

        //gets the best choice and perform it
        int choice = rootNode.MostSelectedMove();
        var movements = Utils.GetGameController().GetPossibleMovements();
        Vector2Int indexes = movements.Find(element => element.y == choice);
        Utils.GetGameController().AddPiece(indexes);
    }

    public static void ExpandTree(object t)
    {
        //start the tree
        MonteCarloSearchTree tree = (MonteCarloSearchTree) t;
        tree.currentSimulation = tree.startingSimulation.Clone();
        tree.rootNode = new Node(tree.currentSimulation.isPlayersTurn);

        Node selectedNode;
        Node expandedNode;

        //for up to so many times, try to expand the nodes and find a winning outcome
        for (int i = 0; i < tree.nbIteration; i++) {
            tree.currentSimulation = tree.startingSimulation.Clone();
            selectedNode = tree.rootNode.SelectNodeToExpand(tree.rootNode.numberOfSimulations, tree.currentSimulation);
            expandedNode = selectedNode.Expand(tree.currentSimulation);
            expandedNode.BackPropagate(expandedNode.Simulate(tree.currentSimulation));
        }

        tree.doneEvent.Set();
    }
}
