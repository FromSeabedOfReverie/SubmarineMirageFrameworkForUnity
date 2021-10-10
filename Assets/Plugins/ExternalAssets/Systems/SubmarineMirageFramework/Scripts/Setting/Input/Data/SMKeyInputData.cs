//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ キー入力の情報クラス
	/// </summary>
	///====================================================================================================
	public class SMKeyInputData : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>間引き開始秒数</summary>
		const float START_THIN_OUT_SECONDS = 23 / 60f;
		/// <summary>間引き繰り返し秒数</summary>
		const float REPEAT_THIN_OUT_SECONDS = 6 / 60f;

		/// <summary>入力の型</summary>
		[SMShowLine] public readonly SMInputKey _type;
		/// <summary>時間の管理者</summary>
		SMTimeManager _timeManager { get; set; }

		/// <summary>有効秒数</summary>
		[SMShowLine] float _enableSeconds		{ get; set; }
		/// <summary>次回の繰り返し秒数</summary>
		[SMShowLine] float _nextThinOutSeconds	{ get; set; } = float.PositiveInfinity;

		/// <summary>有効中か？</summary>
		[SMShow] public bool _isEnabling		{ get; private set; }
		/// <summary>有効時か？</summary>
		[SMShow] public bool _isEnabled			{ get; private set; }
		/// <summary>間引き有効中か？</summary>
		[SMShow] public bool _isThinOutEnabling	{ get; private set; }
		/// <summary>無効時か？</summary>
		[SMShow] public bool _isDisabled		{ get; private set; }

		/// <summary>有効か？判定イベント</summary>
		Func<bool> _isCheckEnableEvent { get; set; }

		/// <summary>有効中イベント</summary>
		public readonly SMSubject _enablingEvent = new SMSubject();
		/// <summary>有効時イベント</summary>
		public readonly SMSubject _enabledEvent = new SMSubject();
		/// <summary>間引き有効中イベント</summary>
		public readonly SMSubject _thinOutEnablingEvent = new SMSubject();
		/// <summary>無効時イベント</summary>
		public readonly SMSubject _disabledEvent = new SMSubject();
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMKeyInputData( SMInputKey type, Func<bool> isCheckEnableEvent ) {
			_type = type;
			_isCheckEnableEvent = isCheckEnableEvent;
			_timeManager = SMServiceLocator.Resolve<SMTimeManager>();


			_disposables.AddFirst( () => {
				_enableSeconds = 0;
				_nextThinOutSeconds = 0;

				_isEnabling = false;
				_isEnabled = false;
				_isThinOutEnabling = false;
				_isDisabled = false;

				_isCheckEnableEvent = null;

				_enablingEvent.Dispose();
				_enabledEvent.Dispose();
				_thinOutEnablingEvent.Dispose();
				_disabledEvent.Dispose();

				_timeManager = null;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Update() {
			var isLastEnable = _isEnabling;
			_isEnabling = _isCheckEnableEvent();

			if ( _isEnabling ) {
				// 押上直後の補正
				if ( _enableSeconds < 0 )	{ _enableSeconds = 0; }
				_enableSeconds += _timeManager._unscaledDeltaTime;
				// オーバーフロー対策
				if ( _enableSeconds < 0 )	{ _enableSeconds = float.PositiveInfinity; }
				if ( !isLastEnable )		{ _nextThinOutSeconds = 0; }

			} else if ( isLastEnable ) {
				_enableSeconds = -1;
				_nextThinOutSeconds = float.PositiveInfinity;

			} else {
				_enableSeconds = 0;
				_nextThinOutSeconds = float.PositiveInfinity;
			}

			_isEnabling = _enableSeconds > 0;
			_isEnabled = _enableSeconds == _timeManager._unscaledDeltaTime && _timeManager._unscaledDeltaTime != 0;
			_isDisabled = _enableSeconds < 0;

			_isThinOutEnabling = _nextThinOutSeconds < _timeManager._unscaledTime;
			if ( _isThinOutEnabling ) {
				if ( _nextThinOutSeconds == 0 ) {
					_nextThinOutSeconds = START_THIN_OUT_SECONDS + _timeManager._unscaledTime;
				} else if ( _nextThinOutSeconds > 0 ) {
					_nextThinOutSeconds = REPEAT_THIN_OUT_SECONDS + _timeManager._unscaledTime;
				}
			}

			if ( _isEnabling )			{ _enablingEvent.Run(); }
			if ( _isEnabled )			{ _enabledEvent.Run(); }
			if ( _isThinOutEnabling )	{ _thinOutEnablingEvent.Run(); }
			if ( _isDisabled )			{ _disabledEvent.Run(); }
		}
	}
}