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
		public SMScene _owner	{ get; private set; }
		public SMGroup _topGroup	{ get; set; }
		public bool _isEnter	{ get; private set; }

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisable => _owner._activeAsyncCanceler;



		public SMGroupManager( SMScene owner ) {
			_modifyler = new SMGroupManagerModifyler( this );
			_owner = owner;

			_disposables.AddLast( () => {
				DisposeGroups();
			} );
		}

		void DisposeGroups() {
			var bs = GetAllTops()
				.SelectMany( o => o.GetAllChildren() )
				.SelectMany( o => o.GetBehaviours() )
				.Reverse()
				.ToArray();
			SMGroupManagerApplyer.DISPOSE_TASK_TYPES
				.SelectMany( t =>
					bs.Where( b => b._type == t )
				)
				.ToArray()
				.ForEach( b => b.Dispose() );
		}


		public IEnumerable<SMGroup> GetAllGroups()
			=> _topGroup.GetBrothers();

		public IEnumerable<SMObject> GetAllTops()
			=> GetAllGroups().Select( g => g._topObject );



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

			foreach ( var type in SMGroupManagerApplyer.ALL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new CreateSMGroupManager( type ) );
			}
			foreach ( var type in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new SelfInitializeSMGroupManager( type ) );
			}
			foreach ( var type in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new InitializeSMGroupManager( type ) );
			}
			foreach ( var type in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new InitialEnableSMGroupManager( type ) );
			}
		}

		public async UniTask Exit() {
			foreach ( var type in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new FinalDisableSMGroupManager( type ) );
			}
			foreach ( var type in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await _modifyler.RegisterAndRun( new FinalizeSMGroupManager( type ) );
			}
			DisposeGroups();

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			SMTimeManager.s_instance.StartMeasure();

			var currents = new Queue<Transform>(
				_owner._scene.GetRootGameObjects().Select( go => go.transform )
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

			SMLog.Debug( $"{nameof( Load )} {_owner.GetAboutName()} : {SMTimeManager.s_instance.StopMeasure()}秒" );
		}



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetName( nameof( _topGroup ), nameof( GetAllTops ) );
			_toStringer.SetValue( nameof( _topGroup ), i => "\n" +
				string.Join( ",\n", GetAllTops().Select( g =>
					g.ToLineString( i + 1 )
				)
			) );
		}
	}
}