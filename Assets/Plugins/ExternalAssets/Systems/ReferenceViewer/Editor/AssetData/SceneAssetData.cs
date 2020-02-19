using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ReferenceViewer
{
    [System.Serializable]
    public class SceneAssetData : AssetData
    {
        public new const string extension = ".unity";

        public SceneAssetData(string assetPath) : base(assetPath)
        {
        }

        public override void AddAssetData(Object obj)
        {
            var scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Single);
            var scenGuid = AssetDatabase.AssetPathToGUID(assetPath);

            foreach (GameObject go in Object.FindObjectsOfType(typeof(GameObject)))
            {
                CollectDependencies(go, new SerializedObject(go), scenGuid);
            }
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}