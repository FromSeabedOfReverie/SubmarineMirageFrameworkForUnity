//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using MultiEvent;


	// TODO : コメント追加、整頓


	public class RunStateSMObject : SMObjectModifyData {
		SMTaskRanState _state;


		public RunStateSMObject( SMObject smObject, SMTaskRanState state ) : base( smObject ) {
			_state = state;

			switch ( state ) {
				case SMTaskRanState.FixedUpdate:
				case SMTaskRanState.Update:
				case SMTaskRanState.LateUpdate:
					// 駄目で元々、即時実行する
					RunStateEvent( _object, state ).Forget();
					break;
				case SMTaskRanState.Finalizing:
					// TODO : キューに貯まっている他のタスクを無視して、即実行したいけど、逐次実行でも問題ない？
					break;
			}
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			switch ( _state ) {
				case SMTaskRanState.FixedUpdate:
				case SMTaskRanState.Update:
				case SMTaskRanState.LateUpdate:
					return;
				default:
					await RunStateEvent( _object, _state );
					break;
			}
		}


		async UniTask RunStateEvent( SMObject smObject, SMTaskRanState state ) {
			using ( var events = new MultiAsyncEvent() ) {
				switch ( smObject._type ) {
					case SMTaskType.FirstWork:
						foreach ( var b in smObject.GetBehaviours() ) {
							events.AddLast( async _ => {
								try										{ await b.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} );
						}
						events.AddLast( async _ => {
							foreach ( var o in smObject.GetChildren() ) {
								await RunStateEvent( o, state );
							}
						} );
						break;

					case SMTaskType.Work:
						events.AddLast( async _ => await smObject.GetBehaviours().Select( async b => {
								try										{ await b.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								smObject.GetChildren().Select( o => RunStateEvent( o, state ) )
							)
						);
						break;

					case SMTaskType.DontWork:
						if ( state != SMTaskRanState.Creating )	{ break; }
						events.AddLast( async _ => await smObject.GetBehaviours().Select( async b => {
								try										{ await b.RunStateEvent( state ); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								smObject.GetChildren().Select( o => RunStateEvent( o, state ) )
							)
						);
						break;
				}
				if ( state == SMTaskRanState.Finalizing )	{ events.Reverse(); }
				await events.Run( smObject._asyncCanceler );
			}
		}
	}
}