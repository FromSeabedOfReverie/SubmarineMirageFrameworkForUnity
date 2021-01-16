//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base.Modifyler {
	using SubmarineMirage.Base;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifylerOwner<TOwner, TModifyler, TData> : SMStandardBase
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		public bool _isInitialized	{ get; set; }
		public bool _isOperable		=> _isInitialized && !_isFinalizing;
		public bool _isFinalizing	{ get; set; }
		public bool _isActive		{ get; set; }
		public TModifyler _modifyler	{ get; protected set; }


		public BaseSMFSMModifylerOwner() {
			_disposables.AddLast( () => {
				_isFinalizing = true;
				_modifyler.Dispose();
			} );
		}
	}
}