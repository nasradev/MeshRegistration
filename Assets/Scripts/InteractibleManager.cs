using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICL.HoloLens.Unity
{
    public class InteractibleManager : Singleton<InteractibleManager>
    {

        public GameObject FocusedGameObject { get; private set; }

        private GameObject oldFocusedGameObject = null;

        void Start()
        {
            FocusedGameObject = null;
        }

        void Update()
        {
            oldFocusedGameObject = FocusedGameObject;

            if (GazeManager.Instance.Hit)
            {
                RaycastHit hitInfo = GazeManager.Instance.HitInfo;
                if (hitInfo.collider != null)
                {
                    FocusedGameObject = hitInfo.collider.gameObject;
                }
                else
                {
                    FocusedGameObject = null;
                }
            }
            else
            {
                FocusedGameObject = null;
            }

            if (FocusedGameObject != oldFocusedGameObject)
            {
                ResetFocusedInteractible();

                if (FocusedGameObject != null)
                {
                    if (FocusedGameObject.GetComponent<Interactible>() != null)
                    {
                        FocusedGameObject.SendMessage("GazeEntered");
                    }
                }
            }
        }

        private void ResetFocusedInteractible()
        {
            if (oldFocusedGameObject != null)
            {
                if (oldFocusedGameObject.GetComponent<Interactible>() != null)
                {
                    oldFocusedGameObject.SendMessage("GazeExited");
                }
            }
        }
    }
}

