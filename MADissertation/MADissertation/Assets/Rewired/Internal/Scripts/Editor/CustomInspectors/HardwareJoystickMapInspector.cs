﻿// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor
{
    using Rewired.Data.Mapping;
    using UnityEditor;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [CustomEditor(typeof(HardwareJoystickMap))]
    public sealed class HardwareJoystickMapInspector : CustomInspector_External
    {
        private void OnEnable()
        {
            internalEditor = new HardwareJoystickMapInspector_Internal(this);
            base.Enabled();
        }
    }
}