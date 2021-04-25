//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using SubmarineMirage.Modifyler;
	using Service;
	using Scene;



	// TODO : コメント追加、整頓



	public abstract class SMTask : SMStandardBase, ISMModifyTarget {
		public SMTaskRunState _ranState { get; set; }
		public SMTaskActiveState _activeState { get; set; }
		public bool _isInitialized => _ranState >= SMTaskRunState.InitialEnable;
		public bool _isOperable => _isInitialized && !_isFinalizing;
		public bool _isFinalizing { get; set; }
		public bool _isActive => _activeState == SMTaskActiveState.Enable;

		[SMShow] protected abstract Type _baseModifyDataType { get; }
		public SMModifyler _modifyler { get; protected set; }
		public ReactiveProperty<bool> _isSceneUpdating { get; set; }



		public SMTask() {
			_modifyler = new SMModifyler( this, _baseModifyDataType );
			_disposables.AddLast( _modifyler );

			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			SetupSceneUpdating( sceneManager?._body._isUpdating );
		}


		public void SetupSceneUpdating( ReactiveProperty<bool> isSceneUpdating ) {
			if ( isSceneUpdating == null ) { return; }


			_modifyler._isCanRunEvent = () => !_isSceneUpdating.Value;

			_isSceneUpdating = isSceneUpdating;
			_disposables.AddLast(
				_isSceneUpdating
					.Where( b => !b )
					.Subscribe( _ => _modifyler.Run().Forget() )
			);
		}
	}
}