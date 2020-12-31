//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Object.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class UnregisterObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public UnregisterObjectSMGroup( SMObject target ) : base( target ) {}


		public override async UniTask Run() {
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( _modifyler )} : {_modifyler}" );
			SMLog.Debug( $"{nameof( _target )} : {_target}" );
#endif

			if ( !_owner.IsTop( _target ) ) {
				SMObjectApplyer.Unlink( _target );
				_owner.SetAllData();
			}

#if TestGroupModifyler
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( _target )} : {_target}" );
#endif

			try {
				await new ChangeActiveSMObject( _target, false, true ).Run();
				await new RunStateSMGroup( _target, SMTaskRunState.Finalize ).Run();
			} finally {
				_target.Dispose();
#if TestGroupModifyler
				SMLog.Debug( $"finally : {_target}" );
#endif
			}

#if TestGroupModifyler
			SMLog.Debug( $"{nameof( _modifyler )} : {_modifyler}" );
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}