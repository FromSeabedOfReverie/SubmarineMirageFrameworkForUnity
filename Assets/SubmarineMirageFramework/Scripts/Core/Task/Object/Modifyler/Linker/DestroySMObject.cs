//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class DestroySMObject : SMObjectModifyData {
		public DestroySMObject( SMObject smObject ) : base( smObject ) {
			_type = ModifyType.Linker;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
#if TestSMTaskModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
			SMLog.Debug( $"{nameof( _group )} : {_group}" );
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( _object )} : {_object}" );
#endif

			if ( !_group.IsTop( _object ) ) {
				UnLinkObject( _object );
				_group.SetAllData();
			}

#if TestSMTaskModifyler
			SMLog.Debug( $"{nameof( _group )} : {_group}" );
			SMLog.Debug( $"{nameof( _object )} : {_object}" );
#endif

			try {
				await new ChangeActiveSMObject( _object, false, true ).Run();
				await new RunStateSMObject( _object, SMTaskRunState.Finalizing ).Run();
			} finally {
				_object.Dispose();
#if TestSMTaskModifyler
				SMLog.Debug( $"finally : {_object}" );
#endif
			}

#if TestSMTaskModifyler
			SMLog.Debug( $"{nameof( _owner )} : {_owner}" );
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}