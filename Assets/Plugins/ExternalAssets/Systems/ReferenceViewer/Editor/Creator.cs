using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReferenceViewer
{
    public class Creator
    {
        public static void Build(Action callback = null)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var currentScenes = EditorSceneManager.GetSceneManagerSetup().Select(sm => sm.path).ToArray();


            Generate.Build(AssetDatabase.GetAllAssetPaths(), assetData =>
            {
                var data = ScriptableObject.CreateInstance<Data>();

                data.assetData.AddRange(assetData);
                Export(data);

                if (!currentScenes.Any())
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                else
                {
                    EditorSceneManager.OpenScene(currentScenes[0]);

                    foreach (var currentScene in currentScenes.Skip(0))
                    {
                        EditorSceneManager.OpenScene(currentScene, OpenSceneMode.Additive);
                    }
                }

                EditorUtility.UnloadUnusedAssetsImmediate();
                if (callback != null)
                    callback();
            });
        }

        private static void Export(Data data)
        {
            data.assetData = data.assetData.OrderBy(d => Path.GetExtension(d.assetPath)).ToList();
            const string directory = "build/ReferenceViewer";

            Directory.CreateDirectory(directory);

            foreach (var assetData in data.assetData.Where(assetData => assetData.sceneData.Count != 0))
            {
                assetData.sceneData =
                    assetData.sceneData.Distinct(new CompareSelector<SceneData, string>(s => s.name + s.guid)).ToList();
            }
            File.Delete(directory + "/data.dat");
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {data},
                directory + "/data.dat", true);
            AssetDatabase.CreateAsset(data, "Assets/Test.asset");
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}