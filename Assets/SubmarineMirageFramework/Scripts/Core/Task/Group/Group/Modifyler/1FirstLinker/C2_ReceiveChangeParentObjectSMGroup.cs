//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public class ReceiveChangeParentObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstLinker;
		[SMShowLine] SMObjectBody _parent	{ get; set; }


		public ReceiveChangeParentObjectSMGroup( SMObjectBody smObject, SMObjectBody parent ) : base( smObject ) {
			_parent = parent;
		}

		protected override void Cancel() {
			_object.DisposeAllChildren();
			_object._gameObject.Destroy();
		}


		public override async UniTask Run() {
			_parent.LinkChild( _object );

			_modifyler.Register( new AdjustObjectRunSMGroup( _object ) );

			await UTask.DontWait();
		}
	}
}