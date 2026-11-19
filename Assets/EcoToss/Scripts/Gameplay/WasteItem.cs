using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Runtime component on the root of every waste prefab.
    /// Knows which WasteData it represents and owns its own physics state,
    /// so other systems (thrower, bins, pool) never touch the Rigidbody directly.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class WasteItem : MonoBehaviour
    {
        #region Serialized Fields

        [Tooltip("Assigned at runtime by WastePool. Visible here only for debugging.")]
        [SerializeField] private WasteData wasteData;

        #endregion

        #region Private Fields

        private Rigidbody wasteBody;

        #endregion

        #region Public Properties

        /// <summary>Data that describes this waste (name, hint, correct bin).</summary>
        public WasteData Data => wasteData;

        /// <summary>True once a bin or the miss detector has already evaluated this throw.</summary>
        public bool IsResolved { get; private set; }

        /// <summary>True while the item is flying or rolling after being thrown.</summary>
        public bool IsInFlight => !wasteBody.isKinematic;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            wasteBody = GetComponent<Rigidbody>();
        }

        #endregion

        #region Public API

        /// <summary>Links this instance to its WasteData. Called once by the pool when the item is created.</summary>
        public void Bind(WasteData data)
        {
            wasteData = data;
        }

        /// <summary>Moves the item to a spawn point while it is frozen (kinematic).</summary>
        public void PlaceAt(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            wasteBody.position = position;
            wasteBody.rotation = rotation;
        }

        /// <summary>Releases the item into the physics simulation with the given velocity.</summary>
        public void Launch(Vector3 launchVelocity)
        {
            wasteBody.isKinematic = false;
            wasteBody.linearVelocity = launchVelocity;
        }

        /// <summary>
        /// Marks the throw as evaluated so a bin and the floor cannot both score it.
        /// Returns false if it was already resolved.
        /// </summary>
        public bool TryMarkAsResolved()
        {
            if (IsResolved) return false;

            IsResolved = true;
            return true;
        }

        /// <summary>Stops all motion and freezes the item so it can be reused from the pool.</summary>
        public void ResetForReuse()
        {
            // Velocities can only be written while the body is non-kinematic.
            if (!wasteBody.isKinematic)
            {
                wasteBody.linearVelocity = Vector3.zero;
                wasteBody.angularVelocity = Vector3.zero;
            }

            wasteBody.isKinematic = true;
            IsResolved = false;
        }

        #endregion
    }
}
