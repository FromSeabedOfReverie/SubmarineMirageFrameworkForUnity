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
	using SubmarineMirage.Base;
	using Service;
	using Event;
	using Task;
	using FSM;
	using Scene.Base;
	using Extension;
	using Utility;
	using Debug;
	using Debug.ToString;



	// TODO : コメント追加、整頓



	public class SMSceneManager
		: MonoBehaviourSMExtension, ISMFSMOwner<SMSceneFSM>, ISMStandardBase, ISMService
	{
		[SMShow] public SMSceneManagerBody _body	{ get; private set; }

		public SMSceneFSM _fsm			=> _body._fsm;
		public SMSceneFSM _foreverFSM	=> _body._foreverFSM;
		public SMSceneFSM _mainFSM		=> _body._mainFSM;
		public SMSceneFSM _uiFSM		=> _body._uiFSM;
		public SMSceneFSM _debugFSM	=> _body._debugFSM;
		public SMScene _foreverScene	=> _body._foreverScene;

		[SMShowLine] public bool _isInitialEnteredFSMs	{ get; set; }

		public bool _isInitialized	=> _body._isInitialized;
		public bool _isOperable	=> _body._isOperable;
		public bool _isFinalizing	=> _body._isFinalizing;
		public bool _isActive		=> _body._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent			=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent		=> _body._finalizeEvent;
#if DEVELOP
		public SMSubject _onGUIEvent				=> _body._onGUIEvent;
#endif
		public SMAsyncCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;

		public SMDisposable _disposables	{ get; private set; } = new SMDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		public SMToStringer _toStringer	{ get; private set; }



		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();
			_body = new SMSceneManagerBody( this );

			var test = new Test.TestSMSceneManager( this );
			test.SetEvent();

			_disposables.AddLast( () => {
				_toStringer.Dispose();
				_body.Dispose();
				Destroy( gameObject );

				test.Dispose();
			} );
		}

		public override void Dispose() => _disposables.Dispose();



		void FixedUpdate() => _body?.FixedUpdate();
		void Update() => _body?.Update();
		void LateUpdate() => _body?.LateUpdate();
#if DEVELOP
		void OnGUI() => _body?.OnGUI();
#endif



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
				return _fsm.GetFSM( sceneType )
					?.GetBehaviours( type )
					?? Enumerable.Empty<SMBehaviour>();
			}
			return _fsm.GetFSMs()
				.SelectMany( fsm => fsm.GetBehaviours( type ) );
		}



		public virtual void SetToString() {}

		public override string ToString( int indent, bool isUseHeadIndent = true )
			=> _toStringer.Run( indent, isUseHeadIndent );

		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}