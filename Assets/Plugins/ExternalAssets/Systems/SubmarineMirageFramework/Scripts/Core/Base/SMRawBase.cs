//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UniRx;



	public abstract class SMRawBase : BaseSM, ISMRawBase {
		public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		[SMShowLine] public bool _isDispose => _disposables.IsDisposed;



		public virtual string AddToString( int indent )
			=> string.Empty;



		public override void Dispose() => _disposables.Dispose();



		protected void CheckDisposeError( string name ) {
			if ( !_isDispose )	{ return; }

			throw new ObjectDisposedException(
				$"{this}", $"既に破棄済\n{this.GetAboutName()}.{name}" );
		}
	}
}