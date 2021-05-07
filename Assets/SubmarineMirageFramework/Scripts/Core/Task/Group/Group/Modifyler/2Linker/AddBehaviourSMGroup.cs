//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Extension;
	using Utility;
	using Debug;


	public class AddBehaviourSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;
		[SMShowLine] public SMBehaviour _behaviour	{ get; private set; }



		public AddBehaviourSMGroup( SMObjectBody smObject, Type type ) : base( smObject ) {
			_behaviour = (SMBehaviour)_object._gameObject.AddComponent( type );

			if ( _behaviour == null ) {
				throw new NotSupportedException(
					$"{nameof( SMBehaviour )}でない為、追加不可 :\n{type.GetAboutName()}" );
			}

			_behaviour.Constructor();
		}


		protected override void Cancel() {
			_behaviour.Dispose();
			_behaviour.Destroy();
		}



		public override async UniTask Run() {
			if ( _target._isFinalizing ) {
				Cancel();
				return;
			}

			_object._behaviourBody.Link( _behaviour._body );
			_behaviour._body.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}