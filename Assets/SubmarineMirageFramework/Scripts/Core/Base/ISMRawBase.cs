//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using UniRx;
	using Extension;


	// TODO : コメント追加、整頓


	public interface ISMRawBase : IBaseSM, IDisposableSMExtension {
		CompositeDisposable _disposables	{ get; }
	}
}