//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Base;
	using Service;
	using Event;
	using Task;
	using FSM;
	using Utility;
	using Debug;



	public class SMSceneManager : SMStandardBase, ISMService {
		[SMShow] public SMSceneManagerBody _body	{ get; private set; }

		public SMFSM _fsm				=> _body._fsm;
		public SMFSM _foreverFSM		=> _body._foreverFSM;
		public SMFSM _mainFSM			=> _body._mainFSM;
		public SMFSM _uiFSM				=> _body._uiFSM;
		public SMFSM _debugFSM			=> _body._debugFSM;
		public SMScene _foreverScene	=> _body._foreverScene;

		public bool _isInitialized	=> _body._isInitialized;
		public bool _isOperable		=> _body._isOperable;
		public bool _isFinalizing	=> _body._isFinalizing;
		public bool _isActive		=> _body._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent			=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent				=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent			=> _body._finalizeEvent;
#if DEVELOP
		public SMSubject _onGUIEvent				=> _body._onGUIEvent;
#endif
		public SMAsyncCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		public SMAsyncCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;



		public SMSceneManager() {
			_body = new SMSceneManagerBody( this );

			_disposables.AddLast( () => {
				_body.Dispose();
				SubmarineMirageFramework.Shutdown();
			} );

///*
			var test = new Test.TestSMSceneManager( this );
//			test.SetEvent();
			_disposables.AddLast( () => {
				test.Dispose();
			} );
//*/
		}

		public override void Dispose() => base.Dispose();



		public T GetBehaviour<T>( Type sceneType = null ) where T : SMBehaviour
			=> GetBehaviours<T>( sceneType )
				.FirstOrDefault();

		public T GetBehaviour<T, TScene>() where T : SMBehaviour where TScene : SMScene
			=> GetBehaviours<T, TScene>()
				.FirstOrDefault();

		public SMBehaviour GetBehaviour( Type type, Type sceneType = null )
			=> GetBehaviours( type, sceneType )
				.FirstOrDefault();


		public IEnumerable<T> GetBehaviours<T>( Type sceneType = null ) where T : SMBehaviour
			=> GetBehaviours( typeof( T ), sceneType )
				.Select( b => (T)b );

		public IEnumerable<T> GetBehaviours<T, TScene>() where T : SMBehaviour where TScene : SMScene
			=> GetBehaviours( typeof( T ), typeof( TScene ) )
				.Select( b => (T)b );

		public IEnumerable<SMBehaviour> GetBehaviours( Type type, Type sceneType = null ) {
			if ( sceneType != null ) {
				var scene = ( SMScene )_fsm.GetFSM( sceneType )
					?._state;
				return scene
					?.GetBehaviours( type )
					?? Enumerable.Empty<SMBehaviour>();
			}
			return _fsm.GetFSMs()
				.Select( fsm => ( SMScene )fsm._state )
				.SelectMany( s => s?.GetBehaviours( type ) )
				.Where( s => s != null );
		}
	}
}