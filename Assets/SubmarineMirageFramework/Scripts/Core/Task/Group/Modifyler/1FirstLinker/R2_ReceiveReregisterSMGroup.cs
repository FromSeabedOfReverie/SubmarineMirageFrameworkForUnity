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



	public class ReceiveReregisterSMGroup : SMGroupModifyData {
		public ReceiveReregisterSMGroup( SMGroup target ) : base( target )
			=> _type = SMTaskModifyType.FirstLinker;

		protected override void Cancel() => _target.Dispose();


		public override async UniTask Run() {
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
#endif
			SMGroupApplyer.Link( _owner, _target );

			await UTask.DontWait();

#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}