//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ ゲームサーバーの部屋クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMGameServerRoom : SMLightBase {
		[SMShow] protected static readonly List<string> s_prohibitionWords = new List<string>();

		[SMShowLine] public string _name { get; protected set; }
		[SMShowLine] public string _password { get; protected set; }

		[SMShowLine] public int _playerCount { get; set; }
		[SMShowLine] public int _maxPlayerCount { get; protected set; }

		[SMShowLine] public abstract bool _isLock { get; set; }
		[SMShowLine] public bool _isActive { get; protected set; }



		public SMGameServerRoom() {
			Wrap();
		}

		public SMGameServerRoom( string name, string password, int maxPlayerCount ) {
			_name = name;
			_password = password;
			_maxPlayerCount = maxPlayerCount;
			_isActive = true;

			Wrap();
		}



		void Wrap() {
			_maxPlayerCount = Mathf.Clamp( _maxPlayerCount, 0, SMNetworkManager.MAX_PLAYERS );

			_name = _name ?? string.Empty;
			_password = _password ?? string.Empty;

			if ( IsUseProhibitionWord( _name ) ) {
				throw new GameServerSMException(
					SMGameServerErrorType.UseProhibitionWord,
					null,
					string.Join( "\n",
						$"{this.GetAboutName()}.{nameof( Wrap )} : 部屋名に、禁則単語を使用",
						$"{nameof( _name )} : {_name}"
					),
					false
				);
			}
			if ( IsUseProhibitionWord( _password ) ) {
				throw new GameServerSMException(
					SMGameServerErrorType.UseProhibitionWord,
					null,
					string.Join( "\n",
						$"{this.GetAboutName()}.{nameof( Wrap )} : パスワードに、禁則単語を使用",
						$"{nameof( _password )} : {_password}"
					),
					false
				);
			}
		}



		public bool IsUseProhibitionWord( string text )
			=> s_prohibitionWords.Any( pt => text.Contains( pt ) );

		public bool IsEqualPassword( string password ) {
			password = password ?? string.Empty;
			return _password == password;
		}

		public bool IsFull()
			=> _playerCount == _maxPlayerCount;



		public abstract string ToToken();
	}
}