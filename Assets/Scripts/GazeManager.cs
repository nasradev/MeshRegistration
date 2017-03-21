using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICL.HoloLens.Unity
{
    //Code copied from Microsoft Holographic Academy
    public class GazeManager : Singleton<GazeManager>
    {
        [Tooltip("Maximum gaze distance for calculating a hit.")]
        public float MaxGazeDistance = 5.0f;

        [Tooltip("Select the layers raycast should target.")]
        public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Physics.Raycast result is true if it hits a Hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// HitInfo property gives access
        /// to RaycastHit public members.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position of the user's gaze.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// RaycastHit Normal direction.
        /// </summary>
        public Vector3 Normal { get; private set; }

        private GazeStabilizer gazeStabilizer;
        public Vector3 GazeOrigin { get; private set; }
        private Vector3 gazeDirection;

        void Awake()
        {
            gazeStabilizer = GetComponent<GazeStabilizer>();
        }

        private void Update()
        {
            GazeOrigin = Camera.main.transform.position;

            gazeDirection = Camera.main.transform.forward;

            gazeStabilizer.UpdateHeadStability(GazeOrigin, Camera.main.transform.rotation);

            GazeOrigin = gazeStabilizer.StableHeadPosition;

            UpdateRaycast();
        }

        /// <summary>
        /// Calculates the Raycast hit position and normal.
        /// </summary>
        private void UpdateRaycast()
        {
            RaycastHit hitInfo;

            Hit = Physics.Raycast(GazeOrigin,
                           gazeDirection,
                           out hitInfo,
                           MaxGazeDistance,
                           RaycastLayerMask);

            HitInfo = hitInfo;

            if (Hit)
            {
                // If raycast hit a hologram...

                Position = hitInfo.point;
                Normal = hitInfo.normal;
            }
            else
            {
                // If raycast did not hit a hologram...
                // Save defaults ...
                Position = GazeOrigin + (gazeDirection * MaxGazeDistance);
                Normal = gazeDirection;
            }
        }
    }
}

