/*==============================================================================
Copyright (c) 2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vuforia.EditorClasses
{
    /// <summary>
    /// Editor of the OVRInitController
    /// </summary>
    [CustomEditor(typeof(OVRInitController), true)]
    public class OVRInitControllerEditor : Editor
    {
        private const string INTEGRATION_DEFINE = "ENABLE_VUFORIA_OCULUS_INTEGRATION";
        private const string OVR_CAMERA_CONTROLLER_SCRIPT = "OVRCameraController.cs";

        #region UNITY_EDITOR_METHODS

        /// <summary>
        /// Executed everytime the gameobject is selected
        /// </summary>
        public void OnEnable()
        {
            OVRInitController ovrInitController = (OVRInitController)target;

            if (!ovrInitController.IsInitialized)
            {

#if !ENABLE_VUFORIA_OCULUS_INTEGRATION
                // if the OVR SDK is present, set the scripting define to enable integration
                if (CheckOVRSDK())
                {
                    AddIntegrationScriptingDefine(INTEGRATION_DEFINE);
                }
                else
                {
                    // the OVR SDK was not detected - do not attempt to add everything automatically again.
                    // an error will be shown in the inspector instead.
                    ovrInitController.SetInitialized();
                }
#else
                // automatically add OVR scripts if they are not set up yet:
                if (!OVRSupportAdded(ovrInitController.gameObject))
                    AddOVRSupport(ovrInitController.gameObject);

                // initialization finished.
                ovrInitController.SetInitialized();
#endif
            }
        }

        /// <summary>
        /// draws the inspector for the component
        /// </summary>
        public override void OnInspectorGUI()
        {
#if !ENABLE_VUFORIA_OCULUS_INTEGRATION
            if (CheckOVRSDK())
            {
                AddIntegrationScriptingDefine(INTEGRATION_DEFINE);
            }
            else
            {
                // the OVR SDK was not detected - show an error
                EditorGUILayout.HelpBox("Please import the Oculus SDK into your project to enable automatic integration with Vuforia. " +
                                        "Please see the developer library for more information: developer.vuforia.com", MessageType.Error);
            }
#else
            OVRInitController ovrInitController = (OVRInitController)target;
            if (OVRSupportAdded(ovrInitController.gameObject))
            {
                EditorGUILayout.HelpBox("Oculus integration is correctly set up.", MessageType.None);

                if (GUILayout.Button("Remove OVR scripts from ARCamera"))
                {
                    RemoveOVRSupport(ovrInitController.gameObject);
                }

                if (GUILayout.Button("Remove this component and OVR scripts from ARCamera"))
                {
                    RemoveOVRInitController(ovrInitController);
                }
            }
            else
            {
                if (GUILayout.Button("Add OVR scripts to ARCamera"))
                {
                    AddOVRSupport(ovrInitController.gameObject);
                }

                if (GUILayout.Button("Remove this component from ARCamera"))
                {
                    RemoveOVRInitController(ovrInitController);
                }
            }
#endif
        }

        #endregion // UNITY_EDITOR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Removes the ENABLE_VUFORIA_OCULUS_INTEGRATION define from all build targets.
        /// Needs to be called explicitly by developer if required.
        /// </summary>
        public static void RemoveIntegrationScriptingDefine()
        {
            IEnumerable<BuildTargetGroup> buildTargetGroups = Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>();

            foreach (BuildTargetGroup buildTargetGroup in buildTargetGroups)
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                if (defines.Contains(INTEGRATION_DEFINE))
                {
                    if (defines.Contains(";" + INTEGRATION_DEFINE))
                        defines = defines.Replace(";" + INTEGRATION_DEFINE, ""); // remove with separator if one exists
                    else
                        defines = defines.Replace(INTEGRATION_DEFINE, "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                }
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS

        /// <summary>
        /// Checks if the OVR SDK exists in the current project
        /// </summary>
        private static bool CheckOVRSDK()
        {
            return File.Exists(Path.Combine(Path.Combine(Path.Combine("Assets", "OVR"), "Scripts"), OVR_CAMERA_CONTROLLER_SCRIPT));
        }

        /// <summary>
        /// Adds a #if define for all build targets
        /// Unity editor scripting is awesome!
        /// </summary>
        private static void AddIntegrationScriptingDefine(string define)
        {
            IEnumerable<BuildTargetGroup> buildTargetGroups = Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>();

            foreach (BuildTargetGroup buildTargetGroup in buildTargetGroups)
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                if (!defines.Contains(define))
                {
                    if (!defines.Equals(string.Empty))
                        defines += ";"; // add a separator if there are already other defines
                    defines += define;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                }
            }
        }

        /// <summary>
        /// Removes OVRInitController, any OVR scripts on the ARCamera and 
        /// the script define to enalbe OVR support.
        /// </summary>
        /// <param name="ovrInitController"></param>
        private static void RemoveOVRInitController(OVRInitController ovrInitController)
        {
            RemoveOVRSupport(ovrInitController.gameObject);
            Undo.DestroyObjectImmediate(ovrInitController);
        }

        /// <summary>
        /// Checks if OVR scripts are added to the ARCamera
        /// </summary>
        private static bool OVRSupportAdded(GameObject go)
        {
            bool cameraControllerExists = false;
            bool deviceExists = false;
            bool camerasCorrectlySetup = true;
            
            if (OVRInitController.OvrCameraControllerType != null)
                if (go.GetComponent(OVRInitController.OvrCameraControllerType) != null)
                    cameraControllerExists = true;

            if (OVRInitController.OvrDeviceType != null)
                if (go.GetComponent(OVRInitController.OvrDeviceType) != null)
                    deviceExists = true;

            foreach (Camera camera in go.GetComponentsInChildren<Camera>())
            {
                bool cameraExists = false;
                bool lenscorrectionExists = false;

                if (OVRInitController.OvrCameraType != null)
                    if (camera.gameObject.GetComponent(OVRInitController.OvrCameraType) != null)
                        cameraExists = true;

                if (OVRInitController.OvrLensCorrectionType != null)
                    if (camera.gameObject.GetComponent(OVRInitController.OvrLensCorrectionType) != null)
                        lenscorrectionExists = true;

                camerasCorrectlySetup = camerasCorrectlySetup && (cameraExists && lenscorrectionExists);
            }

            return cameraControllerExists && deviceExists && camerasCorrectlySetup;
        }

        /// <summary>
        /// Adds OVR scripts to ARCamera
        /// </summary>
        private static void AddOVRSupport(GameObject go)
        {
            // register undo group
            Undo.IncrementCurrentGroup();

            // add OVR scripts to ARCamera:
            if (OVRInitController.OvrCameraControllerType != null)
            {
                if (go.GetComponent(OVRInitController.OvrCameraControllerType) == null)
                    Undo.AddComponent(go, OVRInitController.OvrCameraControllerType);
            }

            if (OVRInitController.OvrDeviceType != null)
            {
                if (go.GetComponent(OVRInitController.OvrDeviceType) == null)
                    Undo.AddComponent(go, OVRInitController.OvrDeviceType);
            }

            // add scripts to child camera objects
            foreach (Camera camera in go.GetComponentsInChildren<Camera>())
            {
                if (OVRInitController.OvrCameraType != null)
                {
                    if (camera.gameObject.GetComponent(OVRInitController.OvrCameraType) == null)
                        Undo.AddComponent(camera.gameObject, OVRInitController.OvrCameraType);
                }

                if (OVRInitController.OvrLensCorrectionType != null)
                {
                    if (camera.gameObject.GetComponent(OVRInitController.OvrLensCorrectionType) == null)
                        Undo.AddComponent(camera.gameObject, OVRInitController.OvrLensCorrectionType);
                }
            }

            // set up cameras correctly:
            SetOVRParameters(go);

            EditorUtility.SetDirty(go);
        }

        /// <summary>
        /// sets various parameters on the Oculus scripts
        /// </summary>
        private static void SetOVRParameters(GameObject arCameraGameObject)
        {
#if ENABLE_VUFORIA_OCULUS_INTEGRATION
            QCARAbstractBehaviour qcarBehaviour = arCameraGameObject.GetComponent<QCARAbstractBehaviour>();
            // set black background color
            OVRCameraController ovrCameraController = arCameraGameObject.GetComponent<OVRCameraController>();
            ovrCameraController.BackgroundColor = Color.black;
            ovrCameraController.FlipCorrectionInY = true;
            
            // set up cameras as Oculus expects them to be:
            qcarBehaviour.PrimaryCamera.depth = 1;
            OVRCamera leftOVRCamera = qcarBehaviour.PrimaryCamera.GetComponent<OVRCamera>();
            leftOVRCamera.RightEye = false;
            
            qcarBehaviour.SecondaryCamera.depth = 0;
            OVRCamera rightOVRCamera = qcarBehaviour.SecondaryCamera.GetComponent<OVRCamera>();
            rightOVRCamera.RightEye = true;
#endif
        }

        /// <summary>
        /// Removes OVR scripts from ARCamera
        /// </summary>
        private static void RemoveOVRSupport(GameObject go)
        {
            // register undo group
            Undo.IncrementCurrentGroup();

            // remove OVR scripts from ARCamera:
            if (OVRInitController.OvrCameraControllerType != null)
            {
                Component cameraControllerComponent = go.GetComponent(OVRInitController.OvrCameraControllerType);
                if (cameraControllerComponent != null)
                    Undo.DestroyObjectImmediate(cameraControllerComponent);
            }

            if (OVRInitController.OvrDeviceType != null)
            {
                Component deviceComponent = go.GetComponent(OVRInitController.OvrDeviceType);
                if (deviceComponent != null)
                    Undo.DestroyObjectImmediate(deviceComponent);
            }

            // remove scripts from child camera objects
            foreach (Camera camera in go.GetComponentsInChildren<Camera>())
            {
                if (OVRInitController.OvrCameraType != null)
                {
                    Component cameraComponent = camera.gameObject.GetComponent(OVRInitController.OvrCameraType);
                    if (cameraComponent != null)
                        Undo.DestroyObjectImmediate(cameraComponent);
                }

                if (OVRInitController.OvrLensCorrectionType != null)
                {
                    Component lensCorrectionComponent = camera.gameObject.GetComponent(OVRInitController.OvrLensCorrectionType);
                    if (lensCorrectionComponent != null)
                        Undo.DestroyObjectImmediate(lensCorrectionComponent);
                }
            }

            EditorUtility.SetDirty(go);
        }

        #endregion // PRIVATE_METHODS
    }
}
