//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
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
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			Log.Debug( $"{nameof( _group )} : {_group}" );
			Log.Debug( $"{nameof( _owner )} : {_owner}" );
			Log.Debug( $"{nameof( _object )} : {_object}" );
#endif

			if ( !_group.IsTop( _object ) ) {
				UnLinkObject( _object );
				_group.SetAllData();
			}

#if TestSMTaskModifyler
			Log.Debug( $"{nameof( _group )} : {_group}" );
			Log.Debug( $"{nameof( _object )} : {_object}" );
#endif

			try {
				await new ChangeActiveSMObject( _object, false, true ).Run();
				await new RunStateSMObject( _object, SMTaskRunState.Finalizing ).Run();
			} finally {
				_object.Dispose();
#if TestSMTaskModifyler
				Log.Debug( $"finally : {_object}" );
#endif
			}

#if TestSMTaskModifyler
			Log.Debug( $"{nameof( _owner )} : {_owner}" );
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}