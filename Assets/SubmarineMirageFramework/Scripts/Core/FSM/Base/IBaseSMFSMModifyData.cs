//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;



	// TODO : コメント追加、整頓



	public interface IBaseSMFSMModifyData<TOwner, TModifyler> : ISMLightBase
		where TOwner : IBaseSMFSMModifylerOwner<TModifyler>
		where TModifyler : IBaseSMFSMModifyler
	{
		SMFSMModifyType _type	{ get; }

		void Set( TOwner owner );
		UniTask Run();
	}
}