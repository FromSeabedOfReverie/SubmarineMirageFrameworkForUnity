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
	using Task;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyler : SMStandardTest {
		Text _text	{ get; set; }
		ISMBehaviour _behaviour	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
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
					+ $"    {nameof( b._body._isInitialActive )} : {b._body._isInitialActive}\n"
					+ $")\n";
			}) );
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