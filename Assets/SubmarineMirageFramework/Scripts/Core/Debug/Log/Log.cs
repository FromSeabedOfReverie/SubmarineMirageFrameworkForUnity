//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System.Linq;
	using UnityEngine;
	using KoganeUnityLib;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録のクラス
	///----------------------------------------------------------------------------------------------------
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化した基盤クラスを継承する。
	/// </summary>
	///====================================================================================================
	public class Log : BaseLog<Log.Tag> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>デバッグ記録の付箋</summary>
		public enum Tag {
			/// <summary>通常のデバッグ記録</summary>
			None,
			/// <summary>処理のデバッグ記録</summary>
			Process,
			/// <summary>シングルトンのデバッグ記録</summary>
			Singleton,
			/// <summary>有限状態機械のデバッグ記録</summary>
			FSM,
			/// <summary>ビルドのデバッグ記録</summary>
			Build,
			/// <summary>サーバーのデバッグ記録</summary>
			Server,
			/// <summary>広告のデバッグ記録</summary>
			Advertisement,
			/// <summary>アプリケーション内課金のデバッグ記録</summary>
			Purchase,
			/// <summary>情報のデバッグ記録</summary>
			Data,
			/// <summary>書類のデバッグ記録</summary>
			File,
			/// <summary>場面のデバッグ記録</summary>
			Scene,
			/// <summary>音のデバッグ記録</summary>
			Audio,
			/// <summary>画面のデバッグ記録</summary>
			UI,
			/// <summary>人工知能のデバッグ記録</summary>
			AI,
			/// <summary>ゲーム物のデバッグ記録</summary>
			GameObject,
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Log() : base(
#if DEVELOP
				true
#else
				false
#endif
		) {
			// NONE以外の付箋色を登録
			var max = EnumUtils.GetValues<Tag>()
				.Select( type => type != Tag.None )
				.Count();
			var delta = 1.0f / max;
			max.Times( i =>
				// HSV色相環で、順当に設定
				RegisterTag( (Tag)( i + 1 ), Color.HSVToRGB( i * delta, 1, 1 ) )
			);


#if false
			Log.Debug( Tag.Process,			Tag.Process );
			Log.Debug( Tag.Singleton,		Tag.Singleton );
			Log.Debug( Tag.FSM,				Tag.FSM );
			Log.Debug( Tag.Build,			Tag.Build );
			Log.Debug( Tag.Server,			Tag.Server );
			Log.Debug( Tag.Advertisement,	Tag.Advertisement );
			Log.Debug( Tag.Purchase,		Tag.Purchase );
			Log.Debug( Tag.Data,			Tag.Data );
			Log.Debug( Tag.File,			Tag.File );
			Log.Debug( Tag.Scene,			Tag.Scene );
			Log.Debug( Tag.Audio,			Tag.Audio );
			Log.Debug( Tag.UI,				Tag.UI );
			Log.Debug( Tag.AI,				Tag.AI );
			Log.Debug( Tag.GameObject,		Tag.GameObject );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タグを登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterTag( Tag tag, Color color ) {
			RegisterTag(
				tag,
#if UNITY_EDITOR
				DecorationManager.s_instance._uGUI.ByColor(
					string.Format( s_defaultFormat, tag ),
					color
				) + "{0}"
#else
				null
#endif
			);
		}
	}
}