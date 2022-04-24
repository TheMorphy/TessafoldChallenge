using System.Collections.Generic;
using _Scripts.Tiles;
using Morphy_Pathfinding._Scripts.Units;
using UnityEngine;

namespace Morphy_Pathfinding._Scripts.Grid
{
    public class UnitMovement : MonoBehaviour
    {
        [SerializeField] GridManager gridManager;

        private void Start()
        {
            InvokeRepeating("MoveUnits", 1f, 1f); // Just moving and attacking (if possible) every 1 second here
        }

        // Moving and attacking function for both the red ball and the blue ball
        private void MoveUnits()
        {
            // In general, this loop is needed in order to find the shortest path and the closest red ball to attack
            for (int i = 0; i < gridManager._playerNodeBaseList.Count; i++)
            {
                Unit player = gridManager._spawnedPlayerList[i];
               
                if (player.isDead)
                {
                    // Checking if the blue ball is dead, so as not to take it into account when pathfinding and attacking
                    continue;
                }

                NodeBase playerNodeBase = gridManager._playerNodeBaseList[i];
                List<NodeBase> shortestPath = null;

                Unit closestEnemyUnit = null;

                // Looking for red balls and doing some checks 
                for (int j = 0; j < gridManager._enemyNodeBaseList.Count; j++)
                {
                    Unit enemy = gridManager._spawnedEnemyList[j];
                    
                    if (enemy.isDead)
                    {
                        // Checking if the red ball is dead, so as not to take it into account when pathfinding and attacking
                        continue;
                    }

                    NodeBase enemyNodeBase = gridManager._enemyNodeBaseList[j];
                    List<NodeBase> liveAllies = new List<NodeBase>(gridManager._playerNodeBaseList);

                    // Using this loop to find blue balls that are already dead to delete them from the liveAllies list, so enemies don't try to hit them
                    // Also not to take them into account when pathfinding
                    for (int k = gridManager._spawnedPlayerList.Count - 1; k >= 0; k--)
                    {
                        if (gridManager._spawnedPlayerList[k].isDead)
                        {
                            liveAllies.RemoveAt(k);
                        }
                    }

                    List<NodeBase> path = Pathfinding.FindPath(playerNodeBase, enemyNodeBase, liveAllies);

                    // Updating for the shortest path
                    if (shortestPath == null)
                    {
                        shortestPath = path;
                        closestEnemyUnit = enemy;
                    }
                    else if (path.Count < shortestPath.Count)
                    {
                        shortestPath = path;
                        closestEnemyUnit = enemy;
                    }
                }

                if (shortestPath == null || shortestPath.Count < 2)
                {
                    if (shortestPath != null)
                    { 
                        //Hit the closest enemy
                        closestEnemyUnit.Hit(player.hitDamage);
                    }
                    continue;
                }
                // Moving a blue ball to the next tile on the shortest path list
                gridManager._playerNodeBaseList[i] = shortestPath[shortestPath.Count - 1];
                player.previousPosition = player.targetPosition;
                player.targetPosition = new Vector2(shortestPath[shortestPath.Count - 1].Coords.Pos.x, shortestPath[shortestPath.Count - 1].Coords.Pos.y);
                player.lerpTime = 0;
            }

            // This loop is needed in order to find the shortest path and the closest blue ball to attack
            for (int i = 0; i < gridManager._enemyNodeBaseList.Count; i++)
            {
                Unit enemy = gridManager._spawnedEnemyList[i];
                
                if (enemy.isDead)
                {
                    // Checking if the red ball is dead, so as not to take it into account when pathfinding and attacking
                    continue;
                }

                NodeBase enemyNodeBase = gridManager._enemyNodeBaseList[i];
                List<NodeBase> shortestPath = null;

                Unit closestPlayerUnit = null;

                // Looking for blue balls and doing some checks 
                for (int j = 0; j < gridManager._playerNodeBaseList.Count; j++)
                {
                    Unit player = gridManager._spawnedPlayerList[j];
                    
                    if (player.isDead)
                    {
                        // Checking if the blue ball is dead, so as not to take it into account when pathfinding and attacking
                        continue;
                    }

                    NodeBase playerNodeBase = gridManager._playerNodeBaseList[j];
                    List<NodeBase> liveAllies = new List<NodeBase>(gridManager._enemyNodeBaseList);

                    // Using this loop to find red balls that are already dead to delete them from the liveAllies list, so enemies don't try to hit them
                    // Also not to take them into account when pathfinding
                    for (int k = gridManager._spawnedEnemyList.Count - 1; k >= 0; k--)
                    {
                        if (gridManager._spawnedEnemyList[k].isDead)
                        {
                            liveAllies.RemoveAt(k);
                        }
                    }
                    
                    List<NodeBase> path = Pathfinding.FindPath(enemyNodeBase, playerNodeBase, liveAllies);

                    // Updating for the shortest path
                    if (shortestPath == null)
                    {
                        shortestPath = path;
                        closestPlayerUnit = player;
                    }
                    else if (path.Count < shortestPath.Count)
                    {
                        shortestPath = path;
                        closestPlayerUnit = player;
                    }
                }

                if (shortestPath == null || shortestPath.Count < 2)
                {
                    if (shortestPath != null)
                    {
                        //Hit the closest player
                        closestPlayerUnit.Hit(enemy.hitDamage);
                    }
                    continue;
                }
                // Moving a red ball to the next tile on the shortest path list
                gridManager._enemyNodeBaseList[i] = shortestPath[shortestPath.Count - 1];
                enemy.previousPosition = enemy.targetPosition;
                enemy.targetPosition = new Vector2(shortestPath[shortestPath.Count - 1].Coords.Pos.x, shortestPath[shortestPath.Count - 1].Coords.Pos.y);
                enemy.lerpTime = 0;
            }
        }
    }
}