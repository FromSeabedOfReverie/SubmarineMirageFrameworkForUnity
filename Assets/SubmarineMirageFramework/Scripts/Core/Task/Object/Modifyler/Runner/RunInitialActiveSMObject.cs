//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;


	// TODO : コメント追加、整頓


	public class RunInitialActiveSMObject : SMObjectModifyData {
		public RunInitialActiveSMObject( SMObject smObject ) : base( smObject ) {
			_type = ModifyType.Runner;
		}

		public override void Cancel() {}



		public override async UniTask Run() {
			switch ( _group._type ) {
				case SMTaskType.FirstWork:	await SequentialRun( _object );	return;
				case SMTaskType.Work:		await ParallelRun( _object );	return;
			}
		}

		async UniTask SequentialRun( SMObject smObject ) {
			if ( !IsCanChangeActive( smObject, false ) )	{ return; }
			foreach ( var b in smObject.GetBehaviours() )
													{ await ChangeActiveSMBehaviour.RegisterAndRunInitial( b ); }
			foreach ( var o in smObject.GetChildren() )	{ await SequentialRun( o ); }
		}

		async UniTask ParallelRun( SMObject smObject ) {
			if ( !IsCanChangeActive( smObject, false ) )	{ return; }
			await Enumerable.Empty<UniTask>()
				.Concat(
					smObject.GetBehaviours().Select( b => ChangeActiveSMBehaviour.RegisterAndRunInitial( b ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o ) ) );
		}
	}
}