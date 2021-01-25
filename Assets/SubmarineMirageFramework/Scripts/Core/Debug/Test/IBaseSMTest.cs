//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using Cysharp.Threading.Tasks;
	using Base;
	using Service;



	// TODO : コメント追加、整頓



	public interface IBaseSMTest : IBaseSM, ISMService {
		UniTask AwakeTop();
	}
}