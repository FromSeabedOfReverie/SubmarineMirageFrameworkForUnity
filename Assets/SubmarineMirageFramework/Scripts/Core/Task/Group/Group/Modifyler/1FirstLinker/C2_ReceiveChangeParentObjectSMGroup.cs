//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReceiveChangeParentObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstLinker;
		[SMShowLine] SMObjectBody _parent	{ get; set; }


		public ReceiveChangeParentObjectSMGroup( SMObjectBody target, SMObjectBody parent ) : base( target ) {
			_parent = parent;
		}

		protected override void Cancel() {
			_target.DisposeAllChildren();
			_target._gameObject.Destroy();
		}


		public override async UniTask Run() {
			_parent.LinkChild( _target );

			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );

			await UTask.DontWait();
		}
	}
}