//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler {
	using SubmarineMirage.Base;
	using Modifyler.Base;



	// TODO : コメント追加、整頓



	public abstract class SMModifyTarget<TTarget, TData> : SMStandardBase, ISMModifyTarget<TTarget, TData>
		where TTarget : IBaseSMModifyTarget
		where TData : ISMModifyData
	{
		public SMModifyler<TTarget, TData> _modifyler	{ get; private set; }


		public SMModifyTarget() {
			_modifyler = new SMModifyler<TTarget, TData>( this );

			_disposables.AddLast( () => {
				_modifyler.Dispose();
			} );
		}
	}
}