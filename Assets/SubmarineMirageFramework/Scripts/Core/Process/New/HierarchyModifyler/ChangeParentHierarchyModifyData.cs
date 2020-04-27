//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UnityEngine;
	using UniRx.Async;
	using Extension;
	using Utility;
	using Type = ProcessBody.Type;
	using LifeSpan = ProcessBody.LifeSpan;
	using RanState = ProcessBody.RanState;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ChangeParentHierarchyModifyData : HierarchyModifyData {
		Transform _parent;
		bool _isWorldPositionStays;


		public ChangeParentHierarchyModifyData( ProcessHierarchy hierarchy, Transform parent,
											bool isWorldPositionStays
		) {
			_hierarchy = hierarchy;
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
		}


		protected override async UniTask Run() {
			_hierarchy._owner.transform.SetParent( _parent, _isWorldPositionStays );

			if ( _hierarchy == _hierarchy._top ) {
				_owner.Gets( _hierarchy._type )
					.Remove( _hierarchy );
			}

			_hierarchy.SetParent(
				_hierarchy._owner.GetComponentInParentUntilOneHierarchy<MonoBehaviourProcess>( true )
					?._hierarchy
			);

			if ( _hierarchy._parent == null ) {
				_hierarchy.ResetTop();

			} else {
// TODO : 親属性と、新規子供達の属性を考慮し、新規属性を設定
//				_hierarchy._parent.SetAllHierarchiesData();
				_hierarchy.GetHierarchiesInChildren().ForEach( h => {
					h._top = _hierarchy._parent._top;
					h._type = _hierarchy._parent._type;
					h._lifeSpan = _hierarchy._parent._lifeSpan;
					h._scene = _hierarchy._parent._scene;
				} );
			}

			await UniTaskUtility.DontWait();
		}
	}
}