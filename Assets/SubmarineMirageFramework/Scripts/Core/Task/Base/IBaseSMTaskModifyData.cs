//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskModifyler
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Base;



	// TODO : コメント追加、整頓



	public interface IBaseSMTaskModifyData<TOwner, TModifyler, TTarget> : ISMLightBase
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
		where TTarget : IBaseSMTaskModifyDataTarget
	{
		TTarget _target	{ get; }
		SMTaskModifyType _type	{ get; }

		void Set( TOwner owner );
		UniTask Run();
	}
}