//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestFSM
namespace SubmarineMirage {



	public abstract class SMState<TOwner, TState> : BaseSMState
		where TOwner : class
		where TState : BaseSMState
	{
		[SMShow] protected TOwner _owner		{ get; private set; }
		[SMShow] protected SMFSM<TState> _fsm	{ get; private set; }

		protected SMAsyncCanceler _asyncCancelerOnExit => _fsm?._asyncCancelerOnExit;



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _owner ), i => _toStringer.DefaultLineValue( _owner ) );
			_toStringer.SetValue( nameof( _fsm ), i => _toStringer.DefaultLineValue( _fsm ) );
		}
#endregion



		public SMState() {
			_disposables.AddFirst( () => {
				_owner = null;
				_fsm = null;
			} );

#if TestFSM
			_disposables.AddFirst( () => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( Dispose )} : start\n{this}" );
			} );
			_disposables.AddLast( () => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( Dispose )} : end\n{this}" );
			} );
			SMLog.Debug( $"{this.GetAboutName()}() : \n{this}" );
#endif
		}

		public override void Dispose()
			=> base.Dispose();

		public override void Setup( object owner, object fsm ) {
			CheckDisposeError( nameof( Setup ) );

			_owner = owner as TOwner;
			_fsm = fsm as SMFSM<TState>;
		}
	}
}