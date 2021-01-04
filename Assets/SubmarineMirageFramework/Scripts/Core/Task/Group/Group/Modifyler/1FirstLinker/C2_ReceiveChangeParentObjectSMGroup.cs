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
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class ReceiveChangeParentObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstLinker;
		[SMShowLine] SMObject _parent	{ get; set; }


		public ReceiveChangeParentObjectSMGroup( SMObject target, SMObject parent ) : base( target )
			=> _parent = parent;

		protected override void Cancel() => _target.Dispose();


		public override async UniTask Run() {
			SMObjectApplyer.LinkChild( _parent, _target );
			SMGroupApplyer.SetAllData( _owner );

			_modifyler.Register( new ChangeParentActiveObjectSMGroup( _target ) );

			await UTask.DontWait();
		}
	}
}