using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace ICL.HoloLens.Unity
{
    // Keeps track of when a hand is detected
    public class HandsManager : Singleton<HandsManager>
    {
        public bool HandDetected { get; private set; }
        public GameObject FocusedGameObject { get; private set; }
        private void Awake()
        {
            InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            InteractionManager.SourceLost += InteractionManager_SourceLost;

            InteractionManager.SourcePressed += InteractionManager_SourcePressed;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;

            FocusedGameObject = null;
        }

        private void InteractionManager_SourceReleased(InteractionSourceState state)
        {
            ResetFocusedGameObject();
        }

        private void InteractionManager_SourcePressed(InteractionSourceState state)
        {
            // Are we at a gameobject?
            if(InteractibleManager.Instance.FocusedGameObject != null)
            {
                FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            }
        }

        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            HandDetected = false;
            //ResetFocusedGameObject();
        }

        private void InteractionManager_SourceDetected(InteractionSourceState state)
        {
            HandDetected = true;
        }

        private void ResetFocusedGameObject()
        {
            FocusedGameObject = null;
            GestureManager.Instance.ResetGestureRecognizers();
        }

        // Clean up
        private void OnDestroy()
        {
            InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
            InteractionManager.SourcePressed -= InteractionManager_SourcePressed;
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
        }
    }
}

