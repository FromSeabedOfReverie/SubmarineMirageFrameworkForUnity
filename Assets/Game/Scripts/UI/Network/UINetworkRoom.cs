//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using UnityEngine;
	using UnityEngine.UI;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage;



	public class UINetworkRoom : SMStandardMonoBehaviour {
		UINetwork _owner { get; set; }

		SMGameServerRoom _room;

		[SerializeField] Text _name;
		[SerializeField] Text _playerCount;
		[SerializeField] Text _info;
		[SerializeField] Button _button;



		void Start() {
			_button?.onClick.AddListener( () =>
				_owner.ClickEnterRoom( _room ).Forget()
			);
		}

		public void Setup( UINetwork owner, SMGameServerRoom room ) {
			_owner = owner;
			_room = room;

			if ( _room == null ) {
				_name.text = string.Empty;
				_playerCount.text = string.Empty;
				return;
			}

			_name.text = _room._name;
			_playerCount.text = $"{_room._playerCount} / {_room._maxPlayerCount} 人";

			if ( _info != null ) {
				_info.text = _room._isActive ? string.Empty : "不可";
			}
			if ( _button != null ) {
				_button.interactable = _room._isActive;
			}
		}
	}
}