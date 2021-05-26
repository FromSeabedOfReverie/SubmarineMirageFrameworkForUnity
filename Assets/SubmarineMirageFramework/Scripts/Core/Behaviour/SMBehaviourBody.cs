//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Behaviour {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Task.Modifyler;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	public class SMBehaviourBody : SMTask {
		public SMBehaviour _behaviour	{ get; private set; }
		[SMShowLine] public GameObject _gameObject => _behaviour.gameObject;
		public SMScene _scene { get; private set; }

		public override SMTaskRunType _type => _behaviour._type;



#region ToString
		public override void SetToString() {
			base.SetToString();


			_toStringer.SetValue( nameof( _gameObject ), i => _gameObject.name );
			_toStringer.Add( nameof( _behaviour ), i =>
				_toStringer.DefaultValue( _behaviour, i, true ) );

			_toStringer.SetLineValue( nameof( _gameObject ), () => _gameObject.name );
		}
#endregion



		public SMBehaviourBody( SMBehaviour behaviour, SMScene scene ) {
			_behaviour = behaviour;
			_scene = scene;

			_isRequestInitialEnable = IsActiveInHierarchyAndComponent();
			_isCanActiveEvent = () => IsActiveInHierarchy() && IsActiveInComponent();

			_disposables.AddLast( () => {
				_behaviour.Dispose();
				var bs = _gameObject.GetComponents<SMBehaviour>();
				UnityEngine.Object.Destroy( bs.Count() == 1 ? _gameObject : _behaviour );
			} );
		}

		public SMBehaviourBody( SMScene scene, SMObject smObject ) {
			var lastGroup = smObject._groupBody;

			_ranState = lastGroup._ranState;
			_activeState = lastGroup._activeState;

			// 親を解除、新グループを再設定
			smObject.Unlink();
			smObject.SetSceneOfChildren( scene );

			// 親アクティブに一致させる変更を予約
			_manager.Register( new AdjustObjectRunSMGroup( smObject ) );
			lastGroup.ReregisterModifyler( this );  // 元グループの変更を、新グループに再登録

			_scene = scene;
			_manager.Register( new RegisterGroupSMGroupManager( this ) );
		}

		public override void Create()
			=> _behaviour.Create();

		public override void Dispose() => base.Dispose();



		public void SetSceneOfChildren( SMScene scene )
			=> _gameObject.transform.root.GetComponentsInChildren<SMBehaviour>()
				.ForEach( b => b._body._scene = scene );



		public bool IsTop() => _gameObject.transform == _gameObject.transform.root;

		public bool IsActiveInHierarchy()
			=> _gameObject.activeInHierarchy;

		public bool IsActiveInParentHierarchy() {
			var parent = _gameObject.transform.parent;
			if ( parent == null ) { return true; }

			return parent.gameObject.activeInHierarchy;
		}

		public bool IsActiveInComponent()
			=> _behaviour.enabled;

		public bool IsActiveInHierarchyAndComponent()
			=> _gameObject.activeInHierarchy && _behaviour.enabled;



		



		public async UniTask DestroyObject() {
			var bs = _gameObject.GetComponentsInChildren<SMBehaviour>()
				.Reverse()
				.ToArray();
			foreach ( var b in bs ) {
				_manager.Register( new FinalDisableSMTask( b._body ) );
			}
			foreach ( var b in bs ) {
				_manager.Register( new FinalizeSMTask( b._body ) );
			}
			await _manager._modifyler.WaitRunning();
		}

		public async UniTask ChangeActiveObject( bool isActive ) {
			if ( isActive ) { _manager.Register( new EnableObjectSMGroup( this ) ); }
			else { _manager.Register( new DisableObjectSMGroup( this ) ); }
			await _manager._modifyler.WaitRunning();
		}

		public async UniTask ChangeParentObject( Transform parent, bool isWorldPositionStays = true ) {
			_manager.Register( new ChangeParentObjectSMTask( this, parent, isWorldPositionStays ) );
			await _manager._modifyler.WaitRunning();
		}



		protected async UniTask RunLower( SMTaskRunAllType type, Func<SMTaskModifyData> createEvent ) {
			var tasks = GetLowers( type );
			switch ( type ) {
				case SMTaskRunAllType.DontRun:
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:
					foreach ( var t in tasks ) {
						try {
							await t._modifyler.RegisterAndRun( createEvent() );
						} catch ( OperationCanceledException ) {
						}
					}
					return;
				case SMTaskRunAllType.Parallel:
					await tasks.Select( async t => {
						try {
							await t._modifyler.RegisterAndRun( createEvent() );
						} catch ( OperationCanceledException ) {
						}
					} );
					return;
			}
		}
	}
}