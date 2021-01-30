//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage.Scene {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Base;
	using Service;
	using MultiEvent;
	using Task;
	using Task.Behaviour;
	using FSM;
	using Extension;
	using Debug;
	using Debug.ToString;



	// TODO : コメント追加、整頓



	public class SMSceneManager
		: MonoBehaviourSMExtension, ISMFSMOwner<SMSceneFSM>, ISMStandardBase, ISMService
	{
		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }

		[SMHide] public bool _isInitialized	=> _body._isInitialized;
		[SMHide] public bool _isOperable	=> _body._isOperable;
		[SMHide] public bool _isFinalizing	=> _body._isFinalizing;
		[SMHide] public bool _isActive		=> _body._isActive;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;
#if DEVELOP
		[SMHide] public SMMultiSubject _onGUIEvent				=> _body._onGUIEvent;
#endif
		[SMHide] public SMTaskCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;

		[SMHide] public SMSceneFSM _fsm	=> _body._fsm;
		public SMSceneManagerBody _body	{ get; private set; }



		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();
			_body = new SMSceneManagerBody( this );

			_disposables.AddLast( () => {
				_toStringer.Dispose();
				_body.Dispose();
				Destroy( gameObject );
			} );
		}

		public override void Dispose() => _disposables.Dispose();


		void FixedUpdate() => _body?.FixedUpdate();
		void Update() => _body?.Update();
		void LateUpdate() => _body?.LateUpdate();
#if DEVELOP
		void OnGUI() => _body?.OnGUI();
#endif



		public void MoveForeverScene( GameObject rawObject )
			=> SceneManager.MoveGameObjectToScene( rawObject, _fsm._foreverScene._rawScene );



		public T GetBehaviour<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> _fsm.GetBehaviour<T>( sceneType );

		public ISMBehaviour GetBehaviour( Type type, SMSceneType? sceneType = null )
			=> _fsm.GetBehaviour( type, sceneType );

		public IEnumerable<T> GetBehaviours<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> _fsm.GetBehaviours<T>( sceneType );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, SMSceneType? sceneType = null )
			=> _fsm.GetBehaviours( type, sceneType );



		public virtual void SetToString()	{}
		public override string ToString( int indent ) => _toStringer.Run( indent );
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}