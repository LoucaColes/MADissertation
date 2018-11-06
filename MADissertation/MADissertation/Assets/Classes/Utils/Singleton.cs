// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Singleton behaviour class, used for components that should only have one instance.
    /// <remarks>Singleton classes live on through scene transitions and will mark their
    /// parent root GameObject with <see cref="Object.DontDestroyOnLoad"/></remarks>
    /// </summary>
    /// <typeparam name="T">The Singleton Type</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static bool searchForInstance = true;

        [SerializeField]
        private bool m_destroyOnLoad = false;

        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// If no instance is found, then we search for an instance
        /// in the scene.
        /// If more than one instance is found, we throw an error and
        /// no instance is returned.
        /// </summary>
        public static T Instance
        {
            get
            {
                // Check if the instance is not initialised and
                // search for instance is true
                if (!IsInitialized && searchForInstance)
                {
                    // Set search for instance to false
                    searchForInstance = false;

                    // Find all objects of instance type
                    T[] objects = FindObjectsOfType<T>();

                    // If the length is 1 set the instance to the first element
                    if (objects.Length == 1)
                    {
                        instance = objects[0];
                    }
                    // Else display error message
                    else if (objects.Length > 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}.", typeof(T).Name, objects.Length);
                    }
                }
                // Return instance
                return instance;
            }
        }

        /// <summary>
        /// Assert the is initialised variable
        /// </summary>
        public static void AssertIsInitialized()
        {
            Debug.Assert(IsInitialized, string.Format("The {0} singleton has not been initialized.", typeof(T).Name));
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance.
        /// Called by Unity when initializing a MonoBehaviour.
        /// Scripts that extend Singleton should be sure to call base.Awake() to ensure the
        /// static Instance reference is properly created.
        /// </summary>
        protected virtual void Awake()
        {
            // If initialised and instance does not equal this
            if (IsInitialized && instance != this)
            {
                // Check if in editor
                if (Application.isEditor)
                {
                    // Destroy this instance
                    DestroyImmediate(this);
                }
                else
                {
                    // Destroy this instance
                    Destroy(this);
                }

                // Display error message
                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}. Additional Instance was destroyed", GetType().Name);
            }
            // If not initialised
            else if (!IsInitialized)
            {
                // Set instance to this instance
                instance = (T)this;

                // Set search for instance to false
                searchForInstance = false;

                // Set dont destoy
                SetDontDestroy();
            }
        }

        /// <summary>
        /// Set dont destroy on load on root object of instance
        /// </summary>
        private void SetDontDestroy()
        {
            if (!m_destroyOnLoad)
            {
                DontDestroyOnLoad(instance.gameObject.transform.root);
            }
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance.
        /// Called by Unity when destroying a MonoBehaviour. Scripts that extend
        /// Singleton should be sure to call base.OnDestroy() to ensure the
        /// underlying static Instance reference is properly cleaned up.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }
    }
}