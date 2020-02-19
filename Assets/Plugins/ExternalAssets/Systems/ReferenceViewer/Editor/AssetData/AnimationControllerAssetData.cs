using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ReferenceViewer
{
    [System.Serializable]
    public class AnimationControllerAssetData : AssetData
    {
        public new const string extension = ".controller";
        private readonly AnimatorController animatorController;

        public AnimationControllerAssetData(string assetPath) : base(assetPath)
        {
            animatorController =
                AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimatorController)) as AnimatorController;
        }

        public override void AddAssetData(Object obj)
        {
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            reference.Add(guid);
            foreach (var animationClip in animatorController.animationClips)
            {
                AddReference(animationClip);
            }

            foreach (var animatorControllerLayer in animatorController.layers)
            {
                AddReferenceForStateMachine(animatorControllerLayer.stateMachine);
            }
        }

        private void AddReferenceForStateMachine(AnimatorStateMachine stateMachine)
        {
            foreach (var stateMachineBehaviour in stateMachine.states
                .SelectMany(childAnimatorState => childAnimatorState.state.behaviours))
            {
                AddReference(stateMachineBehaviour);
            }

            foreach (var stateMachineBehaviour in stateMachine.behaviours)
            {
                AddReference(stateMachineBehaviour);
            }

            foreach (var childAnimatorStateMachine in stateMachine.stateMachines)
            {
                AddReferenceForStateMachine(childAnimatorStateMachine.stateMachine);
            }
        }
    }
}