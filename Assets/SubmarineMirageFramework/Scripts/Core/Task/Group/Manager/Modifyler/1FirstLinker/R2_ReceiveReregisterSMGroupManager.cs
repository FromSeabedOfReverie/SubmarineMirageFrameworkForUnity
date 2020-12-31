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



	public class ReceiveReregisterSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstLinker;


		public ReceiveReregisterSMGroupManager( SMGroup target ) : base( target ) {}

		protected override void Cancel() => _target.Dispose();


		public override async UniTask Run() {
#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
#endif
			SMGroupManagerApplyer.Link( _owner, _target );

			await UTask.DontWait();

#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}