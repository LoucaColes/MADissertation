﻿// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Editor
{
    using Rewired;
    using UnityEditor;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [CustomEditor(typeof(Initializer))]
    public sealed class InitializerInspector : CustomInspector_External
    {
        private void OnEnable()
        {
            internalEditor = new InitializerInspector_Internal(this);
            internalEditor.OnEnable();
        }
    }
}