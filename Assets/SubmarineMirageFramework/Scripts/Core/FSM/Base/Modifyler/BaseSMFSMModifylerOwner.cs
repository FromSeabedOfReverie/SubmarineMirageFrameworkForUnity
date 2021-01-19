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
		public abstract bool _isInitialized	{ get; }
		public abstract bool _isOperable	{ get; }
		public abstract bool _isFinalizing	{ get; }
		public abstract bool _isActive		{ get; }
		public TModifyler _modifyler	{ get; protected set; }


		public BaseSMFSMModifylerOwner() {
			_disposables.AddLast( () => {
				_modifyler.Dispose();
			} );
		}
	}
}