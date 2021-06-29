//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestModifyData
namespace SubmarineMirage.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;
	using Extension;
	using Debug;



	public abstract class SMModifyData : SMLightBase {
		protected object _target			{ get; private set; }
		protected SMModifyler _modifyler	{ get; private set; }

		[SMShowLine] public abstract SMModifyType _type	{ get; }

		[SMShowLine] public bool _isFinished	{ get; private set; }
		[SMShow] bool _isCalledDestructor		{ get; set; }



		public SMModifyData() {
#if TestModifyData
//			SMLog.Debug( $"{this.GetAboutName()} : \n{this}" );
#endif
		}

		public virtual void Set( object target, SMModifyler modifyler ) {
			_target = target;
			_modifyler = modifyler;
#if TestModifyData
			SMLog.Debug( $"{nameof( Set )} : \n{this}" );
#endif
		}

		~SMModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
#if TestModifyData
			SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
			Finish();
			if ( !_isCalledDestructor ) {
				Cancel();
			}
#if TestModifyData
			SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
		}



		protected virtual void Cancel() {
#if TestModifyData
			SMLog.Debug( $"{nameof( Cancel )} : \n{this}" );
#endif
		}



		public abstract UniTask Run();

		public void Finish() {
			if ( _isFinished )	{ return; }

			_isFinished = true;
#if TestModifyData
			SMLog.Debug( $"{nameof( Finish )} : \n{this}" );
#endif
		}
	}
}