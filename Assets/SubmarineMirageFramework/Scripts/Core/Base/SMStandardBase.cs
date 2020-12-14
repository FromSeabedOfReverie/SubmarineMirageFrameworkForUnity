//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using MultiEvent;
	using Debug;
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMStandardBase : SMBase, ISMStandardBase {
		[Hide] public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();
		[ShowLine] public bool _isDispose => _disposables._isDispose;
		[Hide] public SMToStringer _toStringer	{ get; private set; }


		public SMStandardBase() {
			_toStringer = new SMToStringer( this );
			SetToString();
		}

		public override void Dispose() => _disposables.Dispose();

		public virtual void SetToString()	{}
		public override string ToString( int indent ) => _toStringer.Run( indent );
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}