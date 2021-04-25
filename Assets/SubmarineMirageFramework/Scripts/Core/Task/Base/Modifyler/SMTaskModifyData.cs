//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMTaskModifyData : SMModifyData {
		protected async UniTask RunLower( SMTaskRunAllType type, Func<SMTaskModifyData> createEvent ) {
			var tasks = GetLowers( type );
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:
					foreach ( var t in tasks ) {
						await t._modifyler.RegisterAndRun( createEvent() );
					}
					return;
				case SMTaskRunAllType.Parallel:
					await tasks.Select( t =>
						t._modifyler.RegisterAndRun( createEvent() )
					);
					return;
			}
		}

		IEnumerable<SMTask> GetLowers( SMTaskRunAllType type ) {
			var taskType = ToTaskType( type );
			var tasks = GetAllLowers()
				.Where( t => IsTargetLower( t, taskType ) );
			switch ( type ) {
				case SMTaskRunAllType.ReverseSequential:	return tasks.Reverse();
				default:									return tasks;
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

		protected virtual IEnumerable<SMTask> GetAllLowers()
			=> Enumerable.Empty<SMTask>();

		protected virtual bool IsTargetLower( SMTask lowerTask, SMTaskType type )
			=> true;
	}
}