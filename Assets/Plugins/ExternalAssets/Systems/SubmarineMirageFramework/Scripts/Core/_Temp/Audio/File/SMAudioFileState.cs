//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {



	/// <summary>音書類の読込状態</summary>
	public enum SMAudioFileState {
		/// <summary>読込解除</summary>
		Unload,
		/// <summary>読込中</summary>
		Loading,
		/// <summary>読込完了</summary>
		Loaded,
		/// <summary>読込失敗</summary>
		LoadError,
	}
}