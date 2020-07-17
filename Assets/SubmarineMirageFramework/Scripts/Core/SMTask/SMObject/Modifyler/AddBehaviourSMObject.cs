//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public class AddBehaviourSMObject : SMObjectModifyData {
		public SMMonoBehaviour _behaviour	{ get; private set; }


		public AddBehaviourSMObject( SMObject smObject, Type type ) : base( smObject ) {
			if ( _object._owner == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_object}" );
			}
			_behaviour = (SMMonoBehaviour)_object._owner.AddComponent( type );
		}

		public override void Cancel() {
			_behaviour.Dispose();
			UnityObject.Destroy( _behaviour );
		}


		public override async UniTask Run() {
			var last = _object.GetBehaviourAtLast();
			last._next = _behaviour;
			_behaviour._previous = last;

			_behaviour._object = _object;
			SetAllObjectData( _object._top );

			await RunBehaviour();
		}


		async UniTask RunBehaviour() {
			_behaviour.Constructor();

			switch ( _object._type ) {
				case SMTaskType.DontWork:
					await UTask.NextFrame( _behaviour._activeAsyncCanceler );
					await _behaviour.RunStateEvent( SMTaskRanState.Creating );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
						await UTask.NextFrame( _behaviour._activeAsyncCanceler );
						await _behaviour.RunStateEvent( SMTaskRanState.Creating );
						await _behaviour.RunStateEvent( SMTaskRanState.Loading );
						await _behaviour.RunStateEvent( SMTaskRanState.Initializing );
						await _behaviour.RunActiveEvent();
					}
					return;
			}
		}
	}
}