using System;
using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Charges and launches the waste of the current turn.
    /// Input comes from ThrowInputReader and the countdown from AimTimer;
    /// this script only decides the launch speed and releases the waste.
    /// </summary>
    public class WasteThrower : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private ThrowInputReader throwInputReader;
        [SerializeField] private AimTimer aimTimer;
        [Tooltip("Its forward vector is the launch direction (rotated by WasteAim).")]
        [SerializeField] private Transform aimDirectionPivot;

        [Header("Launch Charge")]
        [SerializeField] private float minLaunchSpeed = 3f;
        [SerializeField] private float maxLaunchSpeed = 12f;
        [SerializeField, Min(0.01f)] private float timeToFullCharge = 2f;

        #endregion

        #region Private Fields

        private WasteItem currentWaste;
        private float currentLaunchSpeed;
        private bool isAiming;

        // True only if the button was pressed during this turn, so a click that
        // started elsewhere (e.g. the briefing button) can't launch on release.
        private bool hasChargeStarted;

        #endregion

        #region Events

        /// <summary>Raised right after the waste is released. The TurnController listens to this.</summary>
        public event Action<WasteItem> WasteLaunched;

        #endregion

        #region Public Properties

        public bool IsAiming => isAiming;

        public Vector3 LaunchVelocity => aimDirectionPivot.forward * currentLaunchSpeed;

        public Vector3 LaunchStartPosition =>
            currentWaste != null ? currentWaste.transform.position : aimDirectionPivot.position;

        /// <summary>0 to 1 charge amount, useful for a future charge bar.</summary>
        public float ChargePercent => Mathf.InverseLerp(minLaunchSpeed, maxLaunchSpeed, currentLaunchSpeed);

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            aimTimer.TimeExpired += HandleAimTimeExpired;
        }

        private void OnDisable()
        {
            aimTimer.TimeExpired -= HandleAimTimeExpired;
        }

        private void Update()
        {
            if (!isAiming) return;

            // While input is blocked (pause, briefing) any half-done charge is discarded.
            if (!throwInputReader.IsInputAllowed)
            {
                hasChargeStarted = false;
                return;
            }

            if (throwInputReader.IsChargeHeld)
            {
                hasChargeStarted = true;
                ChargeLaunch();
            }

            if (throwInputReader.WasChargeReleasedThisFrame && hasChargeStarted)
                Launch();
        }

        #endregion

        #region Public API

        /// <summary>Starts an aiming turn with the given waste. Called by the TurnController.</summary>
        public void BeginAiming(WasteItem wasteToThrow)
        {
            if (wasteToThrow == null) return;

            currentWaste = wasteToThrow;
            currentLaunchSpeed = minLaunchSpeed;
            hasChargeStarted = false;
            isAiming = true;
            aimTimer.Begin();
        }

        /// <summary>Cancels the current turn without throwing (e.g. restart or end of match).</summary>
        public void CancelAiming()
        {
            isAiming = false;
            currentWaste = null;
            aimTimer.Stop();
        }

        #endregion

        #region Charge And Launch

        private void ChargeLaunch()
        {
            float chargeRate = (maxLaunchSpeed - minLaunchSpeed) / timeToFullCharge;
            currentLaunchSpeed = Mathf.MoveTowards(currentLaunchSpeed, maxLaunchSpeed, chargeRate * Time.deltaTime);
        }

        private void HandleAimTimeExpired()
        {
            Launch();
        }

        /// <summary>Releases the current waste with the charged velocity and announces it.</summary>
        private void Launch()
        {
            if (!isAiming || currentWaste == null) return;

            isAiming = false;
            aimTimer.Stop();

            WasteItem launchedWaste = currentWaste;
            launchedWaste.Launch(LaunchVelocity);

            GameEvents.RaiseWasteThrown(launchedWaste.Data);
            WasteLaunched?.Invoke(launchedWaste);
        }

        #endregion
    }
}