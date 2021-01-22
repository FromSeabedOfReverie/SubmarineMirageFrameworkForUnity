//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using FSM.Base;
	using FSM.Modifyler.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public class InitialEnterSMParallelFSM<TOwner, TInternalFSM, TEnum>
		: SMParallelFSMModifyData<TOwner, TInternalFSM, TEnum>
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : SMFSM
		where TEnum : Enum
	{
		public override SMFSMModifyType _type => SMFSMModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._isInitialEntered )	{ return; }


			await UTask.WaitWhile(
				_owner._asyncCancelerOnDisableAndExit,
				() => _owner._fsms.Any( pair => !pair.Value._isInitialEntered )
			);

			_owner._isInitialEntered = true;
		}
	}
}