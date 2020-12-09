//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using MultiEvent;
	using Modifyler;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObjectGroup : IDisposableExtension {
		static uint s_idCount;
		public uint _id			{ get; private set; }

		public SMTaskType _type;
		public SMTaskLifeSpan _lifeSpan;
		public BaseScene _scene;
		public SMObjectManager _objects => _scene?._objects;
		public SMObjectModifyler _modifyler	{ get; private set; }

		public SMObjectGroup _previous;
		public SMObjectGroup _next;

		public SMObject _topObject;
		public GameObject _gameObject => _topObject._owner;

		public bool _isGameObject =>	_gameObject != null;
		public bool _isDispose =>		_disposables._isDispose;

		public UTaskCanceler _asyncCanceler =>	_topObject._asyncCanceler;
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObjectGroup( SMObject top ) {
			_id = ++s_idCount;
			_topObject = top;
			_modifyler = new SMObjectModifyler( this );

			SetAllData();

			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_topObject?.Dispose();
				_objects.Unregister( this );
			} );
		}

		~SMObjectGroup() => Dispose();

		public void Dispose() => _disposables.Dispose();



		public SMObjectGroup GetFirst() {
			SMObjectGroup current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMObjectGroup GetLast() {
			SMObjectGroup current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMObjectGroup> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}



		public void Move( SMObjectGroup remove ) {
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
#if TestSMTask
			Log.Debug( $"{nameof( SetAllData )} : start\n{this}" );
#endif
			var lastType = _type;
			var lastScene = _scene;
			var allObjects = _topObject.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestSMTask
			Log.Debug( string.Join( "\n",
				$"{nameof( allObjects )} :",
				$"{string.Join( "\n", allObjects.Select( o => o?.ToLineString() ) )}"
			) );
			Log.Debug( string.Join( "\n",
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
				!SceneManager.s_isCreated					? null :
				_lifeSpan == SMTaskLifeSpan.Forever			? SceneManager.s_instance._fsm._foreverScene :
				_topObject._isGameObject		? SceneManager.s_instance._fsm.Get( _topObject._owner.scene ) :
				SceneManager.s_instance._fsm._scene != null	? SceneManager.s_instance._fsm._scene
															: SceneManager.s_instance._fsm._startScene
			);
#if TestSMTask
			Log.Debug( string.Join( "\n",
				$"{nameof( lastType )} : {lastType}",
				$"{nameof( lastScene )} : {lastScene}",
				$"{nameof( _type )} : {_type}",
				$"{nameof( _lifeSpan )} : {_lifeSpan}",
				$"{nameof( _scene )} : {_scene}"
			) );
#endif
			allObjects.ForEach( o => o._group = this );

			if ( lastScene == null ) {
#if TestSMTask
				Log.Debug( $"Register : {this}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( _objects != null ) {
					_modifyler.Register( new RegisterSMObject() );
				}
			} else if ( _type != lastType || _scene != lastScene ) {
#if TestSMTask
				Log.Debug( $"ReRegister : {this}" );
#endif
				_modifyler.Register( new ReRegisterSMObject( lastType, lastScene ) );
			} else {
#if TestSMTask
				Log.Debug( $"DontRegister : {this}" );
#endif
			}
#if TestSMTask
			Log.Debug( $"{nameof( SetAllData )} : end\n{this}" );
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



		public override string ToString() => string.Join( "\n",
			$"{nameof( SMObjectGroup )}(",
			$"    {nameof( _id )} : {_id}",
			$"    {nameof( _type )} : {_type}",
			$"    {nameof( _lifeSpan )} : {_lifeSpan}",
			$"    {nameof( _scene )} : {_scene}",
			$"    {nameof( _modifyler )} : {_modifyler}",
			
			$"    {nameof( _previous )} : {_previous?.ToLineString()}",
			$"    {nameof( _next )} : {_next?.ToLineString()}",

			$"    {nameof( _topObject )} : {_topObject.ToLineString()}",

			$"    {nameof( _isDispose )} : {_isDispose}",
			")"
		);

		public string ToLineString( bool isViewLink = true ) {
			var result = string.Join( " ",
				_id,
				nameof( SMObjectGroup ),
				""
			);
			if ( _isDispose ) {
				result += "Dispose ";
			}
			if ( isViewLink ) {
				result += string.Join( " ",
					$"↑{_previous?._id}",
					$"↓{_next?._id}",
					$"△{_topObject._id}"
				);
			}
			return result;
		}

	}
}