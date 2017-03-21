using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICL.HoloLens.Unity
{
    public class Interactible : Singleton<Interactible>
    {
        private Material[] defaultMaterials;
        // Use this for initialization
        void Start()
        {
            defaultMaterials = GetComponent<Renderer>().materials;

            // Try to add a box collider in case we dont have one
            Collider collider = GetComponentInChildren<Collider>();
            if (collider == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }

        void GazeEntered()
        {
            //Highlight the materials we have gazed upon
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                defaultMaterials[i].SetFloat("_Highlight", .25f);
            }
        }

        void GazeExited()
        {
            //Remove highlights on the materials we have gazed upon
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                defaultMaterials[i].SetFloat("_Highlight", 0f);
            }
        }

        void OnSelect()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                defaultMaterials[i].SetFloat("_Highlight", 0.5f);
            }

            //this.SendMessage("PerformTagAlong");
        }
    }
}

