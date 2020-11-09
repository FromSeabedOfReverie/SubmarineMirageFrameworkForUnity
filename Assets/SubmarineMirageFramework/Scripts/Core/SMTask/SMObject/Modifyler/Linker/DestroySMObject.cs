//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;



	// TODO : コメント追加、整頓



	public class DestroySMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public DestroySMObject( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() {}


		public override async UniTask Run() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			Log.Debug( $"{nameof( _object._top._modifyler )} : {_object?._top?._modifyler}" );
			Log.Debug( $"{nameof( _object._top )} : {_object?._top}" );
			Log.Debug( $"{nameof( _object )} : {_object}" );
#endif
			var lastGroup = _object._group;
			var isLastTop = lastGroup.IsTop( _object );
			UnLinkObject( _object );
			if ( !isLastTop )	{ lastGroup.SetAllData(); }
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( _object._top )} : {_object?._top}" );
			Log.Debug( $"{nameof( _object )} : {_object}" );
#endif

			await RunObject();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( _object._top._modifyler )} : {_object?._top?._modifyler}" );
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}


		async UniTask RunObject() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( RunObject )} : start\n{this}" );
#endif
			try {
#if TestSMTaskModifyler
				Log.Debug( $"{nameof( ChangeActiveSMObject )} : 待機開始\n{_object}" );
#endif
				await new ChangeActiveSMObject( _object, false, true ).Run();
#if TestSMTaskModifyler
				Log.Debug( $"{nameof( ChangeActiveSMObject )} : 待機終了\n{_object}" );
				Log.Debug( $"{nameof( RunStateSMObject )} : 待機開始\n{_object}" );
#endif
				await new RunStateSMObject( _object, SMTaskRunState.Finalizing ).Run();
#if TestSMTaskModifyler
				Log.Debug( $"{nameof( RunStateSMObject )} : 待機終了\n{_object}" );
#endif

			} finally {
				_object.Dispose();
#if TestSMTaskModifyler
				Log.Debug( $"finally : {_object}" );
#endif
			}
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( RunObject )} : end\n{this}" );
#endif
		}
	}
}