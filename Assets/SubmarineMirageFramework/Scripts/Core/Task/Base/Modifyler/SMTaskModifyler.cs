//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using SubmarineMirage.Base;
	using SubmarineMirage.Modifyler.Base;
	using Service;
	using Scene;



	// TODO : コメント追加、整頓



	public abstract class SMTaskModifyler<TTarget, TModifyler, TData> : SMStandardBase
		where TTarget : ISMTaskModifyTarget
		where TData : ISMModifyData
	{
		public ReactiveProperty<bool> _isSceneUpdating	{ get; set; }




		public SMTaskModifyler( IBaseSMModifyTarget target ) {
			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			SetupSceneUpdating( sceneManager?._body._isUpdating );
		}

		public void SetupSceneUpdating( ReactiveProperty<bool> isSceneUpdating ) {
			if ( isSceneUpdating == null )	{ return; }

			_isSceneUpdating = isSceneUpdating;
			_disposables.AddLast(
				_isSceneUpdating
					.Where( b => !b )
					.Subscribe( _ => Run().Forget() )
			);
		}


		public async UniTask Run() {
			while ( IsHaveData() ) {
				if ( _isSceneUpdating.Value )	{ break; }
			}
		}
	}
}