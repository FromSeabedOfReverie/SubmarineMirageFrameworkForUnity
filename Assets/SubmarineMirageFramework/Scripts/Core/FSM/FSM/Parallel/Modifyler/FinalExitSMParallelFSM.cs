//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using FSM.Base;
	using FSM.Modifyler.Base;



	// TODO : コメント追加、整頓



	public class FinalExitSMParallelFSM<TOwner, TInternalFSM, TEnum>
		: SMParallelFSMModifyData<TOwner, TInternalFSM, TEnum>
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : SMFSM
		where TEnum : Enum
	{
		public override SMFSMModifyType _type => SMFSMModifyType.FirstRunner;


		public override async UniTask Run() {
			SMFSMApplyer.StopAsyncOnDisableAndExit( _owner );

			await _owner._fsms.Select( pair => pair.Value.FinalExit() );

			_modifyler.Dispose();
		}
	}
}