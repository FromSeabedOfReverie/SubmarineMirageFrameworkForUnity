//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler {
	using Modifyler.Base;



	// TODO : コメント追加、整頓



	public interface ISMModifyTarget<TTarget, TData> : IBaseSMModifyTarget
		where TTarget : IBaseSMModifyTarget
		where TData : ISMModifyData
	{
		SMModifyler<TTarget, TData> _modifyler	{ get; }
	}
}