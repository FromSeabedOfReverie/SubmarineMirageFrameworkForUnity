//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ Unity物の生情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public abstract class UnityObjectRawData : ISerializeData {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() {
			return this.ToDeepString();
		}
	}
}