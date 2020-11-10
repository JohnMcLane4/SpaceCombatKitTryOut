using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Manages audio for a beam controller.
    /// </summary>
    public class BeamAudioController : MonoBehaviour
    {

        [SerializeField]
        protected AudioSource beamStartedAudio;

        [SerializeField]
        protected AudioSource beamOngoingAudio;

        [SerializeField]
        protected float maxBeamOngoingAudioVolume = 1;


        protected virtual void Awake()
        {
            if (beamOngoingAudio != null)
            {
                beamOngoingAudio.volume = 0;
            }
        }

        /// <summary>
        /// Play audio when beam state changes.
        /// </summary>
        /// <param name="newBeamState">The new beam state.</param>
        public virtual void OnBeamStateChanged(BeamState newBeamState)
        {
            if (newBeamState == BeamState.FadingIn)
            {
                if (beamStartedAudio != null)
                {
                    beamStartedAudio.Play();
                }
            }
        }

        /// <summary>
        /// Play audio when the beam level changes.
        /// </summary>
        /// <param name="beamLevel">The new beam level.</param>
        public virtual void OnBeamLevelSet(float beamLevel)
        {
            if (beamOngoingAudio != null)
            {
                beamOngoingAudio.volume = maxBeamOngoingAudioVolume * beamLevel;
            }
        }
    }
}
