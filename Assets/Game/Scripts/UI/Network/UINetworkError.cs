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
	using UniRx;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using SubmarineMirage.Service;
	using SubmarineMirage.Audio;
	using SubmarineMirage.Scene;
	using SubmarineMirage.Network;
	using SubmarineMirage.Utility;
	using SubmarineMirage.Setting;
	using SubmarineMirage.Debug;



	public class UINetworkError : SMStandardMonoBehaviour {
		[SerializeField] Text _info;

		SMSceneManager _sceneManager { get; set; }
		SMAudioManager _audioManager { get; set; }
		SMNetworkManager _networkManager { get; set; }
		SMGameServerModel _gameServer { get; set; }



		protected override void StartAfterInitialize() {
			gameObject.DontDestroyOnLoad();

			_sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			_audioManager = SMServiceLocator.Resolve<SMAudioManager>();
			_networkManager = SMServiceLocator.Resolve<SMNetworkManager>();
			_gameServer = _networkManager._gameServerModel;

			SetError( null );

			var buttons = GetComponentsInChildren<Button>( true );
			buttons.ForEach( b => {
				b.onClick.AddListener( () => {
					UTask.Void( async () => {
						switch ( b.name ) {
							case "ButtonClose": {
								_audioManager.Play( SMSE.Decide ).Forget();
								SetError( null );

								var mainFSM = _sceneManager.GetFSM<MainSMScene>();
								if ( mainFSM._state is GameSMScene ) {
//									mainFSM.ChangeState<NetworkSMScene>().Forget();
								}
								return;
							}
						}

						await UTask.DontWait();
					} );
				} );
			} );

			_disposables.AddFirst(
				_gameServer._errorEvent.Subscribe( e => SetError( e ) )
			);
		}



		void SetError( GameServerSMException error ) {
			if ( error == null ) {
				_info.text = string.Empty;
				gameObject.SetActive( false );
				return;
			}

			_info.text = string.Join( "\n",
				$"{error._typeText}",
				$"{error._type}",
				$"{error._internalType}",
				$"{error._text}"
			);
			gameObject.SetActive( true );
		}

		public void SetErrorText( string text ) {
			if ( text.IsNullOrEmpty() ) {
				_info.text = string.Empty;
				gameObject.SetActive( false );
				return;
			}

			_info.text = text;
			gameObject.SetActive( true );
		}
	}
}