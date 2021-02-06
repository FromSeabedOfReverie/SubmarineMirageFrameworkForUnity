//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using SubmarineMirage.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTaskModifylerOwner<TModifyler>
		: SMStandardBase, IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
	{
		public SMTaskRunState _ranState	{ get; set; }
		public SMTaskActiveState _activeState	{ get; set; }
		public bool _isInitialized => _ranState >= SMTaskRunState.InitialEnable;
		public bool _isOperable => _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive => _activeState == SMTaskActiveState.Enable;
		public TModifyler _modifyler	{ get; protected set; }


		public BaseSMTaskModifylerOwner() {
			_disposables.AddLast( () => {
				_modifyler.Dispose();
			} );
		}
	}
}