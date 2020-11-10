using UnityEngine;
using System.Collections;

namespace VSX.UniversalVehicleCombat
{
    
	/// <summary>
    /// Controls the hit effect that is shown when a beam strikes a surface.
    /// </summary>
	public class BeamHitEffectController : MonoBehaviour 
	{

        /// <summary>
        /// Set the 'on' level of the hit effect.
        /// </summary>
        /// <param name="level">The 'on' level.</param>
        public virtual void SetLevel(float level) { }

        /// <summary>
        /// Set the activation of the hit effect.
        /// </summary>
        /// <param name="activate">Whether it is activated or not.</param>
        public virtual void SetActivation(bool activate)
        {
            gameObject.SetActive(activate);
        }

        /// <summary>
        /// Do stuff when the beam hit something.
        /// </summary>
        /// <param name="hit">The hit information.</param>
        public virtual void OnHit(RaycastHit hit)
        {
            gameObject.SetActive(true);
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.normal);
        }

    }
}