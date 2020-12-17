//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Debug;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMObject : SMObjectModifyData {
		[SMShowLine] bool _isActive	{ get; set; }
		[SMShowLine] bool _isChangeOwner	{ get; set; }


		public ChangeActiveSMObject( SMObject smObject, bool isActive, bool isChangeOwner ) : base( smObject ) {
			_isActive = isActive;
			_isChangeOwner = isChangeOwner;
			_type = ModifyType.Runner;
		}

		public override void Cancel() {}



		public override async UniTask Run() {
			if ( _group._type == SMTaskType.DontWork )	{ return; }
			if ( !_object._isGameObject ) {
				await ChangeActiveSMBehaviour.RegisterAndRun( _object._behaviour, _isActive );
				return;
			}
			if ( !IsCanChangeActive( _object, _isChangeOwner ) )	{ return; }

			if ( _isActive )	{ ChangeActiveOfGameObject(); }
			switch ( _group._type ) {
				case SMTaskType.FirstWork:
					if ( _isActive )	{ await SequentialRun( _object, _isActive ); }
					else				{ await ReverseRun( _object, _isActive ); }
					break;
				case SMTaskType.Work:
					await ParallelRun( _object, _isActive );
					break;
			}
			if ( !_isActive )	{ ChangeActiveOfGameObject(); }
		}

		void ChangeActiveOfGameObject() {
			if ( !_isChangeOwner )	{ return; }
			_object._owner.SetActive( _isActive );
#if TestSMTaskModifyler
			SMLog.Debug( $"{nameof( ChangeActiveOfGameObject )} : {_isActive}\n{_object}" );
#endif
		}

		async UniTask SequentialRun( SMObject smObject, bool isActive ) {
			if ( !IsCanChangeActive( smObject, false ) )	{ return; }
			foreach ( var b in smObject.GetBehaviours() )
													{ await ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ); }
			foreach ( var o in smObject.GetChildren() )	{ await SequentialRun( o, isActive ); }
		}

		async UniTask ReverseRun( SMObject smObject, bool isActive ) {
			if ( !IsCanChangeActive( smObject, false ) )	{ return; }
			foreach ( var o in smObject.GetChildren().Reverse() )	{ await ReverseRun( o, isActive ); }
			foreach ( var b in smObject.GetBehaviours().Reverse() )
													{ await ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ); }
		}

		async UniTask ParallelRun( SMObject smObject, bool isActive ) {
			if ( !IsCanChangeActive( smObject, false ) )	{ return; }
			await Enumerable.Empty<UniTask>()
				.Concat(
					smObject.GetBehaviours().Select( b => ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o, isActive ) ) );
		}
	}
}