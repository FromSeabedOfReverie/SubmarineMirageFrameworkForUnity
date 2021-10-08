using UnityEngine;
using UniRx;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;



/// <summary>
/// ■ 入力の設定クラス
/// </summary>
public class SMInputSetting : BaseSMInputSetting {

	/// <summary>
	/// ● 設定
	/// </summary>
	public override void Setup( SMInputManager inputManager ) {
		base.Setup( inputManager );

		SetTouchTile();
	}

	/// <summary>
	/// ● タイル接触を設定
	/// </summary>
	void SetTouchTile() {
		var layerManager = SMServiceLocator.Resolve<SMUnityLayerManager>();
		var hit = new RaycastHit();


		_inputManager._updateEvent.AddLast().Subscribe( _ => {
			if (
				!_inputManager.GetKey( SMInputKey.Finger1 )._isEnabling ||
				!_inputManager.GetKey( SMInputKey.Nothing )._isEnabling ||
				Camera.main == null
			) {
				_inputManager._isTouchTile.Value = false;
				return;
			}

			var ray = Camera.main.ScreenPointToRay( _inputManager.GetAxis( SMInputAxis.Mouse ) );
			var layer = layerManager.GetMask( SMUnityLayer.Tile );
			var isHit = Physics.Raycast( ray, out hit, 100, layer );
			_inputManager._isTouchTile.Value = isHit;
		} );


		_inputManager._updateEvent.AddLast().Subscribe( _ => {
			if (
				!_inputManager._isTouchTile.Value ||
				_inputManager._isRotateCamera.Value
			) {
				_inputManager._touchTileID.Value = TileManagerView.NONE_ID;
				return;
			}

			var go = hit.collider.gameObject;
			var tile = go.GetComponent<TileView>();
			_inputManager._touchTileID.Value = tile._tileID;
		} );


		var isRotateCameraFinger1 = false;
		_inputManager._updateEvent.AddLast().Subscribe( _ => {
			if (
				_inputManager.GetKey( SMInputKey.Finger1 )._isEnabled &&
				!_inputManager._isTouchTile.Value &&
				_inputManager.GetKey( SMInputKey.Nothing )._isEnabled
			) {
				isRotateCameraFinger1 = true;
			}
			if ( _inputManager.GetKey( SMInputKey.Finger1 )._isDisabled ) {
				isRotateCameraFinger1 = false;
			}

			_inputManager._isRotateCamera.Value = (
				isRotateCameraFinger1 ||
				_inputManager.GetKey( SMInputKey.Finger2 )._isEnabling
			);
		} );
	}
}