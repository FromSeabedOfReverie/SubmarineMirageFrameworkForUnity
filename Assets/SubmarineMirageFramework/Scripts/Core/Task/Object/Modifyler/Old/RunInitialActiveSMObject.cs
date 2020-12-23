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
			if ( _owner._type == SMTaskType.DontWork )	{ return; }
			if ( !_target._isGameObject ) {
				await ChangeActiveSMBehaviour.RegisterAndRunInitial( _target._behaviour );
				return;
			}
			if ( !ChangeActiveSMObject.IsCanChange( _target, false ) )	{ return; }

			switch ( _owner._type ) {
				case SMTaskType.FirstWork:	await SequentialRun( _target );	return;
				case SMTaskType.Work:		await ParallelRun( _target );	return;
			}
		}

		async UniTask SequentialRun( SMObject smObject ) {
			if ( !smObject._owner.activeSelf )	{ return; }
			foreach ( var b in smObject.GetBehaviours() )
													{ await ChangeActiveSMBehaviour.RegisterAndRunInitial( b ); }
			foreach ( var o in smObject.GetChildren() )	{ await SequentialRun( o ); }
		}

		async UniTask ParallelRun( SMObject smObject ) {
			if ( !smObject._owner.activeSelf )	{ return; }
			await Enumerable.Empty<UniTask>()
				.Concat(
					smObject.GetBehaviours().Select( b => ChangeActiveSMBehaviour.RegisterAndRunInitial( b ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o ) ) );
		}
	}
}