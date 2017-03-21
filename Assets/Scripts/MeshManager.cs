using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
namespace ICL.HoloLens.Unity
{
    public class MeshManager : MonoBehaviour
    {
        KeywordRecognizer keywordRecognizer;
        // Call this when a keyword is recognized
        delegate void KeywordAction(PhraseRecognizedEventArgs args);
        Dictionary<string, KeywordAction> keywordCollection;
        public Transform prefab;
        bool loaded = false;
        private void Start()
        {
            // Add the phrase(s) we can act on
            keywordCollection = new Dictionary<string, KeywordAction>();
            keywordCollection.Add("Move Mesh", MoveMeshCommand);
            keywordCollection.Add("Rotate Mesh", RotateMeshCommand);
            keywordCollection.Add("Save Mesh", SaveMeshCommand);

            keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();

        }

        // Update is called once per frame
        void Update()
        {
            // Handle the spatial anchoring i.e. restore any saved meshes from before
            if (!loaded && (WorldAnchorManager.Instance.AnchorStore != null))
            {
                String[] ids = WorldAnchorManager.Instance.AnchorStore.GetAllIds();

                foreach (String id in ids)
                {
                    var instance = Instantiate(this.prefab);
                    WorldAnchorManager.Instance.AttachAnchor(instance.gameObject, id);
                }
                loaded = true;
            }
        }
        void OnDestroy()
        {
            keywordRecognizer.Dispose();
        }

        void MoveMeshCommand(PhraseRecognizedEventArgs args)
        {
            // Switch between navigation to manipulation
            GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer); 
        }
        void RotateMeshCommand(PhraseRecognizedEventArgs args)
        {
            // Switch between navigation to manipulation
            GestureManager.Instance.Transition(GestureManager.Instance.NavigationRecognizer); 
        }

        // Save the current position/orientation of the mesh as an anchor
        void SaveMeshCommand(PhraseRecognizedEventArgs args)
        {
            // Stop navigation/manipulation so mesh does not get moved accidentally.
            GestureManager.Instance.Transition(GestureManager.Instance.EmptyGestureRecognizer);

        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            KeywordAction kA;
            if(keywordCollection.TryGetValue(args.text, out kA))
            {
                kA.Invoke(args);
            }
        }
    }

}
