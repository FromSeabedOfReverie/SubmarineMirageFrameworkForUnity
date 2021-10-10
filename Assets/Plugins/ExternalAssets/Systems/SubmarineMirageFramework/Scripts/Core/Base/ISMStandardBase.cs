//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {


	public interface ISMStandardBase : IBaseSM, IDisposableSMExtension {
		SMDisposable _disposables	{ get; }
		SMToStringer _toStringer	{ get; }

		void SetToString();
	}
}