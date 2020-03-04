# Submarine Mirage Framework for Unity
##### Version 0.1
![Logo.png](/Assets/SubmarineMirageFrameworkForUnity/Textures/Logo.png)  



## 概要
Submarine Mirage Framework for Unityとは、Unityでのゲーム開発時に、不便さを補い、安定、迅速、堅牢な実装を行う為の、フレームワークである。  
販売品質のゲームで、一般的、汎用的、必須な機能の、全体実装を目標とする。  
しかし、現在開発中の為、
[未実装機能](/../../issues)
が沢山あり、不安定である。  


#### 対象
+ 中規模インディーゲーム向け  
数名規模のインディーゲーム開発スタジオで、それなりの中規模ゲーム開発での使用を想定している。  
（ストーリーがある中編アクションRPG、小規模のオープンワールドゲーム等。）  
また、プログラム実装に悩んだ際に、処理の参考となる。  

+ 日本人向け  
Unity用の総合機能フレームワークは、オープンソースで多々あるが、プログラムのコメントが中国語や英語である事が多い。  
マニュアルを日本語化して読む程度ならまだしも、外国語のプログラムのコメントを見ながらの開発は辛いと思われる。  
そのような語学が堪能でない日本人にとって、扱い易いフレームワークである。  
逆に、語学堪能な日本人、外国人には、
[GameFramework](https://github.com/EllanJiang/GameFramework)、
[QFramework](https://github.com/liangxiegame/QFramework)、
[BDFramework.Core](https://github.com/yimengfan/BDFramework.Core)、
[KSFramework](https://github.com/mr-kelly/KSFramework)
等のフレームワークの方が出来が良い為、そちらをお勧めする。  

+ 導入事例  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)
（インディーゲーム開発スタジオ）の開発環境に採用中である。  


#### 特徴
+ Unityのいい加減な機能を、補強  
Unityは、いい加減なゲーム開発をサポートしているが、販売品質程度のきちんと動作するゲーム開発を、ほぼ全くサポートしていない。  
Unityの問題ばかりの機能の内、ゲーム開発で頻繁に使われるであろう機能を、堅牢に、使い易く実装している。  

+ フレームワークとして、全面実装  
フレームワークの為、特定の機能実装だけでなく、多種多様な機能を全体的に実装している。  

+ 外部ライブラリ導入済の為、環境構築が簡単  
業界水準を鑑み、ゲーム開発企業で頻繁に使用される、多種多様な外部ライブラリを同封している。  
（ライセンス上、同封可能なライブラリのみ。）  
このフレームワークを導入するだけで、即、開発環境を構築可能である。  

+ 見易いプログラム  
可読性を意識し、（日本語であるが）見易いコメント、分かり易い設計を重視している。  
プログラムが見易い事は、使い易いフレームワークの必須条件と考えられる。  


#### 対応
+ Unity動作環境  
  + 2018.4.16f1～（以降）  
  動作確認は、2018.4.16f1を使用している。

+ ビルド対応  
  + Windows（Mono、IL2CPP）  
  + Android（Mono、IL2CPP）  



## 使用方法
#### 導入  
1. フレームワークを導入  
[Assets/](/Assets)
内の書類を全て複製、移植する。  

1. Unity内のPlayerSettingsを変更  
ScriptingRuntimeVersionを.NET4.xEquivalentに設定する。  
ApiCompatibilityLevelを.NET4.xに設定する。  

1. Unity内でパッケージを導入  
PackageManager、Service等から、UnityAds、UnityIAPを導入する。  


#### 使い方  
+ [MonoBehaviourProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)  
  順序立てた非同期処理を追加したMonoBehaviourである。  
  ```csharp
    using UniRx;
    using UniRx.Async;
    using SubmarineMirageFramework.Process;
    // MonoBehaviourProcessは、処理順序に規則を持たせたMonoBehaviour
    // UniRxで無理矢理組むような、リアクティブスパゲッティを防止できる
    public class TestMonoBehaviourProcess : MonoBehaviourProcess {
        // 疑似的コンストラクタ
        // 生成時に呼ばれるが、実際のコンストラクタは実行時以外にも呼ばれる為、これを使用
        protected override void Constructor() {
            // 読込処理を設定
            // 各種管理クラスの初期化後、最初に管理クラスから呼ばれる
            _loadEvent += async () => {
                // AssetBundle、ServerData等、自身で完結する非同期処理を記述
                await UniTask.Delay( 0 );
            };
            // 初期化処理を設定
            // _loadEvent実行後、管理クラスから呼ばれる
            _initializeEvent += async () => {
                // 他オブジェクトの要素、管理クラスの要素等、読込済の他者の取得処理を記述
                await UniTask.Delay( 0 );
            };
            // 更新処理を設定
            // _initializeEvent実行後、管理クラスから毎フレーム呼ばれる
            _updateEvent.Subscribe( _ => {
                // 毎フレーム実行する処理を記述
            } );
            // 終了処理を設定
            // 破棄直前に管理クラスから呼ばれる
            _finalizeEvent += async () => {
                // 破棄が必要な、非同期処理を記述
                await UniTask.Delay( 0 );
            };
        }
    }
  ```

+ [Singleton](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Singleton/Singleton.cs)  
  [MonoBehaviourProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)
  と同様に動作する、ゲームオブジェクト化する必要の無い、基本的なシングルトンである。  
  ```csharp
    using System.Linq;
    using System.Collections.Generic;
    using UniRx;
    using UniRx.Async;
    using SubmarineMirageFramework.Singleton;
    using SubmarineMirageFramework.Process;
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
  ```

+ [MainProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/Main/MainProcess.cs)  
  ゲーム起動直後に、
  [Singleton](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Singleton/Singleton.cs)
  の生成と処理順序を指定できる。  
  ```csharp
    using System.Linq;
    using UniRx.Async;
    using SubmarineMirageFramework.Process;
    // 実行時に、一番最初に処理されるクラス
    // Assets/SubmarineMirageFrameworkForUnity/Scripts/Main/MainProcess.csを、簡易的に記述している
    // 実際は、そこを編集する
    public class MainProcess {
        // 外部プラグインを初期化する
        static async UniTask InitializePlugin() {
            // ここで、様々な外部プラグインの初期化を記述する
            // ...省略...
            await UniTask.Delay( 0 );
        }
        // 処理を登録する
        static async UniTask RegisterProcesses() {
            // ここで、シングルトン等のProcess系クラスを初期化し、呼び出し順を確定させる
            // ...省略...
            await TestSingleton.WaitForCreation();    // シングルトン試験の生成と登録を行い、完了まで待機
            await UniTask.DelayFrame( 1 );
        }
    }
    // 登録済シングルトンを呼ぶ為の、コンポーネントクラス
    public class UseSingletonByRegister : MonoBehaviourProcess {
        // このコンポーネントが使用する試験データ
        string _data;
        // コンストラクタで、初期化時に管理クラスからデータ取得、を設定
        protected override void Constructor() {
            // ここでシングルトン未使用でも、生成済で内部登録済の為、読込処理が行われる
    //      var i = TestSingleton.s_instance;
            _initializeEvent += async () => {
                _data = TestSingleton.s_instance._data.FirstOrDefault();
                await UniTask.Delay( 0 );
            };
        }
    }
  ```



## 設計思想
UniRxは優れたライブラリだが、いい加減な使用で直ぐに複雑化し、リアクティブスパゲッティと呼ばれる難読プログラムとなる。  
下記プログラムは、管理クラスのシングルトン初期化後に、ゲームオブジェクトがデータ取得後に初期化する、難読プログラムの悪しき例である。  
```csharp
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
```
このような難読プログラムは、サーバー通信によるAssetBundleの配信、WebAPIデータの送受信等の非同期処理が含まれる場合に、多く遭遇する。  
原因は、Unityのコンポーネント設計が全て等価である事、Unityは非同期処理を考慮しない設計である事に、起因していると考えられる。  
その為、当フレームワークでは、下記の遷移図に示す、非同期ゲームループを中心に据えた設計とし、管理クラス等の処理順序を規定している。  
![Flowchart.png](/Flowchart.png)  
※完成予定図の為、現在未対応な機能も含まれる。  



## 実装処理
#### フレームワークの配置フォルダ
[Assets/SubmarineMirageFrameworkForUnity/](/Assets/SubmarineMirageFrameworkForUnity)
に、フレームワークが配置されている。  
以降は、このフォルダ直下の説明を行う。  

+ [/Test/](/Assets/SubmarineMirageFrameworkForUnity/Test)  
機能試験用の書類が存在する。  
当項目では、このフォルダ直下の説明を行う。  

  + [/ReadMe/](/Assets/SubmarineMirageFrameworkForUnity/Test/ReadMe)  
  当書類
  [README](/README.md)
  に記載のサンプルプログラムを纏めている。  

  + [/Sample/](/Assets/SubmarineMirageFrameworkForUnity/Test/Sample)  
  使用例の書類が纏められている。  
  [Sample.unity](/Assets/SubmarineMirageFrameworkForUnity/Test/Sample/Sample.unity)
  シーンにて、使用例を確認できる。  
  [Singleton](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Singleton)、
  [MonoBehaviourProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)、
  [FSM/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/FSM)、
  [Audio/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Audio)
  等の使用例を示している。  
  1対1の3Dシューティングゲームで、戦闘時に音楽を切り替え、キャラクターの死亡時に（ゲーム成功、失敗別の）ジングル音を再生する。  
  実行中にWASDキーでプレイヤーの移動、マウス移動でプレイヤーの方向転換、マウス左押下で銃撃、1キーでデバッグ表示を切り替える。  

  + [/Test/](/Assets/SubmarineMirageFrameworkForUnity/Test/Test)  
  各処理の試験プログラムが纏められている。  
  [Test.unity](/Assets/SubmarineMirageFrameworkForUnity/Test/Test/Test.unity)
  シーン内、Scriptsゲームオブジェクトに、試験プログラムを挿入している。  
  実行中のデバッグ表示切り替えは、1キーを使用する。  

+ [/Scripts/](/Assets/SubmarineMirageFrameworkForUnity/Scripts)  
開発プログラムが存在している。  
以降は、このフォルダ直下の説明を行う。  
  + [/Main/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/Main)  
  [/MainProcess.cs](/Assets/SubmarineMirageFrameworkForUnity/Scripts/Main/MainProcess.cs)
  は、Unity実行時に、一番最初に実行するプログラムである。  


#### 中心プログラム
[/System/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System)
に、フレームワークの中心プログラムが纏められている。  
直下に、ネットワーク通信管理、時間管理のプログラムが存在する。  
以降は、このフォルダ直下の説明を行う。  

+ [/Audio/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Audio)  
音関連のプログラムが纏められている。  
ゲームオブジェクトに貼り付けることなく、プログラムから呼び出せる。  
音楽（BGM）、背景音（BGS）、ジングル音（Jingle）、効果音（SE）、ループ効果音（LoopSE）、声音（Voice）を再生できる。  

+ [/Build/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Build)  
Unityのビルド時に、自動実行されるプログラムが、纏められている。  
プログラム書類に、ライセンス文章を追加するプログラムが存在する。  

+ [/Data/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data)  
データ関連のプログラムが纏められている。  
当項目では、このフォルダ直下の説明を行う。  

  + [/File/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/File)  
  書類の読み書き関連のプログラムが纏められている。  
  画像、音、シリアライズされたクラス、CSV、文章、生のデータを読み書きできる。  
  アプリ内（リソース）、アプリ外、サーバーから読み書きできる。  
  暗号化、キャッシュ（ローカル保存可）に対応している。  
  （アセットバンドルは、未対応である。）  

  + [/Master/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Master)  
  マスターデータ関連のプログラムが纏められている。  
  広告、課金、エラー、システム、アイテムのデータを、CSV書類から読み込む。  
  当項目では、このフォルダ直下の説明を行う。  

    + [/Command/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Master/Command)  
    命令データ関連のプログラムが纏められている。  
    人工知能の命令、構文解析のデータを、CSV文章から読み込む。  

  + [/Raw/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Raw)  
  生データ関連のプログラムが纏められている。  
  UnityのAudioSource、Texture、Sprite等は、そのままでは書類に読み書きできない為、生データと相互変換する。  

  + [/Save/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Save)  
  セーブデータ関連のプログラムが纏められている。  
  遊戯、設定、サーバーキャッシュのデータを、暗号化し、書類に読み書きする。  
  [外部シリアライザ](https://github.com/deniszykov/msgpack-unity3d)
  が優秀な為、クラスごとシリアライズ保存でき、IL2CPPビルド用の事前コード生成の必要もない。  
  （当然、PlayerPrefsは未使用の為、安全である。）  
  データの保存先は、PC対象時は
  [Data/](/Data)
  であり、WindowsPCでAndroid対象時は
  [DataForAndroid.lnk](/DataForAndroid.lnk)
  のリンク先となる。  

  + [/Server/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Server)  
  サーバーデータ関連のプログラムが纏められている。  
  アプリ、製品版宣伝、他アプリ宣伝のデータを、指定URLからダウンロードし、CSV書類を読み込む。  
  [サーバー（GoogleDrive）](https://drive.google.com/open?id=18zwqZntrghe_CXDFTHbBmCcd-jDUedVi)
  に、ダウンロード元の各種サーバーデータが存在する。  

+ [/Debug/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Debug)  
デバッグ関連のプログラムが纏められている。  
処理の種類別に色文字を追加するデバッグログ、ゲーム画面にデバッグ文章の描画、FPS計測の処理を記述している。  
実行中のデバッグ表示切り替えは、1キーを使用する。  

+ [/Extension/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Extension)  
拡張クラス関連のプログラムが纏められている。  
uGUIとnGUIの装飾文字、ゲームオブジェクト、コンポーネント、数学、レイヤー、タグ、音、スプライト、テクスチャ、ネットワーク、  
ジェネリック、オブジェクト、タイプ、真偽値、色、文字、等の便利処理を記述している。  
キャッシュ機能付きMonoBehaviour、入力の管理、スプラッシュ画面の終了待機、シリアライズでのToDeepString()やToDeepCopy()を実装している。  

+ [/FSM/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/FSM)  
有限状態機械関連のプログラムが纏められている。  
コルーチン駆動による、汎用的で、使い易いFSMである。  
OnEnter()、OnUpdate()、OnExit()がコルーチンの為、非同期処理が可能である。  
また、毎フレーム呼ばれるOnUpdateDelta()を使う事で、通常の更新処理も行える。  

+ [/Process/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process)  
MonoBehaviourを介さずゲームループを行う、プログラムが纏められている。  
ゲームオブジェクト化すること無く、Initialize()、Update()、Finalize()等を非同期で実行し、システムの内部処理をMonoBehaviourから分離できる。  

  + [/BaseProcess.cs](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/BaseProcess.cs)  
  ゲームオブジェクト化する意味は無いが、初期化、更新、終了処理等が必要な場合に、使用する基盤クラスである。  
  [MonoBehaviourProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)
  と同じように使用できる。  

  + [/MonoBehaviourProcess.cs](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)  
  フレームワーク内のコンポーネントは、全て、MonoBehaviourではなくこのクラスを継承する。  
  このクラスは、既存のMonoBehaviourと比べ、より厳密なゲームループと非同期実行を提供する。  
  例として、async Load()、async Initialize()が生成直後に呼ばれ、その実行中は、Update()等が呼ばれない。  

  + [/CoroutineProcess.cs](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Coroutine/CoroutineProcess.cs)  
  UniRxのMonoBehaviour不要のコルーチンの、簡易記述を実装している。  
  遅延再生、一時停止、自動解放の処理が簡単に行える。  
  [CoroutineUtility](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Utility/CoroutineUtility.cs)
  にて、更に簡単に使用できる。  

+ [/Singleton/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Singleton)  
[Process](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process)
処理を用いた、シングルトンのプログラムが纏められている。  
ゲームオブジェクト用と、ゲームオブジェクト化しないシステム内部用の、シングルトンが存在する。  
シングルトンは、各種管理処理に継承される。  

+ [/Utility/](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Utility)  
各種処理を便利にしたプログラムが纏められている。  
ゲームオブジェクト、フォルダ階層、シリアライズ、等の便利処理を記述している。  
ビルボード、
[CoroutineProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Coroutine/CoroutineProcess.cs)
の簡易呼出、#DEVELOPのみ存在のゲームオブジェクト、
[MonoBehaviourProcess](/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)
のテンプレートを実装している。  



## 外部ライブラリ
#### 外部ライブラリの配置フォルダ
[Assets/Plugins/ExternalAssets/](/Assets/Plugins/ExternalAssets)
に、導入した外部ライブラリを配置している。  
（Plugins/以下に配置する理由は、.csprojを別アセンブリ化し、ゲーム本体プログラム変更の度に、外部プログラム全体を再ビルドしない為である。）  
主に、前職のゲーム開発企業で使用した、ライブラリを導入している。  

+ [DOTween](http://dotween.demigiant.com/index.php)  
これは、音、画像、移動、数値等、様々な処理に、トゥイーン動作を付けるライブラリである。  
様々なゲーム開発企業で使用される有名ライブラリの為、これに対応する事で、業界水準と鑑みている。  
当フレームワークでは、音のフェード処理や、UIの処理に使用している。  

+ [Json & MessagePack Serialization](https://github.com/deniszykov/msgpack-unity3d)  
これは、プログラムのクラスや構造体のデータを、文章化し、JSONやMessagePack形式に変換するライブラリである。  
Unity固有のクラスや構造体も文章化でき、IL2CPPビルド時に事前コード生成が要らず、特殊な属性を付ける必要も無く（除外属性は付ける事が可能）、扱い易い。  
ただし、クラスのポリモーフィズムに未対応の為、配列等で基底クラス読込の場合、継承先クラスのメンバが復元されない。  
当フレームワークでは、書類の読み書き（Save()、Load()）、クラス中身のデバッグ表示（ToDeepString()）、クラスのコピー（DeepCopy()）等に使用している。  

+ [KoganeUnityLib](https://github.com/baba-s/kogane-unity-lib)  
これは、C#やUnityの簡易記述を可能にする、拡張、便利クラスのライブラリである。  
当初、同じような便利プログラムの実装を検討していたが、車輪の再開発をする必要は無い為、ライブラリに依存した。  
当フレームワークでは、至る所で、便利記述に使用している。  

+ [Lunar Unity Mobile Console](https://github.com/SpaceMadness/lunar-unity-console)  
これは、スマートフォン端末で、画面にデバッグログを表示する為のライブラリである。  
uGUI装飾による色文字等は反映せず、余計な画像によりアプリサイズが大きくなる問題があり、他ライブラリと差し替えを検討している。  
当フレームワークでは、Androidのデバッグの為に使用している。  

+ [ReferenceViewer](https://github.com/anchan828/ReferenceViewer)  
これは、Unity内のアセットが何処で使われているか、全参照を表示するライブラリである。  
当フレームワークでは、アセットの整理整頓に使用している。  

+ [ScriptingDefineSymbolsEditor](https://github.com/kankikuchi/ScriptingDefineSymbolsEditor)  
これは、#シンボルをPlayerSettingsでプラットフォーム別に設定する手間を省き、簡単に定義できるライブラリである。  
当フレームワークでは、#DEVELOP等のシンボル定義に使用している。  

+ [UniRx](https://github.com/neuecc/UniRx)  
これは、ラムダ式等を用い、関数定義を簡略化し、複雑な処理の簡略化を行うライブラリである。  
様々なゲーム開発企業で使用される有名ライブラリの為、これに対応する事で、業界水準と鑑みている。  
当フレームワークでは、至る所で全面的に積極的に使用している。  

+ [UniTask](https://github.com/Cysharp/UniTask)  
これは、C#のasync/awaitの非同期処理を、Unityに対応させ、簡易記述ができるライブラリである。  
コルーチン、UniRxの非同期を経て、次世代の非同期処理として期待されているが、キャンセル処理に難がある為、使用が難しい。  
当フレームワークでは、可能な限り、async/await処理を使用している。  



## 不具合、実装予定
不具合の解決、新機能の実装予定等は、逐次
[Issues](/../../issues)、
[Projects](/../../projects/1)
に記載する。  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)
開発中の為、スタジオがゲーム公開の度に、次回作開発前に、毎回追加分をマージする予定である。  



## 由来
+ Submarine Mirage Framework for Unity  
Unity用の、海底の蜃気楼フレームワーク。  

+ Submarine  
  + 海底  
  [夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)
  ゲーム開発に使用する為、海底とした。  

  + 潜水艦  
  スタジオの組員皆で乗って行く船として、潜水艦の意味も含む。  
  ロゴ画像から分かる通り、ボロボロの沈没船である。  
  社会の競争に負けた沈没船とも言える。  

+ Mirage  
  + 蜃気楼  
  完成可能性が低く、理想を追えども中々実現に至らない為、蜃気楼と名付けた。  
  （ファイナルファンタジー根性でもある。）  
  ゲームが未完成となる場合を鑑み、膨大な作業を水の泡と帰さない為、せめて開発環境を先に公開する。  
  似た悩みを持つインディーゲーム開発者の為、我が屍が道と成らんことを。



## 開発者
+ 夢想海の水底より（From Seabed of Reverie）  
[Web](https://seabed-of-reverie.wixsite.com/front)  
[Twitter](https://twitter.com/SeabedOfReverie)  

+ 共同開発者を募集中  
万人が便利に使う為のフレームワークは、作家性等とは無関係に、作業分担可能と思われる。  
[Issues](/../../issues)、
[掲示板](https://seabed-of-reverie.wixsite.com/front/notice-board)、
[お問い合わせ](https://seabed-of-reverie.wixsite.com/front/contact)等から連絡頂けると幸いである。  



## ライセンス
+ Submarine Mirage Framework for Unity  
[MIT License](/LICENSE)  

+ 外部ライブラリ  
他者が開発した外部ライブラリを多数使用している為、それぞれ別々のライセンスが存在する。  
詳しくは、[LICENSE](/LICENSE)書類を確認頂きたい。  