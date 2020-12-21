//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using Behaviour.Modifyler;
	using Object;



	// TODO : コメント追加、整頓



	public class RunInitialActiveSMObject : SMObjectModifyData {
		public RunInitialActiveSMObject( SMObject target ) : base( target )
			=> _type = SMTaskModifyType.Runner;

		protected override void Cancel() {}



		public override async UniTask Run() {
			switch ( _owner._type ) {
				case SMTaskType.FirstWork:	await SequentialRun( _target );	return;
				case SMTaskType.Work:		await ParallelRun( _target );	return;
			}
		}

		async UniTask SequentialRun( SMObject smObject ) {
			if ( !ChangeActiveSMObject.IsCanChange( smObject, false ) )	{ return; }
			foreach ( var b in smObject.GetBehaviours() )
													{ await ChangeActiveSMBehaviour.RegisterAndRunInitial( b ); }
			foreach ( var o in smObject.GetChildren() )	{ await SequentialRun( o ); }
		}

		async UniTask ParallelRun( SMObject smObject ) {
			if ( !ChangeActiveSMObject.IsCanChange( smObject, false ) )	{ return; }
			await Enumerable.Empty<UniTask>()
				.Concat(
					smObject.GetBehaviours().Select( b => ChangeActiveSMBehaviour.RegisterAndRunInitial( b ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o ) ) );
		}
	}
}