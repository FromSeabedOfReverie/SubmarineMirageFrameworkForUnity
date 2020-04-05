//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM {
	///====================================================================================================
	/// <summary>
	/// ■ 一般的な、有限状態機械のクラス
	///----------------------------------------------------------------------------------------------------
	///		有限状態機械の基本機能のみを使い、拡張する必要が無い場合、このクラスを継承する。
	///		TFSMを定める為に、仲介している。
	/// </summary>
	///====================================================================================================
	public class GeneralStateMachine<TOwner> :
		FiniteStateMachine< TOwner, GeneralStateMachine<TOwner> >
		where TOwner : IFiniteStateMachineOwner< GeneralStateMachine<TOwner> >
	{
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public GeneralStateMachine( TOwner owner,
											State< TOwner, GeneralStateMachine<TOwner> >[] states )
			: base( owner, states )
		{
		}
	}
}