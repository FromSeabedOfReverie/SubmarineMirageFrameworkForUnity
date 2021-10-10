//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using SubmarineMirage.Service;
	using SubmarineMirage.Audio;
	using SubmarineMirage.Scene;
	using SubmarineMirage.Network;
	using SubmarineMirage.Extension;
	using SubmarineMirage.Utility;
	using SubmarineMirage.Setting;
	using SubmarineMirage.Debug;



	public class UINetwork : SMStandardMonoBehaviour {
		[SerializeField] Transform _pageTop;
		readonly List<GameObject> _pages = new List<GameObject>();

		[SerializeField] Transform _roomTop;
		[SerializeField] GameObject _room;
		[SerializeField] UINetworkRoom _page3Room;

		[SerializeField] GameObject _buttonGuard;

		SMSceneManager _sceneManager { get; set; }
		SMAudioManager _audioManager { get; set; }
		SMNetworkManager _networkManager { get; set; }
		SMGameServerModel _gameServer { get; set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		protected override void StartAfterInitialize() {
			_sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			_audioManager = SMServiceLocator.Resolve<SMAudioManager>();
			_networkManager = SMServiceLocator.Resolve<SMNetworkManager>();
			_gameServer = _networkManager._gameServerModel;


			foreach ( Transform t in _pageTop ) {
				t.gameObject.SetActive( false );
				_pages.Add( t.gameObject );
			}
			foreach ( RectTransform t in _roomTop ) {
				t.gameObject.Destroy();
			}

			ChangePage( 0 );
			ChangeButtonGuard( false );


			_disposables.AddFirst(
				_gameServer._playerCountEvent
					.Where( i => i == SMNetworkManager.MAX_PLAYERS )
					.Subscribe( i => {
						UTask.Void( async () => {
							ChangeButtonGuard( true );
							if ( _gameServer._isServer ) {
								_gameServer._isLockRoom = true;
							}
							await _audioManager.Play( SMSE.Title );
							_sceneManager.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
						} );
					} )
			);

			_disposables.AddFirst(
				_gameServer._currentRoomEvent.Subscribe( r => {
					_page3Room.Setup( this, r );
				} )
			);

			_disposables.AddFirst(
				_gameServer._roomsEvent.Subscribe( rs => {
					foreach ( RectTransform t in _roomTop ) {
						t.gameObject.Destroy();
					}

					rs.ForEach( r => {
						var go = _room.Instantiate( _roomTop );
						var ui = go.GetComponent<UINetworkRoom>();
						ui.Setup( this, r );
					} );
				} )
			);

			_disposables.AddFirst(
				_gameServer._errorEvent
					.Where( e => e._isDisconnect )
					.Subscribe( e => {
						ChangePage( 0 );
					} )
			);

			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );


			var buttons = GetComponentsInChildren<Button>( true );
			buttons.ForEach( b => {
				b.onClick.AddListener( () => {
					UTask.Void( async () => {
						ChangeButtonGuard( true );

						switch ( b.name ) {
							case "ButtonOffline": {
								await _audioManager.Play( SMSE.Title );
								if ( await _gameServer.Connect( false, $"プレイヤー" ) ) {
									if ( await _gameServer.CreateRoom(
											$"お祭り会場", string.Empty, SMNetworkManager.MAX_PLAYERS )
									) {
										_sceneManager.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
									}
								}
								return;
							}

							case "ButtonOnline": {
								_audioManager.Play( SMSE.Decide ).Forget();
								if ( await _gameServer.Connect( true, $"プレイヤー{DateTime.Now}" ) ) {
									if ( await _gameServer.EnterLobby() ) {
										ChangePage( 1 );
									}
								}
								ChangeButtonGuard( false );
								return;
							}

							case "ButtonEnd": {
								await _audioManager.Play( SMSE.Decide );
								if ( await _gameServer.Disconnect() ) {
								}
								_sceneManager.GetFSM<MainSMScene>().ChangeState<TitleSMScene>().Forget();
								return;
							}

							case "ButtonCreateRoom": {
								_audioManager.Play( SMSE.Decide ).Forget();
								if ( await _gameServer.CreateRoom(
										$"お祭り会場 {DateTime.Now}", string.Empty, SMNetworkManager.MAX_PLAYERS )
								) {
									ChangePage( 2 );
								}
								ChangeButtonGuard( false );
								return;
							}

							case "ButtonPage0": {
								_audioManager.Play( SMSE.Decide ).Forget();
								if ( await _gameServer.Disconnect() ) {
								}
								ChangePage( 0 );
								ChangeButtonGuard( false );
								return;
							}

							case "ButtonPage1": {
								_audioManager.Play( SMSE.Decide ).Forget();
								if ( await _gameServer.EnterLobby() ) {
									ChangePage( 1 );
								}
								ChangeButtonGuard( false );
								return;
							}
						}
					} );
				} );
			} );
		}



		public async UniTask ClickEnterRoom( SMGameServerRoom room ) {
			ChangeButtonGuard( true );
			_audioManager.Play( SMSE.Decide ).Forget();
			if ( await _gameServer.EnterRoom( room, string.Empty ) ) {
				ChangePage( 2 );
			}
			ChangeButtonGuard( false );
		}



		void ChangePage( int index ) {
			_pages.ForEach( go => go.SetActive( false ) );
			_pages[index].SetActive( true );
		}

		void ChangeButtonGuard( bool isActive ) {
			_buttonGuard.SetActive( isActive );
		}
	}
}