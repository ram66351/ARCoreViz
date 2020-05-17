//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGenerator.cs" company="Google">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using GoogleARCore.Examples.ObjectManipulation;
    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// </summary>
    public class DetectedPlaneGenerator : MonoBehaviour
    {
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;
        bool  isSurfaceDetected = true;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is
        /// used across the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        /// 
        private void Start()
        {
            Covid19Viz.StopPlaneDetectionEvent += StopTracking;
        }

        private void OnApplicationQuit()
        {
            Covid19Viz.StopPlaneDetectionEvent -= StopTracking;
        }

        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking || PawnManipulator.isPlaneChoosen)
            {
                return;
            }

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to
            // visualize them.
            if(isSurfaceDetected)
            {
                Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
                for (int i = 0; i < m_NewPlanes.Count; i++)
                {
                    // Instantiate a plane visualization prefab and set it to track the new plane. The
                    // transform is set to the origin with an identity rotation since the mesh for our
                    // prefab is updated in Unity World coordinates.
                    GameObject planeObject =
                        Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                    planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
                }
            }
        }

        public void StopTracking()
        {
            // Make isSurfaceDetected to false to disable plane detection code
            isSurfaceDetected = false;
            // Tag DetectedPlaneVisualizer prefab to Plane(or anything else)
            GameObject[] anyName = GameObject.FindGameObjectsWithTag("Plane");
            // In DetectedPlaneVisualizer we have multiple polygons so we need to loop and diable DetectedPlaneVisualizer script attatched to that prefab.
            for (int i = 0; i < anyName.Length; i++)
            {
                anyName[i].GetComponent<DetectedPlaneVisualizer>().enabled = false;
            }

        }
    }
}
