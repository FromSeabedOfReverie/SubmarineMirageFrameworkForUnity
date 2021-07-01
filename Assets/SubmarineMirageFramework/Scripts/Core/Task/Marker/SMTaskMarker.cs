//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Marker {
	using Cysharp.Threading.Tasks;



	public class SMTaskMarker : SMTask {
		[SMShowLine] public override SMTaskRunType _type => _internalType;
		SMTaskRunType _internalType;
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
			if ( _taskManager != null ) {
				_taskManager.Register( this, false ).Forget();
			}
		}

		public override void Create() {
		}
	}
}