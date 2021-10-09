//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using Base;
	using Event;
	///====================================================================================================
	/// <summary>
	/// ■ 情報管理の基盤インターフェース
	///		全情報管理クラスにて、ジェネリック辞書を作れないので、インターフェースを作成。
	/// </summary>
	///====================================================================================================
	public interface IBaseSMDataManager : IBaseSM, ISMSerializeData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>名前</summary>
		[SMShow] string _name	{ get; }

		/// <summary>読込イベント</summary>
		SMAsyncEvent _loadEvent { get; }
		/// <summary>書込イベント</summary>
		SMAsyncEvent _saveEvent { get; }
	}
}