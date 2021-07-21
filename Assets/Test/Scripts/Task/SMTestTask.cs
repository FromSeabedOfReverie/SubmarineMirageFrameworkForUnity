//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.TestTask {
	using UniRx;
	using Task;
	using Extension;
	using Utility;
	using Debug;



	public class SMTestTask : SMTask {
		[SMShowLine] public override SMTaskRunType _type	=> _internalType;
		SMTaskRunType _internalType							{ get; set; }

		[SMShowLine] public string _name	{ get; private set; }

		bool _isSetEvents		{ get; set; }
		bool _isSetUpdateEvents	{ get; set; }



		public SMTestTask( string name, SMTaskRunType type, bool isRegister, bool isAdjustRun,
							bool isSetEvents, bool isSetUpdateEvents
		// _internalType未設定のまま、基底コンストラクタで登録すると、Dontタスクになる為、登録しない
		) : base( false, false )
		{
			_name = name;
			_internalType = type;
			_isSetEvents = isSetEvents;
			_isSetUpdateEvents = isSetUpdateEvents;

			// 基底コンストラクタで未登録分を、派生先コンストラクタの下記で、改めて登録
			Register( isRegister, isAdjustRun );
#if TestTask
			SMLog.Debug( $"{nameof( SMTestTask )}() : \n{this}" );
#endif
		}

		public override void Create() {
			if ( !_isSetEvents )	{ return; }

			SMLog.Debug( $"{this.GetAboutName()}.{nameof( Create )} : \n{this}" );

			_selfInitializeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _selfInitializeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _selfInitializeEvent )} : end\n{this}" );
			} );
			_initializeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _initializeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _initializeEvent )} : end\n{this}" );
			} );
			_finalizeEvent.AddLast( async c => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _finalizeEvent )} : start\n{this}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _finalizeEvent )} : end\n{this}" );
			} );

			_enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _enableEvent )} : \n{this}" );
			} );
			_disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _disableEvent )} : \n{this}" );
			} );

			if ( !_isSetUpdateEvents )	{ return; }

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _fixedUpdateEvent )} : \n{this}" );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _updateEvent )} : \n{this}" );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{this.GetAboutName()}.{nameof( _lateUpdateEvent )} : \n{this}" );
			} );
		}
	}
}