//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using UniRx.Async;
	using Utility;


	// TODO : コメント追加、整頓


	public class AddSMObject : SMObjectModifyData {
		public SMMonoBehaviour _behavior	{ get; private set; }


		public AddSMObject( SMObject smObject, Type type ) : base( smObject ) {
			if ( _object._owner == null ) {
				throw new NotSupportedException(
					$"{nameof( SMBehavior )}.{nameof( _object )}に、追加不可 :\n{_object}" );
			}
			_behavior = (SMMonoBehaviour)_object._owner.AddComponent( type );
		}


		public override async UniTask Run() {
			var last = _object.GetBehaviourAtLast();
			last._next = _behavior;
			_behavior._previous = last;

			_behavior._object = _object;
			_object.SetAllData();

			await RunBehaviour();
		}


		async UniTask RunBehaviour() {
			_behavior.Constructor();

			switch ( _object._type ) {
				case SMTaskType.DontWork:
					await UniTaskUtility.Yield( _behavior._activeAsyncCancel );
					await _behavior.RunStateEvent( SMTaskRanState.Creating );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
						await UniTaskUtility.Yield( _behavior._activeAsyncCancel );
						await _behavior.RunStateEvent( SMTaskRanState.Creating );
						await _behavior.RunStateEvent( SMTaskRanState.Loading );
						await _behavior.RunStateEvent( SMTaskRanState.Initializing );
						await _behavior.RunActiveEvent();
					}
					return;
			}
		}
	}
}