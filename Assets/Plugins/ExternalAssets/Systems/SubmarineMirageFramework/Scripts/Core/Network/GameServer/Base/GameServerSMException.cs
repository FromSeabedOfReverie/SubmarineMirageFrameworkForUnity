//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Network {
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using KoganeUnityLib;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ ゲームサーバーの例外クラス
	/// </summary>
	///====================================================================================================
	public class GameServerSMException : SMException {
		static readonly Dictionary<SMGameServerErrorType, string> s_typeToText
			= new Dictionary<SMGameServerErrorType, string>() {
				{ SMGameServerErrorType.NoNetwork,			"ネットワークに未接続" },
				{ SMGameServerErrorType.UseProhibitionWord, "禁則単語を使用" },
				{ SMGameServerErrorType.MismatchPassword,	"パスワードが不一致" },
				{ SMGameServerErrorType.FullRoom,           "部屋が満室" },
				{ SMGameServerErrorType.Other,				"その他" },
			};

		[SMShowLine] public SMGameServerErrorType _type	{ get; private set; }
		[SMShowLine] public string _typeText			{ get; private set; }
		[SMShowLine] public Enum _internalType			{ get; private set; }
		[SMShowLine] public string _text				{ get; private set; }
		[SMShowLine] public bool _isDisconnect			{ get; private set; }



		public GameServerSMException() : base() {
		}

		public GameServerSMException( SMGameServerErrorType type, Enum internalType, string text, bool isDisconnect )
			: base( $"{ToTypeText( type )}, {type}, {internalType}, {text}, {isDisconnect}" )
		{
			_type = type;
			_typeText = ToTypeText( _type );
			_internalType = internalType;
			_text = text;
			_isDisconnect = isDisconnect;
		}

		public GameServerSMException( SMGameServerErrorType type, Enum internalType, string text, bool isDisconnect,
										Exception innerException
		) : base( $"{ToTypeText( type )}, {type}, {internalType}, {text}, {isDisconnect}", innerException )
		{
			_type = type;
			_typeText = ToTypeText( _type );
			_internalType = internalType;
			_text = text;
			_isDisconnect = isDisconnect;
		}

		protected GameServerSMException( SerializationInfo info, StreamingContext context ) : base( info, context ) {
		}



		static string ToTypeText( SMGameServerErrorType type )
			=> s_typeToText.GetOrDefault( type );
	}
}