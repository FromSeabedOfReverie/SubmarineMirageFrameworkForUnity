//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManager
namespace SubmarineMirage.Task.Group.Manager {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Scene;
	using Modifyler;
	using Task.Modifyler;
	using Behaviour;
	using Object;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupManager : BaseSMTaskModifylerOwner<SMGroupManagerModifyler> {
		public SMScene _scene	{ get; private set; }
		public SMGroup _group	{ get; set; }
		public bool _isEnter	{ get; private set; }
		[SMHide] public SMTaskCanceler _asyncCancelerOnDisable => _scene._asyncCancelerOnChangeOrDisable;



		public SMGroupManager( SMScene owner ) {
			_modifyler = new SMGroupManagerModifyler( this );
			_scene = owner;

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				SMGroupManagerApplyer.DisposeAll( this );
			} );
		}

		public override void Dispose() => base.Dispose();



		public IEnumerable<SMGroup> GetAllGroups()
			=> _group.GetBrothers();

		public IEnumerable<SMObject> GetAllTops()
			=> GetAllGroups().Select( g => g._topObject );


		public IEnumerable<IBaseSMTaskModifyler> GetAllModifylers() {
			yield return _modifyler;

			foreach ( var g in GetAllGroups() ) {
				yield return g._modifyler;

				foreach ( var o in g._topObject.GetAllChildren() ) {
					yield return o._modifyler;

					foreach ( var b in o.GetBehaviours() ) {
						yield return b._body._modifyler;
					}
				}
			}
		}


		public T GetBehaviour<T>() where T : ISMBehaviour
			=> GetBehaviours<T>()
				.FirstOrDefault();

		public ISMBehaviour GetBehaviour( Type type )
			=> GetBehaviours( type )
				.FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> GetBehaviours( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type ) {
			var currents = new Queue<SMObject>( GetAllTops() );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				foreach ( var b in o.GetBehaviours( type ) ) {
					yield return b;
				}
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
			}
		}



		public async UniTask Enter() {
			await Load();
//			SMLog.Debug( _modifyler );
//			await UTask.NextFrame( _asyncCancelerOnDisable );
//			await UTask.WaitWhile( _asyncCancelerOnDisable, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;

			await _modifyler.RegisterAndRun( new CreateSMGroupManager() );
			await _modifyler.RegisterAndRun( new SelfInitializeSMGroupManager() );
			await _modifyler.RegisterAndRun( new InitializeSMGroupManager() );
			await _modifyler.RegisterAndRun( new InitialEnableSMGroupManager() );
		}

		public async UniTask Exit() {
			await _modifyler.RegisterAndRun( new FinalDisableSMGroupManager() );
			await _modifyler.RegisterAndRun( new FinalizeSMGroupManager() );
			SMGroupManagerApplyer.DisposeAll( this );

			_ranState = SMTaskRunState.None;
			_activeState = SMTaskActiveState.Disable;
			_isFinalizing = false;

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _scene == _scene._fsm._foreverScene )	{ return; }
			SMTimeManager.s_instance.StartMeasure();

			var currents = new Queue<Transform>(
				_scene._rawScene.GetRootGameObjects().Select( go => go.transform )
			);
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();
				var bs = current.GetComponents<SMMonoBehaviour>();
				if ( !bs.IsEmpty() ) {
					new SMObject( current.gameObject, bs, null );
					await UTask.NextFrame( _asyncCancelerOnDisable );
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}

			SMLog.Debug( $"{nameof( Load )} {_scene.GetAboutName()} : {SMTimeManager.s_instance.StopMeasure()}秒" );
		}



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetName( nameof( _group ), nameof( GetAllTops ) );
			_toStringer.SetValue( nameof( _group ), i => "\n" +
				string.Join( ",\n", GetAllTops().Select( g =>
					g.ToLineString( i + 1 )
				)
			) );
		}
	}
}