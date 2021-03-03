//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using SubmarineMirage.Modifyler.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMTaskModifyData<TTarget, TData, TLowerData, TLowerTarget>
		: SMModifyData<TTarget, TData>
		where TTarget : ISMTaskModifyTarget
		where TData : ISMModifyData
	{
		protected async UniTask RunLower( SMTaskRunAllType type, Func<TLowerData> createEvent ) {
			var ls = GetLowers( type );
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:
					foreach ( var l in ls ) {
						await RegisterAndRunLower( l, createEvent() );
					}
					return;
				case SMTaskRunAllType.Parallel:
					await ls.Select( l =>
						RegisterAndRunLower( l, createEvent() )
					);
					return;
			}
		}

		protected abstract UniTask RegisterAndRunLower( TLowerTarget lowerTarget, TLowerData data );


		IEnumerable<TLowerTarget> GetLowers( SMTaskRunAllType type ) {
			var t = ToTaskType( type );
			var ls = GetAllLowers()
				.Where( l => IsTargetLower( l, t ) );
			switch ( type ) {
				case SMTaskRunAllType.ReverseSequential:	return ls.Reverse();
				default:									return ls;
			}
		}

		SMTaskType ToTaskType( SMTaskRunAllType type ) {
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:	return SMTaskType.FirstWork;
				case SMTaskRunAllType.Parallel:				return SMTaskType.Work;
				default:									return SMTaskType.DontWork;
			}
		}

		protected abstract IEnumerable<TLowerTarget> GetAllLowers();

		protected abstract bool IsTargetLower( TLowerTarget lowerTarget, SMTaskType type );
	}
}