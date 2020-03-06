//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {
	public class GeneralFiniteStateMachine<TOwner> :
		FiniteStateMachine< TOwner, GeneralFiniteStateMachine<TOwner> >
		where TOwner : IFiniteStateMachineOwner< GeneralFiniteStateMachine<TOwner> >
	{
		public GeneralFiniteStateMachine( TOwner owner,
											State< TOwner, GeneralFiniteStateMachine<TOwner> >[] states )
			: base( owner, states )
		{
		}
	}
}