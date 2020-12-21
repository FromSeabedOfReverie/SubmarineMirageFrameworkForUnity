//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class UnregisterSMGroup : SMGroupModifyData {
		public UnregisterSMGroup( SMGroup target ) : base( target )
			=> _type = SMTaskModifyType.Linker;

		protected override void Cancel() {}


		public override async UniTask Run() {
			_target.Dispose();
			await UTask.DontWait();
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Run )} : {this}" );
#endif
		}
	}
}