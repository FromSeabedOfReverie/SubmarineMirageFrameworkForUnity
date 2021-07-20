//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Task.Marker {
	using Debug;



	public class SMTaskMarker : SMTask {
		[SMShowLine] public override SMTaskRunType _type => _internalType;
		SMTaskRunType _internalType	{ get; set; }
		[SMShowLine] public SMTaskMarkerType _markerType { get; private set; }
		[SMShowLine] public string _name { get; private set; }



		public SMTaskMarker( string name, SMTaskRunType type, SMTaskMarkerType markerType )
			// _internalType未設定のまま、基底コンストラクタで登録すると、Dontタスクになる為、登録しない
			: base( false, false )
		{
			_name = name;
			_internalType = type;
			_markerType = markerType;

			// 基底コンストラクタで未登録分を、派生先コンストラクタの下記で、改めて登録
			Register( true, false );
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarker )}() : \n{this}" );
#endif
		}

		public override void Create() {
		}
	}
}