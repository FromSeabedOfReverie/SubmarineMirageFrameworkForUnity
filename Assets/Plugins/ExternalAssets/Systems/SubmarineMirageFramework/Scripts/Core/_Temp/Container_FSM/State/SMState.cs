//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestFSM
namespace SubmarineMirageFramework {
	using System;



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
				SMLog.Debug( $"{this.GetName()}.{nameof( Dispose )} : start\n{this}", SMLogTag.FSM );
			} );
			_disposables.AddLast( () => {
				SMLog.Debug( $"{this.GetName()}.{nameof( Dispose )} : end\n{this}", SMLogTag.FSM );
			} );
			SMLog.Debug( $"{this.GetName()}() : \n{this}", SMLogTag.FSM );
#endif
		}


		public override void Dispose()
			=> base.Dispose();


		public override void Initialize( object owner, object fsm ) {
			CheckDisposeError( nameof( Initialize ) );
			if ( _owner != null ) {
				throw new InvalidOperationException( $"{this.GetName()}.{nameof( Initialize )} : 既に実行済\n{this}" );
			}

			_owner = owner as TOwner;
			_fsm = fsm as SMFSM<TState>;

			if ( _owner == null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"{this.GetName()}.{nameof( Initialize )} : {nameof( _owner )}設定失敗",
					$"{nameof( owner )} : {owner}",
					$"{this}"
				) );
			}
			if ( _fsm == null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"{this.GetName()}.{nameof( Initialize )} : {nameof( _fsm )}設定失敗",
					$"{nameof( fsm )} : {fsm}",
					$"{this}"
				) );
			}

			_isInitialized = true;
		}
	}
}