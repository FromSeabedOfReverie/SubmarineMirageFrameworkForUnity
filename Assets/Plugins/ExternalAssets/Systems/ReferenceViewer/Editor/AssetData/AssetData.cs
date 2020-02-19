using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ReferenceViewer
{
    [System.Serializable]
    public class AssetData
    {
        public const string extension = "";

        public string assetPath;
        public string guid;

        public List<string> reference = new List<string>();
        public List<SubAssetData> subAssets = new List<SubAssetData>();
        public List<SceneData> sceneData = new List<SceneData>();

        public AssetData(string assetPath)
        {
            this.assetPath = assetPath;
            guid = AssetDatabase.AssetPathToGUID(assetPath);
        }

        public virtual void AddAssetData(Object obj)
        {
            AddReference(obj);
        }

        protected void AddReference(Object obj)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            if (string.IsNullOrEmpty(guid) == false)
            {
                if (reference.Contains(guid) == false && this.guid != guid)
                {
                    reference.Add(guid);
                }
            }

            foreach (var o in EditorUtility.CollectDependencies(new[] {obj}))
            {
                if (o == null)
                {
                    continue;
                }
                guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));
                if (string.IsNullOrEmpty(guid) == false)
                {
                    if (reference.Contains(guid) == false  && this.guid != guid)
                    {
                        reference.Add(guid);
                    }
                }
            }
        }

        protected void AddSubAssetReference(Object obj)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            if (string.IsNullOrEmpty(guid) || AssetDatabase.IsSubAsset(obj) == false)
            {
                return;
            }


            var sub = new SubAssetData
            {
                name = obj.name,
                guid = guid,
                typeName = obj.GetType().FullName
            };
            if (subAssets.Count(s => s.guid == sub.guid && s.name == sub.name &&
                                     s.typeName == sub.typeName) == 0)
            {
                subAssets.Add(sub);
            }
        }

        protected void AddSceneReference(GameObject go, Object obj, string sceneGUID)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));

            if (string.IsNullOrEmpty(guid))
            {
                guid = sceneGUID;
            }

            var transform = go.transform;

            sceneData.Add(new SceneData
            {
                guid = guid,
                name = GetPathName(transform),
                typeName = obj.GetType().FullName
            });
        }

        protected void CollectDependencies(GameObject go, SerializedObject so, string scenGuid, bool skipLoop = false)
        {

            var property = so.GetIterator();
            while (property.Next(true))
            {
                if ((property.propertyType == SerializedPropertyType.ObjectReference) &&
                    (property.objectReferenceValue != null))
                {
                    if (skipLoop == false)
                    {
                        CollectDependencies(go, new SerializedObject(property.objectReferenceValue), scenGuid, true);
                    }
                    AddReference(property.objectReferenceValue);
                    AddSceneReference(go, property.objectReferenceValue, scenGuid);
                }
            }
        }
        private static string GetPathName(Transform transform, string name = "")
        {
            while (true)
            {
                name = transform.name + name;
                if (!transform.parent) return name;
                transform = transform.parent;
                name = "/" + name;
            }
        }
    }
}