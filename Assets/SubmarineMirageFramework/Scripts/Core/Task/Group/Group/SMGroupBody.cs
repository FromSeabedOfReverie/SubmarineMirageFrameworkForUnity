//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using Task.Modifyler;
	using Utility;
	using Debug;



	public class SMGroupBody : SMTask {
		public SMGroup _group	{ get; private set; }
		public SMGroupManagerBody _managerBody	{ get; set; }
		[SMShowLine] public SMObjectBody _objectBody	{ get; set; }

		public GameObject _gameObject => _objectBody._gameObject;

		public SMGroupBody _previous	{ get; set; }
		public SMGroupBody _next		{ get; set; }

		[SMShow] protected override Type _baseModifyDataType => typeof( SMGroupModifyData );

		public SMAsyncCanceler _asyncCanceler => _objectBody._asyncCanceler;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.Add( nameof( _previous ), i => _toStringer.DefaultValue( _previous, i, true ) );
			_toStringer.Add( nameof( _next ), i => _toStringer.DefaultValue( _next, i, true ) );
			_toStringer.SetValue( nameof( _objectBody ), i => _toStringer.DefaultValue( _objectBody, i, true ) );

			_toStringer.AddLine( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.AddLine( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _objectBody ), () => $"△{_objectBody._id}" );
		}
#endregion



		public SMGroupBody( SMGroup group, SMGroupManagerBody managerBody, GameObject gameObject,
							IEnumerable<SMBehaviour> behaviours
		) {
			SMGroupBodySub( group );

			var smObject = new SMObject( this, null, gameObject, behaviours );
			_objectBody = smObject._body;

			_managerBody = managerBody;
			_managerBody._scene.MoveGroup( this );
			_managerBody._modifyler.Register( new RegisterGroupSMGroupManager( this ) );
		}

		public SMGroupBody( SMGroup group, SMGroupManagerBody managerBody, SMObjectBody objectBody ) {
			SMGroupBodySub( group );

			_objectBody = objectBody;
			var lastGroup = _objectBody._groupBody;

			_ranState = lastGroup._ranState;
			_activeState = lastGroup._activeState;
			_isFinalizing = lastGroup._isFinalizing;

			// 親を解除、新グループを再設定
			_objectBody.Unlink();
			_objectBody.SetGroupOfAllChildren( this );

			// 親アクティブに一致させる変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _objectBody ) );
			lastGroup.ReregisterModifyler( this );  // 元グループの変更を、新グループに再登録

			_managerBody = managerBody;
			_managerBody._modifyler.Register( new RegisterGroupSMGroupManager( this ) );
		}

		void SMGroupBodySub( SMGroup group ) {
			_group = group;

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
//			SMLog.Debug( $"{nameof( SMGroupBody )} : {_isFinalizing}, {_isActive}, {_ranState}" );
			if ( _isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.InitialEnable )	{ return; }
			SMLog.Debug( $"{nameof( SMGroupBody )}.{nameof( FixedUpdate )}" );

			_objectBody.GetAllChildren()
				.ForEach( o => o.FixedUpdate( type ) );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.InitialEnable ) {
				_ranState = SMTaskRunState.FixedUpdate;
			}
		}

		public void Update( SMTaskType type ) {
			if ( _isFinalizing )	{ return; }
			if ( !_isActive )		{ return; }
			if ( _ranState < SMTaskRunState.FixedUpdate )	{ return; }

			_objectBody.GetAllChildren()
				.ForEach( o => o.Update( type ) );

			if ( type == SMTaskType.Work && _ranState == SMTaskRunState.FixedUpdate ) {
				_ranState = SMTaskRunState.Update;
			}
		}

		public void LateUpdate( SMTaskType type ) {
			if ( _isFinalizing )	{ return; }
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

		public void ChangeParentObject( SMObjectBody objectBody, Transform parent, bool isWorldPositionStays ) {
			if ( parent != null ) {
				_modifyler.Register(
					new SendChangeParentObjectSMGroup( objectBody, parent, isWorldPositionStays ) );
			} else {
				_modifyler.Register( new ReleaseParentObjectSMGroup( objectBody, isWorldPositionStays ) );
			}
		}


		public T AddBehaviour<T>( SMObjectBody objectBody ) where T : SMBehaviour
			=> (T)AddBehaviour( objectBody, typeof( T ) );

		public SMBehaviour AddBehaviour( SMObjectBody objectBody, Type type ) {
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



		public void ReregisterModifyler( SMGroupBody newTarget ) => _modifyler.Reregister(
			newTarget._modifyler,
			d => {
				var gd = ( SMGroupModifyData )d;
				return gd._object._groupBody == newTarget;
			}
		);

		public void UnregisterModifyler( SMObjectBody remove ) => _modifyler.Unregister(
			d => {
				var gd = ( SMGroupModifyData )d;
				return gd._object == remove;
			}
		);
	}
}