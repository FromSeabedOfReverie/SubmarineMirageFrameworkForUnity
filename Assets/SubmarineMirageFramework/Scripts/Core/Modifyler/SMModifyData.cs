//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestModifyData
namespace SubmarineMirage.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;
	using Extension;
	using Debug;



	public abstract class SMModifyData : SMLightBase {
		public ISMModifyTarget _target	{ get; private set; }
		protected SMModifyler _modifyler	{ get; private set; }

		[SMShowLine] public abstract SMModifyType _type	{ get; }

		[SMShowLine] public bool _isFinished	{ get; private set; }
		[SMShow] bool _isCalledDestructor		{ get; set; }



		public SMModifyData() {
#if TestModifyData
			SMLog.Debug( $"{this.GetAboutName()} : {this}" );
#endif
		}

		~SMModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
			Finish();
			if ( _isCalledDestructor )	{ return; }
			Cancel();
		}


		protected virtual void Cancel() {}


		public virtual void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			_target = target;
			_modifyler = modifyler;
		}


		public abstract UniTask Run();

		public void Finish() {
			if ( _isFinished )	{ return; }

			_isFinished = true;
#if TestModifyData
			SMLog.Debug( $"{nameof( Finish )} : {this}" );
#endif
		}
	}
}