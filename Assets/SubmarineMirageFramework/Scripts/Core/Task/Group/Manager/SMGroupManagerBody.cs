//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task.Modifyler;
	using Task.Modifyler.Base;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupManagerBody : SMTaskModifyTarget<SMGroupManagerModifyler> {
		public static readonly SMTaskRunAllType[] SEQUENTIAL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Sequential, SMTaskRunAllType.Parallel,
		};
		public static readonly SMTaskRunAllType[] REVERSE_SEQUENTIAL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Parallel, SMTaskRunAllType.ReverseSequential,
		};
		public static readonly SMTaskRunAllType[] ALL_RUN_TYPES = new SMTaskRunAllType[] {
			SMTaskRunAllType.Sequential, SMTaskRunAllType.Parallel, SMTaskRunAllType.DontRun,
		};
		public static readonly SMTaskType[] UPDATE_TASK_TYPES = new SMTaskType[] {
			SMTaskType.FirstWork, SMTaskType.Work,
		};
		public static readonly SMTaskType[] DISPOSE_TASK_TYPES = new SMTaskType[] {
			SMTaskType.Work, SMTaskType.FirstWork, SMTaskType.DontWork,
		};

		public SMGroupManager _manager	{ get; private set; }
		public SMScene _scene	{ get; private set; }
		public SMGroupBody _groupBody	{ get; set; }

		[SMShowLine] public bool _isEnter	{ get; private set; }

		public SMAsyncCanceler _asyncCancelerOnDisable => _scene._asyncCancelerOnDispose;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.Add( nameof( _groupBody ), i =>
				_toStringer.DefaultValue( GetAllTopObjects(), i, true ) );
		}
#endregion



		public SMGroupManagerBody( SMGroupManager manager, SMScene scene ) {
			_modifyler = new SMGroupManagerModifyler( this );
			_manager = manager;
			_scene = scene;

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;

				DisposeAllGroups();
			} );
		}

		public override void Dispose() => base.Dispose();

		public void DisposeAllGroups() {
			GetAllGroups()
				.Reverse()
				.ForEach( g => g.DisposeAllObjects() );
		}



		public void Link( SMGroupBody add ) {
			if ( _groupBody == null ) {
				_groupBody = add;
			} else {
				var last = _groupBody.GetLast();
				add._previous = last;
				last._next = add;
			}
		}


		public void Unlink( SMGroupBody remove ) {
			if ( _groupBody == remove )		{ _groupBody = remove._next; }
			if ( remove._previous != null )	{ remove._previous._next = remove._next; }
			if ( remove._next != null )		{ remove._next._previous = remove._previous; }
			remove._previous = null;
			remove._next = null;
		}



		public async UniTask Enter() {
			await Load();

//			SMLog.Debug( GetAllGroups().ToShowString() );

			_isEnter = true;

			await _modifyler.RegisterAndRun( new CreateSMGroupManager() );
			await _modifyler.RegisterAndRun( new SelfInitializeSMGroupManager() );
			await _modifyler.RegisterAndRun( new InitializeSMGroupManager() );
			await _modifyler.RegisterAndRun( new InitialEnableSMGroupManager() );
		}

		public async UniTask Exit() {
			await _modifyler.RegisterAndRun( new FinalDisableSMGroupManager() );
			await _modifyler.RegisterAndRun( new FinalizeSMGroupManager() );
			DisposeAllGroups();

			_ranState = SMTaskRunState.None;
			_activeState = SMTaskActiveState.Disable;
			_isFinalizing = false;

			_isEnter = false;
		}

		async UniTask Load() {
			var currents = new Queue<Transform>(
				_scene._rawScene.GetRootGameObjects().Select( go => go.transform )
			);
			while ( !currents.IsEmpty() ) {
				var current = currents.Dequeue();
				var bs = current.GetComponents<SMBehaviour>();
				if ( !bs.IsEmpty() ) {
					new SMGroup( this, current.gameObject, bs );
					await UTask.NextFrame( _asyncCancelerOnDisable );
				} else {
					foreach ( Transform child in current ) {
						currents.Enqueue( child );
					}
				}
			}
		}



		public void FixedUpdate() {
			if ( _isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }
//			SMLog.Debug( $"{nameof( SMGroupManagerBody )}.{nameof( FixedUpdate )}" );

			var gs = GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => g.FixedUpdate( t ) )
			);

			if ( _ranState == SMTaskRunState.InitialEnable )	{ _ranState = SMTaskRunState.FixedUpdate; }
		}

		public void Update() {
			if ( _isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			var gs = GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => g.Update( t ) )
			);

			if ( _ranState == SMTaskRunState.FixedUpdate )	{ _ranState = SMTaskRunState.Update; }
		}

		public void LateUpdate() {
			if ( _isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }

			var gs = GetAllGroups().ToArray();
			UPDATE_TASK_TYPES.ForEach( t =>
				gs.ForEach( g => g.LateUpdate( t ) )
			);

			if ( _ranState == SMTaskRunState.Update )	{ _ranState = SMTaskRunState.LateUpdate; }
		}



		public void RunAllModifylers()
			=> GetAllModifylers()
				.ForEach( m => m.Run().Forget() );



		public IEnumerable<SMGroupBody> GetAllGroups()
			=> _groupBody?.GetBrothers() ?? Enumerable.Empty<SMGroupBody>();

		public IEnumerable<SMObjectBody> GetAllTopObjects()
			=> GetAllGroups().Select( g => g._objectBody );


		public IEnumerable<IBaseSMTaskModifyler> GetAllModifylers() {
			yield return _modifyler;

			foreach ( var g in GetAllGroups() ) {
				yield return g._modifyler;

				foreach ( var o in g._objectBody.GetAllChildren() ) {
					yield return o._modifyler;

					foreach ( var b in o._behaviourBody.GetBrothers() ) {
						yield return b._modifyler;
					}
				}
			}
		}
	}
}