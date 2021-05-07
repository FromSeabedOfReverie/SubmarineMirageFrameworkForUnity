//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using UniRx;
	using Debug;


	public abstract class SMRawBase : BaseSM, ISMRawBase {
		public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		[SMShowLine] public bool _isDispose => _disposables.IsDisposed;

		public override void Dispose() => _disposables.Dispose();
	}
}