using System.Collections.Generic;
using EcoToss.Core;
using EcoToss.Gameplay;
using UnityEngine;

namespace EcoToss.Feedback
{
    /// <summary>
    /// Plays a particle burst at the bin mouth when a waste is sorted.
    /// Listens to GameEvents only, so no gameplay script needs to know about VFX.
    /// One effect instance per bin and result is created up front and reused (simple pooling).
    /// </summary>
    public class BinVfxController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Bins")]
        [Tooltip("All bins in the scene. Each one gets its own effect instances at its mouth.")]
        [SerializeField] private BinTrigger[] binTriggers;

        [Header("Effect Prefabs")]
        [SerializeField] private ParticleSystem correctSortEffectPrefab;
        [SerializeField] private ParticleSystem incorrectSortEffectPrefab;

        #endregion

        #region Private Fields

        private readonly Dictionary<BinType, ParticleSystem> correctEffectsByBin = new();
        private readonly Dictionary<BinType, ParticleSystem> incorrectEffectsByBin = new();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            CreateEffectInstances();
        }

        private void OnEnable()
        {
            GameEvents.WasteSortedCorrectly += HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly += HandleWasteSortedIncorrectly;
        }

        private void OnDisable()
        {
            GameEvents.WasteSortedCorrectly -= HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly -= HandleWasteSortedIncorrectly;
        }

        #endregion

        #region Setup

        private void CreateEffectInstances()
        {
            foreach (BinTrigger binTrigger in binTriggers)
            {
                if (binTrigger == null) continue;

                if (correctSortEffectPrefab != null)
                    correctEffectsByBin[binTrigger.BinType] = CreateEffectAt(correctSortEffectPrefab, binTrigger);

                if (incorrectSortEffectPrefab != null)
                    incorrectEffectsByBin[binTrigger.BinType] = CreateEffectAt(incorrectSortEffectPrefab, binTrigger);
            }
        }

        private ParticleSystem CreateEffectAt(ParticleSystem effectPrefab, BinTrigger binTrigger)
        {
            // Child of this controller so the hierarchy stays clean.
            ParticleSystem effectInstance = Instantiate(effectPrefab, binTrigger.MouthPosition, Quaternion.identity, transform);
            effectInstance.name = $"{effectPrefab.name}_{binTrigger.BinType}";
            return effectInstance;
        }

        #endregion

        #region Event Handlers

        private void HandleWasteSortedCorrectly(WasteData sortedWaste)
        {
            // A correct sort always lands in the waste's own bin.
            PlayEffect(correctEffectsByBin, sortedWaste.CorrectBin);
        }

        private void HandleWasteSortedIncorrectly(WasteData sortedWaste, BinType binHit)
        {
            PlayEffect(incorrectEffectsByBin, binHit);
        }

        private static void PlayEffect(Dictionary<BinType, ParticleSystem> effectsByBin, BinType binType)
        {
            if (!effectsByBin.TryGetValue(binType, out ParticleSystem effect)) return;

            // Restart from zero in case the previous burst is still visible.
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Play(true);
        }

        #endregion
    }
}
