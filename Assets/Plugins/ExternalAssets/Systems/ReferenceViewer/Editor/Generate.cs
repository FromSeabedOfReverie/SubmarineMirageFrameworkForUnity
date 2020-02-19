using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReferenceViewer
{
    public class Generate
    {
        private static float progress;


        public static void Build(string[] assetPaths, Action<AssetData[]> callback = null)
        {
            EditorUtility.ClearProgressBar();
            var result = new AssetData[0];
            EditorApplication.LockReloadAssemblies();
            assetPaths = assetPaths.OrderBy(path => Path.GetExtension(path)).ToArray();
            for (var i = 0; i < assetPaths.Length; i++)
            {
                var assetPath = assetPaths[i];

                if (assetPath.StartsWith("Assets/") == false)
                    continue;
                progress = (float) i / assetPaths.Length;

                var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                DisplayProgressBar(assetPath, progress);

                AssetData assetData = null;

                switch (Path.GetExtension(assetPath))
                {
                    case PrefabAssetData.extension:
                        assetData = new PrefabAssetData(assetPath);
                        break;
                    case SceneAssetData.extension:
                        assetData = new SceneAssetData(assetPath);
                        break;
                    case AnimationControllerAssetData.extension:
                        assetData = new AnimationControllerAssetData(assetPath);
                        break;
                    default:
                        assetData = new AssetData(assetPath);
                        break;
                }

                assetData.AddAssetData(obj);
                ArrayUtility.Add(ref result, assetData);
            }
            callback(result);
            EditorApplication.UnlockReloadAssemblies();
            EditorUtility.ClearProgressBar();
        }

        protected static void DisplayProgressBar(string path, float progress)
        {
            EditorUtility.DisplayProgressBar(Path.GetFileName(path),
                Mathf.FloorToInt(progress * 100) + "% - " + Path.GetFileName(path), progress);
        }
    }
}