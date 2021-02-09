//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Modifyler.Base;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitializeSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.SelfInitialize )	{ return; }

			await _owner._initializeEvent.Run( _owner._asyncCancelerOnDispose );
			_owner._ranState = SMTaskRunState.Initialize;
		}
	}
}