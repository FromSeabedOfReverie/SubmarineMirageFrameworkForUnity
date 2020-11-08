//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Modifyler;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObjectGroup : IDisposableExtension {
		static uint s_idCount;
		public uint _id			{ get; private set; }

		public SMTaskType _type;
		public SMTaskLifeSpan _lifeSpan;
		public BaseScene _scene;
		public SMObjectManager _objects => _scene?._objects;
		public SMObjectModifyler _modifyler	{ get; private set; }

		public SMObjectGroup _previous;
		public SMObjectGroup _next;

		public SMObject _topObject;
		public readonly LinkedList<SMObject> _addObjects = new LinkedList<SMObject>();
		public readonly LinkedList<SMObject> _removeObjects = new LinkedList<SMObject>();

		public bool _isDispose =>	_disposables._isDispose;
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObjectGroup( SMObject top ) {
			_id = ++s_idCount;
			_topObject = top;

			_modifyler = new SMObjectModifyler( this );

			_disposables.AddLast( _modifyler );
			_disposables.AddLast( () => {
				_topObject.Dispose();
				_addObjects.ForEach( o => o.Dispose() );
				_removeObjects.ForEach( o => o.Dispose() );
// TODO : _previous、_next、Managerへのリンク変更を行う
			} );
		}

		~SMObjectGroup() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public bool IsTop( SMObject smObject ) => smObject == _topObject;



		public SMObjectGroup GetFirst() {
			SMObjectGroup current = null;
			for ( current = this; current._previous != null; current = current._previous )	{}
			return current;
		}
		public SMObjectGroup GetLast() {
			SMObjectGroup current = null;
			for ( current = this; current._next != null; current = current._next )	{}
			return current;
		}

		public IEnumerable<SMObjectGroup> GetBrothers() {
			for ( var current = GetFirst(); current != null; current = current._next )	{
				yield return current;
			}
		}



		public async UniTask RunStateEvent( SMTaskRunState state ) {
			RunStateSMObject.RunOrRegister( _topObject, state );
			await _modifyler.WaitRunning();
		}

		public async UniTask ChangeActive( bool isActive ) {
			_modifyler.Register( new ChangeActiveSMObject( _topObject, isActive, true ) );
			await _modifyler.WaitRunning();
		}

		public async UniTask RunInitialActive() {
			_modifyler.Register( new RunActiveSMObject( _topObject ) );
			await _modifyler.WaitRunning();
		}



		public override string ToString() => string.Join( "\n",
			$"{nameof( SMObjectGroup )}(",
			$"    {nameof( _id )} : {_id}",
			$"    {nameof( _type )} : {_type}",
			$"    {nameof( _lifeSpan )} : {_lifeSpan}",
			$"    {nameof( _scene )} : {_scene}",
			$"    {nameof( _modifyler )} : {_modifyler}",
			
			$"    {nameof( _previous )} : {_previous?.ToLineString()}",
			$"    {nameof( _next )} : {_next?.ToLineString()}",

			$"    {nameof( _topObject )} : {_topObject.ToLineString()}",
			$"    {nameof( _addObjects )} : ",
			string.Join( "\n", _addObjects.Select( o => $"        {o.ToLineString()}" ) ),
			$"    {nameof( _removeObjects )} : ",
			string.Join( "\n", _removeObjects.Select( o => $"        {o.ToLineString()}" ) ),

			$"    {nameof( _isDispose )} : {_isDispose}",
			")"
		);

		public string ToLineString( bool isViewLink = true ) {
			var result = string.Join( " ",
				_id,
				nameof( SMObjectGroup ),
				""
			);
			if ( _isDispose ) {
				result += "Dispose ";
			}
			if ( isViewLink ) {
				result += string.Join( " ",
					$"↑{_previous?._id}",
					$"↓{_next?._id}",
					$"△{_topObject._id}",
					$"＋{string.Join( ",", _addObjects.Select( o => o._id ) )}",
					$"－{string.Join( ",", _removeObjects.Select( o => o._id ) )}"
				);
			}
			return result;
		}

	}
}