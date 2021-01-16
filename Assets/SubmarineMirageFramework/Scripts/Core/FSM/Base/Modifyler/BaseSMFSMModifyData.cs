//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifyData<TOwner, TModifyler, TData> : SMLightBase
		where TOwner : BaseSMFSMModifylerOwner<TOwner, TModifyler, TData>
		where TModifyler : BaseSMFSMModifyler<TOwner, TModifyler, TData>
		where TData : BaseSMFSMModifyData<TOwner, TModifyler, TData>
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		[SMShowLine] public abstract SMFSMModifyType _type	{ get; }



		public BaseSMFSMModifyData() {}

		public override void Dispose() {}


		public virtual void Set( TOwner owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}


		public abstract UniTask Run();
	}
}