using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace ICL.HoloLens.Unity
{
    public class GestureManager : Singleton<GestureManager>
    {
        //Set an empty gesture recognizer so we can block any gestures until voice commanded.
        public GestureRecognizer EmptyGestureRecognizer { get; private set; }
        public GestureRecognizer NavigationRecognizer { get; private set; }
        public GestureRecognizer ManipulationRecognizer { get; private set; }
        public bool IsNavigating { get; private set; }
        public Vector3 NavigationPosition { get; private set; }
        public bool IsManipulating { get; private set; }
        public Vector3 ManipulationPosition { get; private set; }

        // Currently active gesture recognizer.
        public GestureRecognizer ActiveRecognizer { get; private set; }
        private void Awake()
        {
            // Recognise navigations (i.e. pinch + drag)
            NavigationRecognizer = new GestureRecognizer();
            // Now we can recognise tap gestures, and dragging on the X and Y.
            NavigationRecognizer.SetRecognizableGestures(
                GestureSettings.Tap |
                GestureSettings.NavigationX |
                GestureSettings.NavigationY);
            // Here we add the handlers for the various gestures.
            NavigationRecognizer.TappedEvent += NavigationRecognizer_TappedEvent;

            // We will use these for e.g. moving an object. The actual activity is handles in a 'listener' script
            NavigationRecognizer.NavigationStartedEvent += NavigationRecognizer_NavigationStartedEvent;
            NavigationRecognizer.NavigationUpdatedEvent += NavigationRecognizer_NavigationUpdatedEvent;
            NavigationRecognizer.NavigationCompletedEvent += NavigationRecognizer_NavigationCompletedEvent;
            NavigationRecognizer.NavigationCanceledEvent += NavigationRecognizer_NavigationCanceledEvent;

            // TODO: Check the ManipulationRecognizer's functions.
            // Instantiate the ManipulationRecognizer.
            ManipulationRecognizer = new GestureRecognizer();

            // Add the ManipulationTranslate GestureSetting to the ManipulationRecognizer's RecognizableGestures.
            ManipulationRecognizer.SetRecognizableGestures(
                GestureSettings.ManipulationTranslate);

            // Register for the Manipulation events on the ManipulationRecognizer.
            // We will use these for e.g. rotating an object. The actual activity is handles in a 'listener' script
            ManipulationRecognizer.ManipulationStartedEvent += ManipulationRecognizer_ManipulationStartedEvent;
            ManipulationRecognizer.ManipulationUpdatedEvent += ManipulationRecognizer_ManipulationUpdatedEvent;
            ManipulationRecognizer.ManipulationCompletedEvent += ManipulationRecognizer_ManipulationCompletedEvent;
            ManipulationRecognizer.ManipulationCanceledEvent += ManipulationRecognizer_ManipulationCanceledEvent;

            // The default one doesn't do anything.
            EmptyGestureRecognizer = new GestureRecognizer();

            EmptyGestureRecognizer.TappedEvent += EmptyGestureRecognizer_TappedEvent;
            // Transition to the currently active gesture recognizer
            ResetGestureRecognizers();
        }



        private void OnDestroy()
        {
            NavigationRecognizer.TappedEvent -= NavigationRecognizer_TappedEvent;
            NavigationRecognizer.NavigationStartedEvent -= NavigationRecognizer_NavigationStartedEvent;
            NavigationRecognizer.NavigationUpdatedEvent -= NavigationRecognizer_NavigationUpdatedEvent;
            NavigationRecognizer.NavigationCompletedEvent -= NavigationRecognizer_NavigationCompletedEvent;
            NavigationRecognizer.NavigationCanceledEvent -= NavigationRecognizer_NavigationCanceledEvent;
           

            // Register for the Manipulation events on the ManipulationRecognizer.
            ManipulationRecognizer.ManipulationStartedEvent -= ManipulationRecognizer_ManipulationStartedEvent;
            ManipulationRecognizer.ManipulationUpdatedEvent -= ManipulationRecognizer_ManipulationUpdatedEvent;
            ManipulationRecognizer.ManipulationCompletedEvent -= ManipulationRecognizer_ManipulationCompletedEvent;
            ManipulationRecognizer.ManipulationCanceledEvent -= ManipulationRecognizer_ManipulationCanceledEvent;
        }

        public void ResetGestureRecognizers()
        {
            Transition(EmptyGestureRecognizer);
        }

        public void Transition(GestureRecognizer newRecognizer)
        {
            if (newRecognizer == null)
            {
                return;
            }

            if (ActiveRecognizer != null)
            {
                if (ActiveRecognizer == newRecognizer)
                {
                    return;
                }

                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StopCapturingGestures();
            }

            newRecognizer.StartCapturingGestures();
            ActiveRecognizer = newRecognizer;
        }

        private void EmptyGestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            //TODO - don't really need this to do anything right now.
        }
        // Navigation Recognizer Events
        private void NavigationRecognizer_NavigationCanceledEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            IsNavigating = false;
            //TODO: see if we need to clean up anything here.
        }

        private void NavigationRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            IsNavigating = false;
        }

        private void NavigationRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            IsNavigating = true;
            NavigationPosition = normalizedOffset;
        }

        private void NavigationRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            IsNavigating = true;
            NavigationPosition = normalizedOffset;
        }

        private void NavigationRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            GameObject focusedObject = InteractibleManager.Instance.FocusedGameObject;
            if(focusedObject != null)
            {
                focusedObject.SendMessageUpwards("OnSelect");
            }
        }


        // Manipulation Recognizer Events
        GameObject storedObject;
        private void ManipulationRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            IsManipulating = false;
            //if (storedObject != null)
            //{
            //    storedObject.SendMessageUpwards("PerformManipulationCompleted");
            //}
        }

        private void ManipulationRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            IsManipulating = false;
            //TODO: Problem is the below sends a message to save the position as anchor.
            // So if it doesn't execute we won't save it properly.
            //HandsManager.Instance.FocusedGameObject.SendMessageUpwards("PerformManipulationCompleted");
            if(storedObject != null)
            {
                storedObject.SendMessageUpwards("PerformManipulationCompleted");
            }
        }

        private void ManipulationRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            //Check we are manipulating an object and if so, call its manipulation script on the position
            if (HandsManager.Instance.FocusedGameObject != null)
            {
                storedObject = HandsManager.Instance.FocusedGameObject;
                IsManipulating = true;
                ManipulationPosition = cumulativeDelta;
                HandsManager.Instance.FocusedGameObject.SendMessageUpwards("PerformManipulationUpdate", cumulativeDelta);
            }
        }

        private void ManipulationRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            //Check we are manipulating an object and if so, call its manipulation script on the position
            if (HandsManager.Instance.FocusedGameObject != null)
            {
                storedObject = HandsManager.Instance.FocusedGameObject;
                IsManipulating = true;
                ManipulationPosition = cumulativeDelta;
                HandsManager.Instance.FocusedGameObject.SendMessageUpwards("PerformManiplationStart", cumulativeDelta);
            }
        }
    }

}

