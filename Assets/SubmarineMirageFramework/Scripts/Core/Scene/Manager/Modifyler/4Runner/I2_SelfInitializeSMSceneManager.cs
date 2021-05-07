//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Task;
	using Debug;


	public class SelfInitializeSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Create )	{ return; }

			await _target._selfInitializeEvent.Run( _target._asyncCancelerOnDispose );
			_target._ranState = SMTaskRunState.SelfInitialize;
		}
	}
}