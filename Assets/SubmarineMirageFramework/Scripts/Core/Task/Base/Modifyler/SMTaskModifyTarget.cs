//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using SubmarineMirage.Modifyler;
	using SubmarineMirage.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMTaskModifyTarget<TTarget, TData>
		: SMModifyTarget<TTarget, TData>, ISMTaskModifyTarget
		where TTarget : ISMTaskModifyTarget
		where TData : ISMModifyData
	{
		public SMTaskRunState _ranState	{ get; set; }
		public SMTaskActiveState _activeState	{ get; set; }
		public bool _isInitialized => _ranState >= SMTaskRunState.InitialEnable;
		public bool _isOperable => _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive => _activeState == SMTaskActiveState.Enable;
	}
}