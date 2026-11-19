using System.Collections.Generic;
using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Picks the next waste to throw (never the same type twice in a row),
    /// takes it from the WastePool, places it at the spawn point and raises WasteSpawned.
    /// It does not decide WHEN to spawn: the TurnController calls SpawnNext().
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private WastePool wastePool;
        [Tooltip("Where each new waste appears before being thrown.")]
        [SerializeField] private Transform wasteSpawnPoint;

        [Header("Catalog")]
        [Tooltip("All waste types that can appear during a match.")]
        [SerializeField] private List<WasteData> wasteCatalog = new List<WasteData>();

        #endregion

        #region Private Fields

        private int lastSpawnedIndex = -1;

        #endregion

        #region Public Properties

        /// <summary>The waste currently waiting to be thrown or in flight. Null if none.</summary>
        public WasteItem CurrentWaste { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (wastePool == null || wasteSpawnPoint == null || wasteCatalog.Count == 0)
            {
                Debug.LogError($"[{nameof(SpawnManager)}] Missing pool, spawn point or catalog entries.", this);
                enabled = false;
            }
        }

        #endregion

        #region Public API

        /// <summary>Spawns a new random waste at the spawn point and returns it.</summary>
        public WasteItem SpawnNext()
        {
            if (!enabled) return null;

            WasteData nextWasteData = PickNextWasteData();
            WasteItem spawnedWaste = wastePool.Get(nextWasteData);
            if (spawnedWaste == null) return null;

            spawnedWaste.PlaceAt(wasteSpawnPoint.position, wasteSpawnPoint.rotation);
            CurrentWaste = spawnedWaste;

            GameEvents.RaiseWasteSpawned(nextWasteData);
            return spawnedWaste;
        }

        /// <summary>Returns the current waste to the pool and clears it.</summary>
        public void DespawnCurrent()
        {
            if (CurrentWaste == null) return;

            wastePool.Release(CurrentWaste);
            CurrentWaste = null;
        }

        #endregion

        #region Selection

        /// <summary>Random pick that skips the previous index, so no type repeats twice in a row.</summary>
        private WasteData PickNextWasteData()
        {
            int catalogSize = wasteCatalog.Count;
            if (catalogSize == 1) return wasteCatalog[0];

            // First spawn: any index. Later: choose among (size - 1) options and shift past the last index to exclude it.
            bool hasPrevious = lastSpawnedIndex >= 0;
            int pickedIndex = Random.Range(0, hasPrevious ? catalogSize - 1 : catalogSize);
            if (hasPrevious && pickedIndex >= lastSpawnedIndex) pickedIndex++;

            lastSpawnedIndex = pickedIndex;
            return wasteCatalog[pickedIndex];
        }

        #endregion
    }
}
