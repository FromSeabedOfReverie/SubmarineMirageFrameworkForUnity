//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler.Base {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;



	// TODO : コメント追加、整頓



	public interface ISMModifyData : ISMLightBase {
		SMModifyType _type	{ get; }


		void Set( IBaseSMModifyTarget target, ISMModifyler modifyler );
		UniTask Run();
	}
}