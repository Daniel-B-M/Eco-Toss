using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Trigger placed inside a bin, a bit below the rim. When a waste falls in,
    /// it compares the waste's CorrectBin with this bin and raises the matching event.
    /// It does not touch karma or the pool: other systems react to the events.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BinTrigger : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Bin")]
        [SerializeField] private BinType binType;

        [Tooltip("Center of the bin opening. Used later by Assisted Mode (ThrowToBin).")]
        [SerializeField] private Transform binMouthPoint;

        [Header("Debug")]
        [SerializeField] private bool logDepositsInConsole = true;

        #endregion

        #region Public Properties

        public BinType BinType => binType;

        public Vector3 MouthPosition =>
            binMouthPoint != null ? binMouthPoint.position : transform.position;

        #endregion

        #region Unity Lifecycle

        private void Reset()
        {
            // Runs when the component is added in the editor.
            GetComponent<Collider>().isTrigger = true;
        }

        private void Awake()
        {
            Collider depositCollider = GetComponent<Collider>();
            if (!depositCollider.isTrigger)
            {
                Debug.LogWarning($"[{nameof(BinTrigger)}] Collider on '{name}' was not a trigger. Fixing it.", this);
                depositCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Waste colliders live on the root, next to the Rigidbody.
            Rigidbody enteringBody = other.attachedRigidbody;
            if (enteringBody == null) return;
            if (!enteringBody.TryGetComponent(out WasteItem enteringWaste)) return;

            // A bin and the floor can't both score the same throw.
            if (!enteringWaste.TryMarkAsResolved()) return;

            EvaluateDeposit(enteringWaste.Data);
        }

        #endregion

        #region Evaluation

        private void EvaluateDeposit(WasteData depositedWaste)
        {
            bool isCorrectBin = depositedWaste.CorrectBin == binType;

            if (logDepositsInConsole)
                Debug.Log($"[{nameof(BinTrigger)}] {depositedWaste.DisplayName} -> {binType}: {(isCorrectBin ? "CORRECT" : "WRONG")}", this);

            if (isCorrectBin)
                GameEvents.RaiseWasteSortedCorrectly(depositedWaste);
            else
                GameEvents.RaiseWasteSortedIncorrectly(depositedWaste, binType);
        }

        #endregion
    }
}
