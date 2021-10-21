//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Test {
	using UniRx;



	public class SMTestTask : SMTask {
		[SMShowLine] public override SMTaskRunType _type	=> _internalType;
		readonly SMTaskRunType _internalType;
		[SMShow] readonly SMTestTaskLogType _logType;

		[SMShowLine] public readonly string _name;



		public SMTestTask( string name, SMTaskRunType type, bool isRegister, bool isAdjustRun,
							SMTestTaskLogType logType
		// _internalType未設定のまま、基底コンストラクタで登録すると、Dontタスクになる為、登録しない
		) : base( false, false )
		{
			_name = name;
			_internalType = type;
			_logType = logType;

			// 基底コンストラクタで未登録分を、派生先コンストラクタの下記で、改めて登録
			Register( isRegister, isAdjustRun );
#if TestTask
			SMLog.Debug( $"{nameof( SMTestTask )}() : \n{this}" );
#endif
		}

		public override void Create() {
			if ( _logType == SMTestTaskLogType.None )	{ return; }


			SMLog.Debug( $"{this.GetName()}.{nameof( Create )} : \n{this}" );

			_selfInitializeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _selfInitializeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetName()}.{nameof( _selfInitializeEvent )} : end\n{this}" );
			} );
			_initializeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _initializeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetName()}.{nameof( _initializeEvent )} : end\n{this}" );
			} );
			_finalizeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _finalizeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetName()}.{nameof( _finalizeEvent )} : end\n{this}" );
			} );

			_enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _enableEvent )} : \n{this}" );
			} );
			_disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _disableEvent )} : \n{this}" );
			} );

			if ( _logType == SMTestTaskLogType.Setup )	{ return; }


			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _fixedUpdateEvent )} : \n{this}" );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _updateEvent )} : \n{this}" );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetName()}.{nameof( _lateUpdateEvent )} : \n{this}" );
			} );
		}
	}
}