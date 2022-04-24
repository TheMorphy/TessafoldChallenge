using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using Morphy_Pathfinding._Scripts.Grid.Scriptables;
using Morphy_Pathfinding._Scripts.Units;
using UnityEngine;

namespace Morphy_Pathfinding._Scripts.Grid {
    public class GridManager : MonoBehaviour {
        public static GridManager Instance;

        // I call blue balls player and red balls enemies
        [SerializeField] private Sprite _playerSprite, _goalSprite;
        [SerializeField] private Unit _unitPrefab, _enemyPrefab;
        [SerializeField] private ScriptableGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;

        public Dictionary<Vector2, NodeBase> Tiles { get; private set; }

        public List<NodeBase> _playerNodeBaseList, _enemyNodeBaseList; //List of tiles occupied
        public List<Unit> _spawnedPlayerList, _spawnedEnemyList; //List of spawned units

        // How many player you want to spawn in the simulation
        [SerializeField] int playersToSpawnMin;
        [SerializeField] int playersToSpawnMax;

        // How many enemies you want to spawn in the simulation
        [SerializeField] int enemiesToSpawnMin;
        [SerializeField] int enemiesToSpawnMax;

        void Awake() => Instance = this;

        private void Start() {
            // Generating a grid and spawning units
            Tiles = _scriptableGrid.GenerateGrid();
         
            foreach (var tile in Tiles.Values) tile.CacheNeighbors();

            SpawnUnits();
        }

        void SpawnUnits() {
            // We choose a random number that determines how many units we want to spawn
            int playerAmount = Random.Range(playersToSpawnMin, playersToSpawnMax);
            int enemyAmount = Random.Range(enemiesToSpawnMin, enemiesToSpawnMax);

            // Check if the tile is walkable and choose the first one to spawn in
            NodeBase playerNodeBase = Tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;

            for (int i = 0; i < playerAmount; i++)
            {
                if (i > 0)
                {
                    // Make sure you spawn near the ally
                    foreach (var currentPlayerNode in _playerNodeBaseList)
                    {
                        IEnumerable<NodeBase> potentialNeighbourList = currentPlayerNode.Neighbors.Where(t => t.Walkable && !_playerNodeBaseList.Contains(t));
                        if (potentialNeighbourList.Count<NodeBase>() > 0)
                        {
                            playerNodeBase = potentialNeighbourList.First();
                            break;
                        }
                    } 

                    // Check that the tile is not already occupied. If it is, then choose another tile to spawn in
                    bool retry = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (_playerNodeBaseList[j] == playerNodeBase)
                        {
                            playerNodeBase = Tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
                            retry = true;
                            break;
                        }
                    }
                    if (retry)
                    {
                        i--;
                        continue;
                    }
                }
                // After all checks we spawn the player (Blue Ball) and add it to the list
                _playerNodeBaseList.Add(playerNodeBase);
                Unit spawnedPlayer = Instantiate(_unitPrefab, playerNodeBase.Coords.Pos, Quaternion.identity);
                spawnedPlayer.Init(_playerSprite);
                _spawnedPlayerList.Add(spawnedPlayer);
            }

            // Check if the tile is walkable and choose the first one to spawn in
            NodeBase enemyNodeBase = Tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;

            for (int i = 0; i < enemyAmount; i++)
            {
                if (i > 0)
                {
                    // Make sure you spawn near the ally
                    foreach (var currentEnemyNode in _enemyNodeBaseList)
                    {
                        IEnumerable<NodeBase> potentialNeighbourList = currentEnemyNode.Neighbors.Where(t => t.Walkable && !_enemyNodeBaseList.Contains(t));
                        if (potentialNeighbourList.Count<NodeBase>() > 0)
                        {
                            enemyNodeBase = potentialNeighbourList.First();
                            break;
                        }
                    }

                    // Check that the tile is not already occupied. If it is, then choose another tile to spawn in
                    bool retry = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (_enemyNodeBaseList[j] == enemyNodeBase)
                        {
                            enemyNodeBase = Tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
                            retry = true;
                            break;
                        }
                    }
                    if (retry)
                    {
                        i--;
                        continue;
                    }
                }
                // After all checks we spawn the enemy (Red Ball) and add it to the list
                _enemyNodeBaseList.Add(enemyNodeBase);
                Unit spawnedEnemy = Instantiate(_enemyPrefab, enemyNodeBase.Coords.Pos, Quaternion.identity);
                spawnedEnemy.Init(_goalSprite);
                _spawnedEnemyList.Add(spawnedEnemy);
            }
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => Tiles.TryGetValue(pos, out var tile) ? tile : null;

        // Draw connections stuff
        //private void OnDrawGizmos() {
        //    if (!Application.isPlaying || !_drawConnections) return;
        //    Gizmos.color = Color.red;
        //    foreach (var tile in Tiles) {
        //        if (tile.Value.Connection == null) continue;
        //        Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
        //    }
        //}
    }
}