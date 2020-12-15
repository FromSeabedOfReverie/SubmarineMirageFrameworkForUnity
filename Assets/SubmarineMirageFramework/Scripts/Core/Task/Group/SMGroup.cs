//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Task {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Modifyler;
	using Scene;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMGroup : SMStandardBase {
		public SMTaskType _type			{ get; set; }
		public SMTaskLifeSpan _lifeSpan	{ get; set; }
		public SMScene _scene		{ get; set; }
		[SMHide] public SMGroupManager _groups => _scene?._groups;
		public SMObjectModifyler _modifyler	{ get; private set; }

		[SMShowLine] public SMGroup _previous	{ get; set; }
		[SMShowLine] public SMGroup _next		{ get; set; }

		[SMShowLine] public SMObject _topObject	{ get; set; }
		[SMHide] public GameObject _gameObject => _topObject._owner;
		[SMHide] public bool _isGameObject => _gameObject != null;

		[SMHide] public SMTaskCanceler _asyncCanceler =>	_topObject._asyncCanceler;


		public SMGroup( SMObject top ) {
			_topObject = top;
			_modifyler = new SMObjectModifyler( this );

			SetAllData();

			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_topObject?.Dispose();
				_groups.Unregister( this );
			} );
		}



		public SMGroup GetFirst() {
			SMGroup current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMGroup GetLast() {
			SMGroup current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMGroup> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}



		public void Move( SMGroup remove ) {
			_modifyler.Move( remove._modifyler );
			remove._topObject.GetAllChildren().ForEach( o => o._group = this );
			remove._topObject = null;
			remove.Dispose();
		}



		public bool IsTop( SMObject smObject ) => smObject == _topObject;

		public void SetTop( SMObject smObject ) {
			_topObject = smObject.GetTop();
			SetAllData();
		}

		public void SetAllData() {
#if TestTask
			SMLog.Debug( $"{nameof( SetAllData )} : start\n{this}" );
#endif
			var lastType = _type;
			var lastScene = _scene;
			var allObjects = _topObject.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestTask
			SMLog.Debug( string.Join( "\n",
				$"{nameof( allObjects )} :",
				$"{string.Join( "\n", allObjects.Select( o => o?.ToLineString() ) )}"
			) );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( allBehaviours )} :",
				$"{string.Join( "\n", allBehaviours.Select( b => b?.ToLineString() ) )}"
			) );
#endif
			_type = (
				allBehaviours.Any( b => b._type == SMTaskType.FirstWork )	? SMTaskType.FirstWork :
				allBehaviours.Any( b => b._type == SMTaskType.Work )		? SMTaskType.Work
																			: SMTaskType.DontWork
			);
			_lifeSpan = allBehaviours.Any( b => b._lifeSpan == SMTaskLifeSpan.Forever ) ?
				SMTaskLifeSpan.Forever : SMTaskLifeSpan.InScene;
			_scene = (
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				!SMSceneManager.s_isCreated					? null :
				_lifeSpan == SMTaskLifeSpan.Forever			? SMSceneManager.s_instance._fsm._foreverScene :
				_topObject._isGameObject		? SMSceneManager.s_instance._fsm.Get( _topObject._owner.scene ) :
				SMSceneManager.s_instance._fsm._scene != null	? SMSceneManager.s_instance._fsm._scene
															: SMSceneManager.s_instance._fsm._startScene
			);
#if TestTask
			SMLog.Debug( string.Join( "\n",
				$"{nameof( lastType )} : {lastType}",
				$"{nameof( lastScene )} : {lastScene}",
				$"{nameof( _type )} : {_type}",
				$"{nameof( _lifeSpan )} : {_lifeSpan}",
				$"{nameof( _scene )} : {_scene}"
			) );
#endif
			allObjects.ForEach( o => o._group = this );

			if ( lastScene == null ) {
#if TestTask
				SMLog.Debug( $"Register : {this}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( _groups != null ) {
					_modifyler.Register( new RegisterSMGroup() );
				}
			} else if ( _type != lastType || _scene != lastScene ) {
#if TestTask
				SMLog.Debug( $"ReRegister : {this}" );
#endif
				_modifyler.Register( new ReRegisterSMGroup( lastType, lastScene ) );
			} else {
#if TestTask
				SMLog.Debug( $"DontRegister : {this}" );
#endif
			}
#if TestTask
			SMLog.Debug( $"{nameof( SetAllData )} : end\n{this}" );
#endif
		}



		public async UniTask RunStateEvent( SMTaskRunState state ) {
			RunStateSMObject.RegisterAndRun( this, state );
			await _modifyler.WaitRunning();
		}

		public async UniTask ChangeActive( bool isActive ) {
			_modifyler.Register( new ChangeActiveSMObject( _topObject, isActive, true ) );
			await _modifyler.WaitRunning();
		}

		public async UniTask RunInitialActive() {
			_modifyler.Register( new RunInitialActiveSMObject( _topObject ) );
			await _modifyler.WaitRunning();
		}



		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _previous ), i => _previous?.ToLineString() );
			_toStringer.SetValue( nameof( _next ), i => _next?.ToLineString() );
			_toStringer.SetValue( nameof( _topObject ), i => _topObject.ToLineString() );

			_toStringer.SetLineValue( nameof( _isDispose ), () => _isDispose ? nameof( Dispose ) : "" );
			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _topObject ), () => $"△{_topObject._id}" );
		}
	}
}