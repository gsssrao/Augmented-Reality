/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// partial class that also sets eyewear factory
    /// </summary>
    public partial class ComponentFactoryStarterBehaviour : MonoBehaviour
    {
        [FactorySetter]
        void SetEyewearComponentFactory()
        {
            Debug.Log("Setting EyewearComponentFactory");
            EyewearComponentFactory.Instance = new VuforiaEyewearComponentFactory();
        }
    }

    public class VuforiaEyewearComponentFactory : IEyewearComponentFactory
    {
        // type safe implementation to resolve OVRInitController type despite it being not a precompiled script
        public Type GetOVRInitControllerType()
        {
            return typeof (OVRInitController);
        }
    }
}
