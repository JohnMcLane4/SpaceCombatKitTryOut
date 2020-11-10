using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Provides information for leading a target in order to hit it when it is moving.
    /// </summary>
    public class TargetLeader : MonoBehaviour
    {
     
        // The intercept speed (e.g. of the bullet or projectile)
        [SerializeField]
        protected float interceptSpeed;
        public float InterceptSpeed
        {
            get { return interceptSpeed; }
            set { interceptSpeed = value; }
        }

        protected Vector3 leadTargetPosition = Vector3.zero;
        public Vector3 LeadTargetPosition { get { return leadTargetPosition; } }

        [SerializeField]
        protected Trackable target;
        public Trackable Target
        {
            get { return target; }
            set { target = value; }
        }

        [SerializeField]
        protected string targetVelocityKey = "Velocity";

        [SerializeField]
        protected bool useTargetBoundsCenter = true;

        public Vector3EventHandler onLeadTargetPositionUpdated;


        /// <summary>
        /// Set the target.
        /// </summary>
        /// <param name="target">The new target.</param>
        public virtual void SetTarget(Trackable target)
        {
            this.target = target;
        }

        /// <summary>
        /// Clear the target.
        /// </summary>
        public virtual void ClearTarget()
        {
            this.target = null;
        }

        /// <summary>
        /// Calculate the lead position to aim at to hit a moving target for the win.
        /// </summary>
        /// <param name="shooterPosition">The shooter position.</param>
        /// <param name="interceptSpeed">The interceot speed.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetVelocity">The target velocity vector.</param>
        /// <returns></returns>
        public static Vector3 GetLeadPosition(Vector3 shooterPosition, float interceptSpeed, Vector3 targetPosition, Vector3 targetVelocity)
        {

            if (interceptSpeed < 0.0001f) return targetPosition;
            
            // Get target direction
            Vector3 targetDirection = targetPosition - shooterPosition;

            // Get the target velocity magnitude
            float vE = targetVelocity.magnitude;

            // Note that the dot product of a and b (a.b) is also equal to |a||b|cos(theta) where theta = angle between them.
            // This saves using the components of the vectors themselves, only the magnitudes.

            // Get the length of the playerRelPos vector
            float playerRelPosMag = targetDirection.magnitude;

            // Get angle between player-target axis and the target's velocity vector
            float angPET = Vector3.Angle(targetDirection, targetVelocity) * Mathf.Deg2Rad; // Vector3.Angle returns in degrees

            // Get the cosine of this angle
            float cosPET = Mathf.Cos(angPET);

            // Get the coefficients of the quadratic equation
            float a = vE * vE - interceptSpeed * interceptSpeed;
            float b = 2 * playerRelPosMag * vE * cosPET;
            float c = playerRelPosMag * playerRelPosMag;

            // Check for solutions. If the discriminant (b*b)-(4*a*c) is:
            // >0: two real solutions - get the maximum one (the other will be a negative time value and can be discarded)
            // =0: one real solution (both solutions the same so either one will do)
            // <0; two imaginary solutions - never will hit the target
            float discriminant = (b * b) - (4 * a * c);

            if (discriminant <= 0) return targetPosition;

            // Get the intercept time by solving the quadratic equation. Note that if a = 0 then we will be 
            // trying to divide by zero. But in that case no quadratics are necessary, the equation will be first-order
            // and can simply be rearranged to get the intercept time.
            float interceptTime;
            if (a != 0)
                interceptTime = Mathf.Max(((-1 * b) - Mathf.Sqrt(discriminant)) / (2 * a), ((-1 * b) + Mathf.Sqrt(discriminant)) / (2 * a));
            else
            {
                interceptTime = -c / (2 * b);
            }

            return (targetPosition + (targetVelocity * interceptTime));
        }


        protected void UpdateLeadTargetPosition()
        {

            // Update the lead target information
            if (target != null)
            {
                Vector3 targetPos = useTargetBoundsCenter ? target.transform.TransformPoint(target.TrackingBounds.center) : target.transform.position;

                if (Mathf.Approximately(interceptSpeed, 0))
                {
                    leadTargetPosition = targetPos;
                    return;
                }

                Vector3 targetVelocity = Vector3.zero;
                if (target.variablesDictionary.ContainsKey(targetVelocityKey))
                {
                    targetVelocity = target.variablesDictionary[targetVelocityKey].Vector3Value;
                }

                leadTargetPosition = GetLeadPosition(transform.position, interceptSpeed, targetPos, targetVelocity);

                onLeadTargetPositionUpdated.Invoke(leadTargetPosition);
            }
        }

        // Called every frame
        protected virtual void Update()
        {
            UpdateLeadTargetPosition();
        }
    }
}