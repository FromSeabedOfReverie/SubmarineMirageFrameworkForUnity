//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {



	/// <summary>デバッグ記録の付箋</summary>
	public enum SMLogTag {
		/// <summary>通常のデバッグ記録</summary>
		None,
		/// <summary>仕事のデバッグ記録</summary>
		Task,
		/// <summary>サービスのデバッグ記録</summary>
		Service,
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
}