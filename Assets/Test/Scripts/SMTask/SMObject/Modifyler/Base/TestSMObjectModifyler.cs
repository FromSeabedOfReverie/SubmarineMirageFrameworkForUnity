//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask.Modifyler {
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using UTask;
	using SMTask;
	using Extension;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyler : SMStandardTest {
		Text _text;
		ISMBehaviour _behaviour;


		protected override void Create() {
			Application.targetFrameRate = 30;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast(Observable.EveryLateUpdate().Subscribe( (System.Action<long>)(_ => {
				if (_behaviour._object == null ) {
					_text.text = string.Empty;
					return;
				}
				var b = _behaviour._object._behaviour;
				_text.text =
					$"{_behaviour._object}\n"
					+ $"{b.GetAboutName()}(\n"
					+ $"    {nameof( b._isInitialized )} : {b._isInitialized}\n"
					+ $"    {nameof( b._isActive )} : {b._isActive}\n"
					+ $"    {nameof( b._body._ranState )} : {b._body._ranState}\n"
					+ $"    {nameof( b._body._isActive )} : {b._body._isActive}\n"
					+ $"    {nameof( b._body._isInitialActive )} : {b._body._isInitialActive}\n"
					+ $")\n";
			}) ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					default:	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}
	}
}