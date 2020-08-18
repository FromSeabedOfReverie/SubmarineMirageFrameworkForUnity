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
		public override ModifyType _type => ModifyType.Runner;
		SMTaskRanState _state;


		public RunStateSMObject( SMObject smObject, SMTaskRanState state ) : base( smObject ) {
			_state = state;

			switch ( state ) {
				case SMTaskRanState.FixedUpdate:
				case SMTaskRanState.Update:
				case SMTaskRanState.LateUpdate:
					throw new ArgumentOutOfRangeException(
						$"{state}",
						$"負荷軽減の為、静的関数 {nameof( RunOrRegister )} 以外で、実行不可"
					);
			}
		}

		public override void Cancel() {}


		public override UniTask Run() => RunStateEvent( _object, _state );


		public static void RunOrRegister( SMObject smObject, SMTaskRanState state ) {
			switch ( state ) {
				case SMTaskRanState.FixedUpdate:
				case SMTaskRanState.Update:
				case SMTaskRanState.LateUpdate:
					// 駄目で元々、即時実行する
					RunStateEvent( smObject, state ).Forget();
					break;

// TODO : キューに貯まっている他のタスクを無視して、即実行したいけど、逐次実行でも問題ない？
//				case SMTaskRanState.Finalizing:
//					break;

				default:
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, state ) );
					break;
			}
		}


		static async UniTask RunStateEvent( SMObject smObject, SMTaskRanState state ) {
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