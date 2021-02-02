//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class AddBehaviourSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] public SMMonoBehaviour _behaviour	{ get; private set; }



		public AddBehaviourSMGroup( SMObjectBody target, Type type ) : base( target ) {
			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_target}" );
			}
			_behaviour = (SMMonoBehaviour)_target._gameObject.AddComponent( type );
			if ( _behaviour == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{type}" );
			}
		}


		protected override void Cancel() {
			_behaviour.Dispose();
			_behaviour.Destroy();
		}



		public override async UniTask Run() {
			if ( _owner._isFinalizing ) {
				Cancel();
				return;
			}

			_target._behaviourBody.Link( _behaviour._body );
			_owner.SetAllData();

			_behaviour._body.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}