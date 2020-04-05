//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Audio {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 音再生管理のインタフェース
	///----------------------------------------------------------------------------------------------------
	///		音源クラスに、ジェネリッククラスを持たせられない為、作成。
	/// </summary>
	///====================================================================================================
	public interface IAudioManager {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>各種書類音量の一覧</summary>
		Dictionary< Type, Dictionary<object, float> > _defaultVolumes	{ get; }
		/// <summary>音の発生源</summary>
		GameObject _speaker	{ get; }
		/// <summary>一時停止中か？</summary>
		bool _isPause	{ get; }
	}
}