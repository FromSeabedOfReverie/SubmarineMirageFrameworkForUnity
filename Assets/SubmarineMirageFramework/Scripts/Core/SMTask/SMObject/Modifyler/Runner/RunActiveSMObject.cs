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


	public class RunActiveSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Runner;


		public RunActiveSMObject( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() {}


		public override UniTask Run() => RunActiveEvent( _object );


// TODO : 親がdisableでも、enableしそう
		async UniTask RunActiveEvent( SMObject smObject ) {
			using ( var events = new MultiAsyncEvent() ) {
				switch ( smObject._type ) {
					case SMTaskType.FirstWork:
						foreach ( var b in smObject.GetBehaviours() ) {
							events.AddLast( async _ => {
								try										{ await b.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} );
						}
						events.AddLast( async _ => {
							foreach ( var o in smObject.GetChildren() ) {
								await RunActiveEvent( o );
							}
						} );
						break;

					case SMTaskType.Work:
						events.AddLast( async _ => await smObject.GetBehaviours().Select( async b => {
								try										{ await b.RunActiveEvent(); }
								catch ( OperationCanceledException )	{}
							} )
							.Concat(
								smObject.GetChildren().Select( o => RunActiveEvent( o ) )
							)
						);
						break;
				}
				await events.Run( smObject._asyncCanceler );
			}
		}
	}
}