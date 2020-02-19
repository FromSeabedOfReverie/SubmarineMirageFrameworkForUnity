using UnityEngine;

namespace ReferenceViewer
{
    [System.Serializable]
    public class PrefabAssetData : AssetData
    {
        public new const string extension = ".prefab";

        public PrefabAssetData(string assetPath) : base(assetPath)
        {
        }

        public override void AddAssetData(Object obj)
        {
            AddReference(obj);
//            CollectDependencies(obj as GameObject, new SerializedObject(obj), "");
        }
    }
}