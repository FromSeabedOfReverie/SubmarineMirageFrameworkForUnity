//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.ReadMe {

    using System.Linq;
    using System.Collections.Generic;
    using UniRx;
    using Cysharp.Threading.Tasks;
    using SubmarineMirage.Singleton;
    using SubmarineMirage.Process;
    // MonoBehaviourProcessと同じようにProcessサイクルを持つ、シンプルなシングルトン
    public class TestSingleton : Singleton<TestSingleton> {
        // 試験データの一覧
        public readonly List<string> _data = new List<string>();
        // コンストラクタで、読込時にデータ読込を設定
        public TestSingleton() {
            _loadEvent += async () => {
                _data.Add( "TestData" );
                await UniTask.Delay( 0 );
            };
        }
    }
    // シングルトンを呼ぶ為の、コンポーネントクラス
    public class UseSingleton : MonoBehaviourProcess {
        // このコンポーネントが使用する試験データ
        string _data;
        // コンストラクタで、初期化時に管理クラスからデータ取得、を設定
        protected override void Constructor() {
            // 予めシングルトンを使用し生成しないと、内部登録されず、読込処理が行われない
            var i = TestSingleton.s_instance;
            _initializeEvent += async () => {
                _data = TestSingleton.s_instance._data.FirstOrDefault();
                await UniTask.Delay( 0 );
            };
        }
    }

}