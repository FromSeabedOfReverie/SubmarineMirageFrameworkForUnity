//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using SubmarineMirage.Base;



	// TODO : コメント追加、整頓



	public interface IBaseSMTaskModifylerOwner<TModifyler> : ISMStandardBase
		where TModifyler : IBaseSMTaskModifyler
	{
		SMTaskRunState _ranState	{ get; set; }
		SMTaskActiveState _activeState	{ get; set; }
		bool _isInitialized	{ get; }
		bool _isOperable	{ get; }
		bool _isFinalizing	{ get; set; }
		bool _isActive	{ get; }
		TModifyler _modifyler	{ get; }
	}
}