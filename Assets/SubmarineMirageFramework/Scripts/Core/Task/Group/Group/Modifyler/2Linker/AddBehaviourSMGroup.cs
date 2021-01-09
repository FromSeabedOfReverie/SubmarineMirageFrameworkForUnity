//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Behaviour;
	using Behaviour.Modifyler;
	using Object;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class AddBehaviourSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] public SMMonoBehaviour _behaviour	{ get; private set; }



		public AddBehaviourSMGroup( SMObject target, Type type ) : base( target ) {
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

			SMBehaviourApplyer.Link( _target, _behaviour );
			SMGroupApplyer.SetAllData( _owner );

			_behaviour.Constructor();
			await SMBehaviourApplyer.RegisterRunEventToOwner( _target, _behaviour );
		}
	}
}