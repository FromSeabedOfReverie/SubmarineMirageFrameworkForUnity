//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class InitialEnableSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public InitialEnableSMGroupManager() : base( null ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new InitialEnableSMGroup( t ) );
			}
			_target._activeState = SMTaskActiveState.Enable;
			_target._ranState = SMTaskRunState.InitialEnable;
		}
	}
}