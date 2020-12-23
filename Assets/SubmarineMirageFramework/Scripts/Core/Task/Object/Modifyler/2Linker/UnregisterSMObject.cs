//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Debug;



	// TODO : コメント追加、整頓



	public class UnregisterSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public UnregisterSMObject( SMObject target ) : base( target ) {}

		protected override void Cancel() {}


		public override async UniTask Run() {
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( _modifyler )} : {_modifyler}" );
			SMLog.Debug( $"{nameof( _target )} : {_target}" );
#endif

			if ( !_owner.IsTop( _target ) ) {
				SMObjectApplyer.Unlink( _target );
				_owner.SetAllData();
			}

#if TestObjectModifyler
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( _target )} : {_target}" );
#endif

			try {
				await new ChangeActiveSMObject( _target, false, true ).Run();
				await new RunStateSMObject( _target, SMTaskRunState.Finalize ).Run();
			} finally {
				_target.Dispose();
#if TestObjectModifyler
				SMLog.Debug( $"finally : {_target}" );
#endif
			}

#if TestObjectModifyler
			SMLog.Debug( $"{nameof( _modifyler )} : {_modifyler}" );
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}