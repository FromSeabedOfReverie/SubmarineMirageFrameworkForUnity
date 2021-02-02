//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Service;
	using Task.Modifyler;
	using Task.Modifyler.Base;
	using Scene;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupBody : BaseSMTaskModifylerOwner<SMGroupModifyler> {
		public SMGroup _group	{ get; private set; }
		public SMGroupManagerBody _managerBody	{ get; set; }
		[SMShowLine] public SMObjectBody _objectBody	{ get; set; }

		[SMHide] SMSceneManager _sceneManager	{ get; set; }
		[SMHide] public GameObject _gameObject => _objectBody._gameObject;

		[SMShowLine] public SMGroupBody _previous	{ get; set; }
		[SMShowLine] public SMGroupBody _next		{ get; set; }

		public SMTaskLifeSpan _lifeSpan	{ get; set; }
		[SMHide] public bool _isGameObject => _gameObject != null;

		[SMHide] public SMAsyncCanceler _asyncCanceler => _objectBody._asyncCanceler;



		public SMGroupBody( SMGroup group, GameObject gameObject, IEnumerable<ISMBehaviour> behaviours ) {
			SMGroupBodySub( group );

			var smObject = new SMObject( this, null, gameObject, behaviours );
			_objectBody = smObject._body;
			SetAllData();
		}

		public SMGroupBody( SMGroup group, SMObjectBody objectBody ) {
			SMGroupBodySub( group );

			_objectBody = objectBody;
			SetAllData();
		}

		void SMGroupBodySub( SMGroup group ) {
			_modifyler = new SMGroupModifyler( this );
			_group = group;
			_sceneManager = SMServiceLocator.Resolve<SMSceneManager>();

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;
			} );
		}

		public override void Dispose() => base.Dispose();

		public void DisposeAllObjects() {
			_objectBody.DisposeAllChildren();
			_group.Dispose();
		}



		public void SetAllData() {
			var lastManagerBody = _managerBody;
			var allObjects = _objectBody.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o._behaviourBody.GetBrothers() );

			_lifeSpan = allBehaviours.Any( b => b._lifeSpan == SMTaskLifeSpan.Forever ) ?
				SMTaskLifeSpan.Forever : SMTaskLifeSpan.InScene;

			if ( _lifeSpan == SMTaskLifeSpan.Forever ) {
				_managerBody = _sceneManager._fsm._foreverScene._groupManagerBody;
			} else if ( _isGameObject ) {
				_managerBody = _sceneManager._fsm.GetScene( _gameObject.scene )._groupManagerBody;
			} else if ( _sceneManager._fsm._mainFSM._scene != null ) {
				_managerBody = _sceneManager._fsm._mainFSM._scene._groupManagerBody;
			} else {
				var mainFSM = _sceneManager._fsm._mainFSM;
				_managerBody = mainFSM.GetState( mainFSM._startStateType )._groupManagerBody;
			}
			// 親子変更等で、無理矢理、削除不能化した場合を考慮
			if ( _managerBody._scene == _sceneManager._fsm._foreverScene ) {
				_lifeSpan = SMTaskLifeSpan.Forever;
			}

// TODO : 廃止
			allObjects.ForEach( o => o._groupBody = this );

			if ( lastManagerBody == null ) {
				_managerBody._modifyler.Register( new RegisterGroupSMGroupManager( this ) );
			} else if ( _managerBody != lastManagerBody ) {
				lastManagerBody._modifyler.Register( new SendReregisterGroupSMGroupManager( this ) );
			}
		}



		public bool IsTop( SMObjectBody objectBody ) => objectBody == _objectBody;



		public void RegisterRunEventToOwner() {
			if (	_managerBody._ranState >= SMTaskRunState.FinalDisable &&
					_ranState < SMTaskRunState.FinalDisable
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					_modifyler.Register( new FinalDisableSMGroup( t ) );
				}
			}

			if (	_managerBody._ranState >= SMTaskRunState.Finalize &&
					_ranState < SMTaskRunState.Finalize
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					_modifyler.Register( new FinalizeSMGroup( t ) );
				}
			}

			if ( _managerBody._isFinalizing )	{ return; }


			if (	_managerBody._ranState >= SMTaskRunState.Create &&
					_ranState < SMTaskRunState.Create
			) {
				foreach ( var t in SMGroupManagerBody.ALL_RUN_TYPES ) {
					_modifyler.Register( new CreateSMGroup( t ) );
				}
			}

			if (	_managerBody._ranState >= SMTaskRunState.SelfInitialize &&
					_ranState < SMTaskRunState.SelfInitialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					_modifyler.Register( new SelfInitializeSMGroup( t ) );
				}
			}

			if (	_managerBody._ranState >= SMTaskRunState.Initialize &&
					_ranState < SMTaskRunState.Initialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					_modifyler.Register( new InitializeSMGroup( t ) );
				}
			}

			if (	_managerBody._ranState >= SMTaskRunState.InitialEnable &&
					_ranState < SMTaskRunState.InitialEnable
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					_modifyler.Register( new InitialEnableSMGroup( t ) );
				}
			}
		}



		public void FixedUpdate( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }

			_objectBody.GetAllChildren()
				.ForEach( o => o.FixedUpdate( type ) );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.InitialEnable ) {
				_ranState = SMTaskRunState.FixedUpdate;
			}
		}

		public void Update( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_objectBody.GetAllChildren()
				.ForEach( o => o.Update( type ) );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.FixedUpdate ) {
				_ranState = SMTaskRunState.Update;
			}
		}

		public void LateUpdate( SMTaskType type ) {
			if ( !_isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.Update )	{ return; }
			
			_objectBody.GetAllChildren()
				.ForEach( o => o.LateUpdate( type ) );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.Update ) {
				_ranState = SMTaskRunState.LateUpdate;
			}
		}



		public void DestroyObject( SMObjectBody objectBody )
			=> _modifyler.Register( new DestroyObjectSMGroup( objectBody ) );

		public void ChangeActiveObject( SMObjectBody objectBody, bool isActive ) {
			if ( isActive )	{ _modifyler.Register( new EnableObjectSMGroup( objectBody ) ); }
			else			{ _modifyler.Register( new DisableObjectSMGroup( objectBody ) ); }
		}

		public void ChangeParentObject( SMObjectBody objectBody, Transform parent, bool isWorldPositionStays )
			=> _modifyler.Register(
				new SendChangeParentObjectSMGroup( objectBody, parent, isWorldPositionStays ) );


		public T AddBehaviour<T>( SMObjectBody objectBody ) where T : SMMonoBehaviour
			=> (T)AddBehaviour( objectBody, typeof( T ) );

		public SMMonoBehaviour AddBehaviour( SMObjectBody objectBody, Type type ) {
			var data = new AddBehaviourSMGroup( objectBody, type );
			_modifyler.Register( data );
			return data._behaviour;
		}



		public SMGroupBody GetFirst() {
			SMGroupBody current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}

		public SMGroupBody GetLast() {
			SMGroupBody current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMGroupBody> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}



		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.SetValue( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );
			_toStringer.SetValue( nameof( _objectBody ), i => _toStringer.DefaultValue( _objectBody, i, true ) );

			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _objectBody ), () => $"△{_objectBody._id}" );
		}
	}
}