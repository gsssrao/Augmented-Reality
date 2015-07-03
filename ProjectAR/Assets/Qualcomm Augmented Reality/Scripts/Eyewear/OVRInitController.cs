/*==============================================================================
Copyright (c) 2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/


using System;
using UnityEngine;

namespace Vuforia
{

    /// <summary>
    /// This class enables the integration of Vuforia with the Oculus SDK
    /// To enable it, the Oculus SDK needs to be imported into the Unity project.
    /// Please see the Vuforia developer library at developer.vuforia.com for more details.
    /// </summary>
    public class OVRInitController : MonoBehaviour
    {
        #region STATIC_PROPERTIES

#if ENABLE_VUFORIA_OCULUS_INTEGRATION
        public static readonly Type OvrCameraControllerType = typeof(OVRCameraController);
        public static readonly Type OvrDeviceType = typeof(OVRDevice);
        public static readonly Type OvrCameraType = typeof(OVRCamera);
        public static readonly Type OvrLensCorrectionType = typeof(OVRLensCorrection);
#else
        public static readonly Type OvrCameraControllerType = null;
        public static readonly Type OvrDeviceType = null;
        public static readonly Type OvrCameraType = null;
        public static readonly Type OvrLensCorrectionType = null;
#endif

        #endregion // STATIC_PROPERTIES



        #region PRIVATE_MEMBERS
        
#if ENABLE_VUFORIA_OCULUS_INTEGRATION
        private OVRCameraController mOVRController;
        private QCARBehaviour mQCARBehaviour;
#endif

        [SerializeField]
        [HideInInspector]
        private bool mInitialized;

        #endregion // PRIVATE_MEMBERS



        #region PUBLIC_PROPERTIES

        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        #endregion // PUBLIC_PROPERTIES



        #region PUBLIC_METHODS

        public void SetInitialized()
        {
            mInitialized = true;
        }

        #endregion // PUBLIC_METHODS



        #region UNITY_MONOBEHAVIOUR_METHODS

        
#if ENABLE_VUFORIA_OCULUS_INTEGRATION

        void Awake()
        {
            OVRCameraController ovrCameraController = GetComponent<OVRCameraController>();
            if (ovrCameraController != null)
            {
                mOVRController = ovrCameraController;
                mOVRController.enabled = false;
                // This API disables OVR tracking in the scene while Vuforia is tracking.
                // It needs to be manually added to the Oculus SDK scripts, please see
                // the Vuforia developer library at developer.vuforia.com for more details.
                mOVRController.DisableVrTracking = true;
            }

            QCARBehaviour arCamera = GetComponent<QCARBehaviour>();
            if (arCamera != null)
            {
                mQCARBehaviour = arCamera;
                mQCARBehaviour.enabled = false;
            }
        }

        void Start()
        {
            // make sure that the Oculus SDK is initialized before Vuforia
            mOVRController.enabled = true;
            mQCARBehaviour.enabled = true;
        }

#endif

        #endregion // PUBLIC_METHODS
    }
}
