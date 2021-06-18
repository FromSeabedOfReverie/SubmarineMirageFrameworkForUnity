//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;


	public class SelfInitializeSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );


		public SelfInitializeSMTask( SMTask task ) : base( task ) {}


		public override async UniTask Run() {
			if ( _task._ranState != SMTaskRunState.Create )	{ return; }

			await _task._selfInitializeEvent.Run( _task._asyncCancelerOnDispose );
			_task._ranState = SMTaskRunState.SelfInitialize;
		}
	}
}