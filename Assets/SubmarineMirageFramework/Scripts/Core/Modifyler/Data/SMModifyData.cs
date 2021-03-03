//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMModifyData<TTarget, TData> : SMLightBase, ISMModifyData
		where TTarget : IBaseSMModifyTarget
		where TData : ISMModifyData
	{
		protected TTarget _target	{ get; private set; }
		protected SMModifyler<TTarget, TData> _modifyler	{ get; private set; }

		[SMShowLine] public abstract SMModifyType _type	{ get; }
		[SMShow] bool _isCalledDestructor	{ get; set; }



		public SMModifyData() {
//			SMLog.Debug( $"{this.GetAboutName()}() : {this}" );
		}

		~SMModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
			if ( _isCalledDestructor )	{ return; }
			Cancel();
		}


		protected virtual void Cancel() {}


		public virtual void Set( IBaseSMModifyTarget target, ISMModifyler modifyler ) {
			_target = (TTarget)target;
			_modifyler = (SMModifyler<TTarget, TData>)modifyler;
		}


		public abstract UniTask Run();
	}
}