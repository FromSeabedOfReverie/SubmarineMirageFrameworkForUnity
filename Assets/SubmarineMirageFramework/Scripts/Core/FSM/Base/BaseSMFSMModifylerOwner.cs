//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Base;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifylerOwner<TModifyler>
		: SMStandardBase, IBaseSMFSMModifylerOwner<TModifyler>
		where TModifyler : IBaseSMFSMModifyler
	{
		public SMFSMRunState _ranState	{ get; set; }
		public bool _isInitialized => false;
		public bool _isOperable => _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive => false;
		public TModifyler _modifyler	{ get; protected set; }


		public BaseSMFSMModifylerOwner() {
			_disposables.AddLast( () => {
				_modifyler.Dispose();
			} );
		}
	}
}