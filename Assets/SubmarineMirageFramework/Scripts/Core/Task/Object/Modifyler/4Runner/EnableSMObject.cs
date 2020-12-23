//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Behaviour.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class EnableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public EnableSMObject( SMObject target ) : base( target ) {}

		protected override void Cancel() {}



		public override async UniTask Run() {
			if ( _owner._type == SMTaskType.DontWork )	{ return; }

			if ( !_target._isGameObject ) {
				_target._behaviour._modifyler.Register( new EnableSMBehaviour( _target._behaviour._body ) );
				await _target._behaviour._modifyler.WaitRunning();
				return;
			}

			if ( !IsCanChange( _target, _isChangeOwner ) )	{ return; }

			if ( _isActive )	{ ChangeActiveOfGameObject(); }
			switch ( _owner._type ) {
				case SMTaskType.FirstWork:
					if ( _isActive )	{ await SequentialRun( _target, _isActive ); }
					else				{ await ReverseRun( _target, _isActive ); }
					break;
				case SMTaskType.Work:
					await ParallelRun( _target, _isActive );
					break;
			}
			if ( !_isActive )	{ ChangeActiveOfGameObject(); }
		}


		void ChangeActiveOfGameObject() {
			if ( !_isChangeOwner )	{ return; }
			_target._owner.SetActive( _isActive );
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( ChangeActiveOfGameObject )} : {_isActive}\n{_target}" );
#endif
		}

		async UniTask SequentialRun( SMObject smObject, bool isActive ) {
			if ( !smObject._owner.activeSelf )	{ return; }
			foreach ( var b in smObject.GetBehaviours() )
													{ await ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ); }
			foreach ( var o in smObject.GetChildren() )	{ await SequentialRun( o, isActive ); }
		}

		async UniTask ReverseRun( SMObject smObject, bool isActive ) {
			if ( !smObject._owner.activeSelf )	{ return; }
			foreach ( var o in smObject.GetChildren().Reverse() )	{ await ReverseRun( o, isActive ); }
			foreach ( var b in smObject.GetBehaviours().Reverse() )
													{ await ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ); }
		}

		async UniTask ParallelRun( SMObject smObject, bool isActive ) {
			if ( !smObject._owner.activeSelf )	{ return; }
			await Enumerable.Empty<UniTask>()
				.Concat(
					smObject.GetBehaviours().Select( b => ChangeActiveSMBehaviour.RegisterAndRun( b, isActive ) ) )
				.Concat( smObject.GetChildren().Select( o => ParallelRun( o, isActive ) ) );
		}



		public static bool IsCanChange( SMObject smObject, bool isChangeOwner ) {
			if ( smObject._parent != null && !smObject._parent._owner.activeInHierarchy )	{ return false; }
			if ( !isChangeOwner && !smObject._owner.activeSelf )							{ return false; }
			return true;
		}
	}
}