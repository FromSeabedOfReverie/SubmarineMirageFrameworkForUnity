//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;


	// TODO : コメント追加、整頓


	public class UnregisterSMGroup : SMGroupModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public UnregisterSMGroup() : base( null )	{}

		public override void Cancel()	{}


		public override async UniTask Run() {
			_group.Dispose();
			await UTask.DontWait();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : {this}" );
#endif
		}
	}
}