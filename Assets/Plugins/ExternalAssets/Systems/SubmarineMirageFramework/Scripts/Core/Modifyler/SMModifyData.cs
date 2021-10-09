//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestModifyData
namespace SubmarineMirage.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Base;
	using Debug;



	public class SMModifyData : SMLightBase {
		[SMShowLine] string _name				{ get; set; }
		[SMShowLine] public SMModifyType _type	{ get; private set; }

		[SMShow] bool _isCalledDestructor	{ get; set; }
		[SMShow] public bool _isFinished	{ get; private set; }

		Func<UniTask> _runEvent	{ get; set; }
		Action _cancelEvent		{ get; set; }



		public SMModifyData( string name, SMModifyType type, Func<UniTask> runEvent, Action cancelEvent ) {
			_name = name;
			_type = type;
			_runEvent = runEvent;
			_cancelEvent = cancelEvent;

#if TestModifyData
			SMLog.Debug( $"{nameof( SMModifyData )}() : \n{this}" );
#endif
		}

		~SMModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
#if TestModifyData
			SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
			Cancel();
			Finish();

			_runEvent = null;
			_cancelEvent = null;

#if TestModifyData
			SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
		}



		public UniTask Run()
			=> _runEvent.Invoke();



		void Cancel() {
			if ( _isCalledDestructor )	{ return; }
			if ( _cancelEvent == null )	{ return; }

#if TestModifyData
			SMLog.Debug( $"{nameof( Cancel )} : \n{this}" );
#endif
			_cancelEvent.Invoke();
			_cancelEvent = null;
		}



		public void Finish() {
			if ( _isFinished )	{ return; }

			_isFinished = true;

#if TestModifyData
			SMLog.Debug( $"{nameof( Finish )} : \n{this}" );
#endif
		}
	}
}