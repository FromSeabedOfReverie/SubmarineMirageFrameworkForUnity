//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Base;



	// TODO : コメント追加、整頓



	public interface IBaseSMFSMModifylerOwner<TModifyler> : ISMStandardBase
		where TModifyler : IBaseSMFSMModifyler
	{
		SMFSMRunState _ranState	{ get; set; }
		bool _isInitialized	{ get; }
		bool _isOperable	{ get; }
		bool _isFinalizing	{ get; set; }
		bool _isActive	{ get; }
		TModifyler _modifyler	{ get; }
	}
}