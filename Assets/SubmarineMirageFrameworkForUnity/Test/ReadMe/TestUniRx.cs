//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.ReadMe {

    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;
    using UniRx.Triggers;
    // シーン配置済の管理処理のシングルトン
    public class TestUniRxManager : MonoBehaviour {
        // 自身のインスタンス
        public static TestUniRxManager s_instance { get; private set; }
        // 試験データの一覧
        public List<string> _data { get; private set; } = new List<string>();
        // 初期化
        void Start() {
            // 自身を保持し、自身のデータを設定し、他者のデータ登録を待機後、データを加工
            s_instance = this;
            _data.Add( "ManagerData" );
            var isInitialized = false;
            Observable.TimerFrame( 2 ).Subscribe( _ => {    // ※待機時間がいい加減
                _data = _data.Select( d => d + "Processed" ).ToList();
                isInitialized = true;
            } )
            .AddTo( gameObject );
            // 更新処理を登録し、初期化済の場合のみ、何らかの処理
            this.UpdateAsObservable().Where( _ => isInitialized ).Subscribe( _ => {
            } );
        }
    }
    // シーン配置済のゲームオブジェクト1
    public class UseUniRx1 : MonoBehaviour {
        // 管理クラスのデータ
        string _managerData;
        // 他者のデータ
        string _data;
        // 初期化
        void Start() {
            // シングルトン作成を待機後、自身のデータを登録し、他者のデータ登録と加工を待機後、データを設定
            var isInitialized = false;
            Observable.NextFrame().Subscribe( _ => {    // ※待機がいい加減で、複雑な入れ子になっている
                TestUniRxManager.s_instance._data.Add( "Data1" );
                Observable.TimerFrame( 2 ).Subscribe( __ => {
                    _managerData = TestUniRxManager.s_instance._data[0];
                    _data = TestUniRxManager.s_instance._data[2];
                    isInitialized = true;
                } )
                .AddTo( gameObject );
            } )
            .AddTo( gameObject );
            // 更新処理を登録し、初期化済の場合のみ、何らかの処理
            this.UpdateAsObservable().Where( _ => isInitialized ).Subscribe( _ => {
            } );
        }
    }
    // シーン配置済のゲームオブジェクト2
    public class UseUniRx2 : MonoBehaviour {
        // 管理クラスのデータ
        string _managerData;
        // 他者のデータ
        string _data;
        // 初期化
        void Start() {
            // シングルトン作成を待機後、自身のデータを登録し、他者のデータ登録と加工を待機後、データを設定
            var isInitialized = false;
            Observable.NextFrame().Subscribe( _ => {    // ※待機がいい加減で、複雑な入れ子になっている
                TestUniRxManager.s_instance._data.Add( "Data2" );
                Observable.TimerFrame( 2 ).Subscribe( __ => {
                    _managerData = TestUniRxManager.s_instance._data[0];
                    _data = TestUniRxManager.s_instance._data[1];
                    isInitialized = true;
                } )
                .AddTo( gameObject );
            } )
            .AddTo( gameObject );
            // 更新処理を登録し、初期化済の場合のみ、何らかの処理
            this.UpdateAsObservable().Where( _ => isInitialized ).Subscribe( _ => {
            } );
        }
    }

}