//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;



	// TODO : コメント追加、整頓



	public interface IBaseSMTaskModifyData<TOwner, TModifyler> : ISMLightBase
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
	{
		SMTaskModifyType _type	{ get; }

		void Set( TOwner owner );
		UniTask Run();
	}
}