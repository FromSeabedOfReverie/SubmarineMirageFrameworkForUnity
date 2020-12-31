//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class UnregisterSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;


		public UnregisterSMGroupManager( SMGroup target ) : base( target ) {}

		protected override void Cancel() {}


		public override async UniTask Run() {
			_target.Dispose();
			await UTask.DontWait();
#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Run )} : {this}" );
#endif
		}
	}
}