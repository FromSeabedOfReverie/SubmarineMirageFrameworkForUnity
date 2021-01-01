//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupManagerModifyler
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		SMGroup _target	{ get; set; }


		public SendReregisterGroupSMGroupManager( SMGroup target )
			=> _target = target;


		public override async UniTask Run() {
#if TestGroupManagerModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
			if ( _target._groups != null )	{ SMLog.Debug( $"new : {_target._groups}" ); }
			if ( _owner != null )			{ SMLog.Debug( $"last : {_owner}" ); }
#endif

			if ( _target._isGameObject && _target._scene != _owner._owner ) {
				SceneManager.MoveGameObjectToScene( _target._gameObject, _target._scene._scene );
#if TestGroupManagerModifyler
				SMLog.Debug( string.Join( "\n",
					"シーン移動 :",
					_target._lifeSpan,
					_target.ToLineString(),
					_target._scene._scene
				) );
			} else {
				SMLog.Debug( string.Join( "\n",
					"シーン移動なし :",
					_target._lifeSpan,
					_target.ToLineString(),
					_target._scene._scene
				) );
#endif
			}

			SMGroupManagerApplyer.Unlink( _owner, _target );
			_target._groups._modifyler.Register( new ReceiveReregisterGroupSMGroupManager( _target ) );
			_modifyler.Reregister( _target._groups, _target );


			await UTask.DontWait();

#if TestGroupManagerModifyler
			if ( _target._groups != null )	{ SMLog.Debug( $"new : {_target._groups}" ); }
			if ( _owner != null )			{ SMLog.Debug( $"last : {_owner}" ); }
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}
	}
}