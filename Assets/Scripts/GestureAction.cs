using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
namespace ICL.HoloLens.Unity
{
    public class GestureAction : MonoBehaviour
    {

        public float Sensitivity = 2f;
        private Vector3 manipulationPreviousPosition;
        private float rotationFactorX;
        private float rotationFactorY;
        private int meshNumber;
        private int meshCount;

        // Update is called once per frame
        void Update()
        {
            PerformRotation();
        }

        private void PerformRotation()
        {
            if (GestureManager.Instance.IsNavigating)
            {
                rotationFactorX = GestureManager.Instance.NavigationPosition.x * Sensitivity;
                rotationFactorY = GestureManager.Instance.NavigationPosition.y * Sensitivity;
                transform.Rotate(new Vector3(0, -1 * rotationFactorX, 0));
                transform.Rotate(new Vector3(-1 * rotationFactorY, 0, 0));
            }
        }

        void PerformManiplationStart(Vector3 position)
        {
            manipulationPreviousPosition = position;
            if (GestureManager.Instance.IsManipulating)
            {
                meshNumber = ++meshCount;
                WorldAnchorManager.Instance.RemoveAnchor(this.gameObject);
                // can add a rigid body here to enable "hit surfaces" etc.
            }
        }

        
        void PerformManipulationUpdate(Vector3 position)
        {
            if (GestureManager.Instance.IsManipulating)
            {
                Vector3 moveVector = Vector3.zero;

                moveVector = position - manipulationPreviousPosition;

                manipulationPreviousPosition = position;

                transform.position += moveVector;
                meshNumber++;
            }

        }

        void PerformManipulationCompleted()
        {
            WorldAnchorManager.Instance.AttachAnchor(this.gameObject, this.meshNumber.ToString());
        }



    }
}

