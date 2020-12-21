//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskModifyler
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTaskModifyData<TOwner, TModifyler, TTarget>
		: SMLightBase, IBaseSMTaskModifyData<TOwner, TModifyler, TTarget>
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
		where TTarget : IBaseSMTaskModifyDataTarget
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		[SMShowLine] public TTarget _target	{ get; private set; }
		[SMShowLine] public SMTaskModifyType _type	{ get; protected set; }
		bool _isCalledDestructor	{ get; set; }



		public BaseSMTaskModifyData( TTarget target ) {
			_target = target;
#if TestTaskModifyler
			SMLog.Debug( $"{this.GetAboutName()}() : {this}" );
#endif
		}

		~BaseSMTaskModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
			if ( !_isCalledDestructor )	{ Cancel(); }
		}

		protected abstract void Cancel();


		public void Set( TOwner owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}


		public abstract UniTask Run();
	}
}