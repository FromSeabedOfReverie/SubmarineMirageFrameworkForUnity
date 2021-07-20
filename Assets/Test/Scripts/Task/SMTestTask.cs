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
	using Utility;
	using Debug;



	public class SMTestTask : SMTask {
		[SMShowLine] public override SMTaskRunType _type => _internalType;
		SMTaskRunType _internalType			{ get; set; }
		[SMShowLine] public string _name	{ get; private set; }
		[SMShow] bool _isSetEvents			{ get; set; }



		public SMTestTask( string name, SMTaskRunType type, bool isRegister, bool isAdjustRun, bool isSetEvents )
			// _internalType未設定のまま、基底コンストラクタで登録すると、Dontタスクになる為、登録しない
			: base( false, false )
		{
			_name = name;
			_internalType = type;
			_isSetEvents = isSetEvents;

			// 基底コンストラクタで未登録分を、派生先コンストラクタの下記で、改めて登録
			Register( isRegister, isAdjustRun );
#if TestTask
			SMLog.Debug( $"{nameof( SMTestTask )}() : \n{this}" );
#endif
		}

		public override void Create() {
			if ( !_isSetEvents )	{ return; }

			_selfInitializeEvent.AddLast( async c => {
				SMLog.Debug( $"{nameof( _selfInitializeEvent )} : start\n{ToLineString()}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{nameof( _selfInitializeEvent )} : end\n{ToLineString()}" );
			} );
			_initializeEvent.AddLast( async c => {
				SMLog.Debug( $"{nameof( _initializeEvent )} : start\n{ToLineString()}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{nameof( _initializeEvent )} : end\n{ToLineString()}" );
			} );
			_finalizeEvent.AddLast( async c => {
				SMLog.Debug( $"{nameof( _finalizeEvent )} : start\n{ToLineString()}" );
				await UTask.Delay( c, 1000 );
				SMLog.Debug( $"{nameof( _finalizeEvent )} : end\n{ToLineString()}" );
			} );

			_enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{nameof( _enableEvent )} : \n{ToLineString()}" );
			} );
			_disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{nameof( _disableEvent )} : \n{ToLineString()}" );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{nameof( _fixedUpdateEvent )} : \n{ToLineString()}" );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{nameof( _updateEvent )} : \n{ToLineString()}" );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{nameof( _lateUpdateEvent )} : \n{ToLineString()}" );
			} );
		}
	}
}