using System.Collections.Generic;
using UnityEngine;

namespace ReferenceViewer
{
    [System.Serializable]
    public class Data : ScriptableObject
    {
        public List<AssetData> assetData = new List<AssetData>();
    }

    [System.Serializable]
    public class SceneData
    {
        public string guid;
        public string typeName;
        public string name;
    }

    [System.Serializable]
    public class SubAssetData
    {
        public string guid = "";
        public string name = "";
        public string typeName = "";
    }
}