//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using KoganeUnityLib;
	using Task.Behaviour;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyler : SMStandardTest {
		Text _text	{ get; set; }
		ISMBehaviour _behaviour	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;

			Resources.Load<GameObject>( "TestCamera" ).Instantiate();
			var go = Resources.Load<GameObject>( "TestCanvas" ).Instantiate();
			go.DontDestroyOnLoad();
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
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
					+ $"    {nameof( b._body._isRunInitialActive )} : {b._body._isRunInitialActive}\n"
					+ $")\n";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					default:	break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}
	}
}