//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMModifyData : SMLightBase {
		public ISMModifyTarget _target	{ get; private set; }
		protected SMModifyler _modifyler	{ get; private set; }

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


		public virtual void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			_target = target;
			_modifyler = modifyler;
		}


		public abstract UniTask Run();
	}
}