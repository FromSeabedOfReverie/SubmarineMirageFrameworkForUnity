//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroup
namespace SubmarineMirage.Task.Group {
	using System.Collections.Generic;
	using UnityEngine;
	using Modifyler;
	using Task.Modifyler;
	using Object;
	using Group.Manager;
	using Scene;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMGroup : BaseSMTaskModifylerOwner<SMGroupModifyler> {
		public SMTaskLifeSpan _lifeSpan	{ get; set; }
		public SMScene _scene		{ get; set; }
		[SMHide] public SMGroupManager _groups => _scene?._groups;

		[SMShowLine] public SMGroup _previous	{ get; set; }
		[SMShowLine] public SMGroup _next		{ get; set; }

		[SMShowLine] public SMObject _topObject	{ get; set; }
		[SMHide] public GameObject _gameObject => _topObject._gameObject;
		[SMHide] public bool _isGameObject => _gameObject != null;

		[SMHide] public SMTaskCanceler _asyncCanceler => _topObject._asyncCanceler;


		public SMGroup( SMObject top ) {
			_modifyler = new SMGroupModifyler( this );
			_topObject = top;

			SMGroupApplyer.SetAllData( this );

			_disposables.AddLast( () => {
				_isFinalizing = true;
				_ranState = SMTaskRunState.Finalize;
			} );
		}

		public override void Dispose() => base.Dispose();



		public SMGroup GetFirst() {
			SMGroup current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMGroup GetLast() {
			SMGroup current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMGroup> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}



		public bool IsTop( SMObject smObject ) => smObject == _topObject;



		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _previous ), i => _previous?.ToLineString() );
			_toStringer.SetValue( nameof( _next ), i => _next?.ToLineString() );
			_toStringer.SetValue( nameof( _topObject ), i => _topObject.ToLineString() );

			_toStringer.SetLineValue( nameof( _isDispose ), () => _isDispose ? nameof( Dispose ) : "" );
			_toStringer.SetLineValue( nameof( _previous ), () => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ), () => $"↓{_next?._id}" );
			_toStringer.SetLineValue( nameof( _topObject ), () => $"△{_topObject._id}" );
		}
	}
}