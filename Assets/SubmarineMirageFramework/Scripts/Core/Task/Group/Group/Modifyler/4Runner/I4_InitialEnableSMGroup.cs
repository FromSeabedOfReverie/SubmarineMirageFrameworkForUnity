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



	// TODO : コメント追加、整頓



	public class InitialEnableSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] SMTaskRunAllType _runType	{ get; set; }


		public InitialEnableSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			await RunLower( _runType, () => new InitialEnableSMObject( _runType ) );

			if ( _runType == SMTaskRunAllType.Parallel ) {
				_target._activeState = SMTaskActiveState.Enable;
				_target._ranState = SMTaskRunState.InitialEnable;
			}
		}
	}
}