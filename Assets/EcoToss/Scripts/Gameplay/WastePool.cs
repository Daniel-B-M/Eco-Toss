using System.Collections.Generic;
using EcoToss.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Object Pool for waste items. Keeps one ObjectPool per WasteData so each
    /// type reuses its own instances instead of instantiating and destroying every turn.
    /// Pools are created lazily the first time a WasteData is requested.
    /// </summary>
    public class WastePool : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Pool Settings")]
        [Tooltip("Instances reserved per waste type when its pool is created.")]
        [SerializeField, Min(1)] private int defaultCapacityPerType = 2;

        [Tooltip("Maximum inactive instances kept per waste type. Extra ones are destroyed on release.")]
        [SerializeField, Min(1)] private int maxSizePerType = 5;

        #endregion

        #region Private Fields

        private readonly Dictionary<WasteData, ObjectPool<WasteItem>> poolsByWasteData =
            new Dictionary<WasteData, ObjectPool<WasteItem>>();

        #endregion

        #region Public API

        /// <summary>Returns an active, frozen WasteItem for the given data. Null if the data is not usable.</summary>
        public WasteItem Get(WasteData wasteData)
        {
            ObjectPool<WasteItem> pool = GetOrCreatePool(wasteData);
            return pool?.Get();
        }

        /// <summary>Resets the item and returns it to the pool of its WasteData.</summary>
        public void Release(WasteItem wasteItem)
        {
            if (wasteItem == null) return;

            if (wasteItem.Data != null && poolsByWasteData.TryGetValue(wasteItem.Data, out ObjectPool<WasteItem> pool))
            {
                pool.Release(wasteItem);
            }
            else
            {
                // Item did not come from this pool (e.g. placed by hand in the scene).
                Destroy(wasteItem.gameObject);
            }
        }

        #endregion

        #region Pool Creation

        private ObjectPool<WasteItem> GetOrCreatePool(WasteData wasteData)
        {
            if (!IsValid(wasteData)) return null;

            if (poolsByWasteData.TryGetValue(wasteData, out ObjectPool<WasteItem> existingPool))
                return existingPool;

            var newPool = new ObjectPool<WasteItem>(
                createFunc: () => CreateItem(wasteData),
                actionOnGet: OnGetItem,
                actionOnRelease: OnReleaseItem,
                actionOnDestroy: OnDestroyItem,
                collectionCheck: true,
                defaultCapacity: defaultCapacityPerType,
                maxSize: maxSizePerType);

            poolsByWasteData.Add(wasteData, newPool);
            return newPool;
        }

        private bool IsValid(WasteData wasteData)
        {
            if (wasteData == null)
            {
                Debug.LogError($"[{nameof(WastePool)}] Requested a null WasteData.", this);
                return false;
            }

            if (wasteData.Prefab == null || wasteData.Prefab.GetComponent<WasteItem>() == null)
            {
                Debug.LogError($"[{nameof(WastePool)}] '{wasteData.name}' has no prefab with a {nameof(WasteItem)} on its root.", wasteData);
                return false;
            }

            return true;
        }

        #endregion

        #region Pool Callbacks

        private WasteItem CreateItem(WasteData wasteData)
        {
            GameObject instance = Instantiate(wasteData.Prefab, transform);
            instance.name = wasteData.Prefab.name;

            WasteItem wasteItem = instance.GetComponent<WasteItem>();
            wasteItem.Bind(wasteData);
            return wasteItem;
        }

        private void OnGetItem(WasteItem wasteItem)
        {
            wasteItem.ResetForReuse();
            wasteItem.gameObject.SetActive(true);
        }

        private void OnReleaseItem(WasteItem wasteItem)
        {
            wasteItem.ResetForReuse();
            wasteItem.gameObject.SetActive(false);
        }

        private void OnDestroyItem(WasteItem wasteItem)
        {
            if (wasteItem != null) Destroy(wasteItem.gameObject);
        }

        #endregion
    }
}
