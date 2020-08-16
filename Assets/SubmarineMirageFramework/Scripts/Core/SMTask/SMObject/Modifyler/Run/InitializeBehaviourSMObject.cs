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


	// TODO : コメント追加、整頓


	public class InitializeBehaviourSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Runner;
		SMMonoBehaviour _behaviour;


		public InitializeBehaviourSMObject( SMObject smObject, SMMonoBehaviour behaviour ) : base( smObject ) {
			if ( _object._owner == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、実行不可 :\n{_object}" );
			}
			_behaviour = behaviour;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			switch ( _object._type ) {
				case SMTaskType.DontWork:
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
					await UTask.NextFrame( _behaviour._activeAsyncCanceler );
					await _behaviour.RunStateEvent( SMTaskRanState.Creating );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
// TODO : 作成直後に実行するとエラーになる為、待機しているが、できれば不要にしたい
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