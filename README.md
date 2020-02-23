# Submarine Mirage Framework for Unity
##### Version 0.1
![Logo.png](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Textures/Logo.png?raw=true)  



## 概要
Submarine Mirage Framework for Unityとは、Unityでのゲーム開発時に、不便さを補い、安定、迅速、堅牢な実装を行う為の、フレームワークである。  
市販レベルのゲームで、一般的、汎用的、必須な機能を、全体的に実装している。  
しかし、現在開発中の為、[未実装機能](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)が沢山あり、不安定である。  


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
[KSFramework](https://github.com/mr-kelly/KSFramework)等のフレームワークの方が出来が良い為、そちらをお勧めする。  

+ 導入事例  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)（インディーゲーム開発スタジオ）の開発環境に採用中である。  


#### 特徴
+ Unityのいい加減な機能を、補強  
Unityは、いい加減なゲーム開発をサポートし、市販ゲーム程度のきちんと動作するゲーム開発を、ほぼ全くサポートしない。  
Unityの問題ばかりの機能の内、ゲーム開発で頻繁に使われるであろう機能を、堅牢に、使い易く実装している。  

+ フレームワークとして、全面実装  
フレームワークの為、特定の機能実装だけでなく、多種多様な機能を全体的に実装している。  

+ 外部ライブラリ導入済の為、環境構築が簡単  
業界水準を鑑み、ゲーム開発企業で頻繁に使用される、多種多様な外部ライブラリを同封している。  
（ライセンス上、同封可能なライブラリのみ。）  
このフレームワークを導入するだけで、即、開発環境を構築可能である。  

+ 見易いプログラム  
可読性を意識し、（日本語であるが）見易いコメント、分かり易い設計を重視している。  
プログラムが見易いので、使い易いフレームワークである。  


#### 対応
+ Unity動作環境  
  + 2018.4.16f1～（以降）  
  動作確認は、2018.4.16f1を使用している。

+ ビルド対応  
  + Windows（IL2CPP含む）  
  + Android（IL2CPP含む）  



## 使用方法
現在、執筆中。  

<!--
+ 導入  
Assets/以下のファイルを全てコピー
UnityAds、UnityIAPを導入


+ サンプル  
SubmarineMirageFrameworkForUnity\Sample\Scenes\Sample.unity

+ 使い方  
BaseProcessを使い、
MonoBehaiviorProcessを使い、
Singletonを使い、管理クラス作成
MainProcessに、登録


+ 狙い  
UniRx使用のリアクティブスパゲッティと、Processプログラムの比較を、画像等で表示
要は、UniRxだとゴチャゴチャなのは、コンポーネント設計が、全て等価だから
マネージャー系は、最初にまとめて順番通り処理、その後シーンコンポーネントを順不同で処理し、処理完了まで待ってから、フェードイン
Constractorが、生成直後に呼ばれ、
Load→Initializeを非同期実行し、
FixedUpdate→Update→LateUpdateを初期化完了後に毎回、それぞれのタイミングで呼び出し、
Finalizeを非同期実行を待機後、
オブジェクトが破棄されて、シーン遷移

Constractorで、マネージャーの参照を取得（この時すでにマネージャーは初期化済）
Loadで、自身のオブジェクトに必要なリソースのサーバー、ローカル読込
Initializeで、他オブジェクトの参照を入手（既に読込済なのでnullにならない）
全ての初期化が完了後、各種Update処理
Finalizeで、破棄に時間が掛かる処理を想定し、非同期破棄
全て破棄してから、シーン切り替え


  ゲーム起動　→　初期シーン読込　→　全ての中心管理処理のLoad()　→　全ての中心管理処理のInitialize()　→  
  シーン読込　→　シーン内の全ての管理処理のLoad()　→　シーン内の全ての管理処理のInitialize()　→  
  全てのゲームオブジェクト処理のLoad()　→　全てのゲームオブジェクト処理のInitialize()　→  
  中心管理処理、シーン内の全ての管理処理、ゲームオブジェクト処理のFixedUpdate()、Update()、LateUpdate()　→　繰り返し　→  
  シーン切り替え　→  
  全てのゲームオブジェクト処理のFinalize()　→  
  シーン内の全ての管理処理のFinalize()　→  
  全ての中心管理処理のFinalize()　→  
  ゲーム終了  

Load、Initialize（非同期の為、サーバー受信やロードに使用できる）が順番に呼ばれ、
初期化が完了してから、FixedUpdate、Update、LateUpdateを繰り返し呼び出し、
シーンが破棄される前に、Finalizeが、非同期で呼ばれる
-->


## 実装処理
#### フレームワークの配置フォルダ
[Assets/SubmarineMirageFrameworkForUnity/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity)
に、フレームワークが配置されている。  
以降は、このフォルダ直下の説明を行う。  

+ [/Sample/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Sample)  
使用例のシーンが存在する。  

+ [/Scripts/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts)  
開発プログラムが存在している。  
以降は、このフォルダ直下の説明を行う。  
  + [/Main/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/Main)  
  [/MainProcess.cs](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/Main/MainProcess.cs)
  は、Unity実行時に、一番最初に実行するプログラムである。  

  + [/Test/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/Test)  
  各処理の試験プログラムが纏められている。  
  [Sample.unity](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Sample/Scenes/Sample.unity)
  シーンの、Scriptsゲームオブジェクトの、コンポーネントプログラムも、このフォルダに存在する。  


#### 中心プログラム
[/System/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System)
に、フレームワークの中心プログラムが纏められている。  
直下に、ネットワーク通信管理、時間管理のプログラムが存在する。  
以降は、このフォルダ直下の説明を行う。  

+ [/Audio/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Audio)  
音関連のプログラムが纏められている。  
ゲームオブジェクトに貼り付けることなく、プログラムから呼び出せる。  
音楽（BGM）、背景音（BGS）、ジングル音（Jingle）、効果音（SE）、ループ効果音（LoopSE）、声音（Voice）を再生できる。  

+ [/Build/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Build)  
Unityのビルド時に、自動実行されるプログラムが、纏められている。  
プログラム書類に、ライセンス文章を追加するプログラムが存在する。  

+ [/Data/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data)  
データ関連のプログラムが纏められている。  
当項目では、このフォルダ直下の説明を行う。  

  + [/File/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/File)  
  書類の読み書き関連のプログラムが纏められている。  
  画像、音、シリアライズされたクラス、CSV、文章、生のデータを読み書きできる。  
  アプリ内（リソース）、アプリ外、サーバーから読み書きできる。  
  暗号化、キャッシュ（ローカル保存可）に対応している。  
  （アセットバンドルは、未対応である。）  

  + [/Master/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Master)  
  マスターデータ関連のプログラムが纏められている。  
  広告、課金、エラー、システム、アイテムのデータを、CSV書類から読み込む。  
  当項目では、このフォルダ直下の説明を行う。  

    + [/Command/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Master/Command)  
    命令データ関連のプログラムが纏められている。  
    人工知能の命令、構文解析のデータを、CSV文章から読み込む。  

  + [/Raw/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Raw)  
  生データ関連のプログラムが纏められている。  
  UnityのAudioSource、Texture、Sprite等は、そのままでは書類に読み書きできない為、生データと相互変換する。  

  + [/Save/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Save)  
  セーブデータ関連のプログラムが纏められている。  
  遊戯、設定、サーバーキャッシュのデータを、暗号化し、書類に読み書きする。  
  [外部シリアライザ](https://github.com/deniszykov/msgpack-unity3d)
  が優秀な為、クラスごとシリアライズ保存でき、IL2CPPビルド用の事前コード生成の必要もない。  
  （当然、PlayerPrefsは未使用の為、安全である。）  
  データの保存先は、PC対象時は
  [Data/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Data)
  であり、WindowsPCでAndroid対象時は
  [DataForAndroid.lnk](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/DataForAndroid.lnk)
  のリンク先となる。  

  + [/Server/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Data/Server)  
  サーバーデータ関連のプログラムが纏められている。  
  アプリ、製品版宣伝、他アプリ宣伝のデータを、指定URLからダウンロードし、CSV書類を読み込む。  
  [サーバー（GoogleDrive）](https://drive.google.com/open?id=18zwqZntrghe_CXDFTHbBmCcd-jDUedVi)
  に、ダウンロード元の各種サーバーデータが存在する。  

+ [/Debug/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Debug)  
デバッグ関連のプログラムが纏められている。  
処理の種類別に色文字を追加するデバッグログ、ゲーム画面にデバッグ文章の描画、FPS計測の処理を記述している。  

+ [/Extension/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Extension)  
拡張クラス関連のプログラムが纏められている。  
uGUIとnGUIの装飾文字、ゲームオブジェクト、コンポーネント、数学、レイヤー、タグ、音、スプライト、テクスチャ、ネットワーク、  
ジェネリック、オブジェクト、タイプ、真偽値、色、文字、等の便利処理を記述している。  
キャッシュ機能付きMonoBehaviour、入力の管理、スプラッシュ画面の終了待機、シリアライズでのToDeepString()やToDeepCopy()を実装している。  

+ [/FSM/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/FSM)  
有限状態機械関連のプログラムが纏められている。  
コルーチン駆動による、汎用的で、使い易いFSMである。  
OnEnter()、OnUpdate()、OnExit()がコルーチンの為、非同期処理が可能である。  
また、毎フレーム呼ばれるOnUpdateDelta()を使う事で、通常の更新処理も行える。  

+ [/Process/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process)  
MonoBehaviourを介さずゲームループを行う、プログラムが纏められている。  
ゲームオブジェクト化すること無く、Initialize()、Update()、Finalize()等を非同期で実行し、システムの内部処理をMonoBehaviourから分離できる。  

  + [/BaseProcess.cs](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/BaseProcess.cs)  
  ゲームオブジェクト化する意味は無いが、初期化、更新、終了処理等が必要な場合に、使用する基盤クラスである。  
  [MonoBehaviourProcess](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)
  と同じように使用できる。  

  + [/MonoBehaviourProcess.cs](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)  
  フレームワーク内のコンポーネントは、全て、MonoBehaviourではなくこのクラスを継承する。  
  このクラスは、既存のMonoBehaviourと比べ、より厳密なゲームループと非同期実行を提供する。  
  例として、async Load()、async Initialize()が生成直後に呼ばれ、その実行中は、Update()等が呼ばれない。  

  + [/CoroutineProcess.cs](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Coroutine/CoroutineProcess.cs)  
  UniRxのMonoBehaviour不要のコルーチンの、簡易記述を実装している。  
  遅延再生、一時停止、自動解放の処理が簡単に行える。  
  [CoroutineUtility](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Utility/CoroutineUtility.cs)
  にて、更に簡単に使用できる。  

+ [/Singleton/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Singleton)  
[Process](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process)
処理を用いた、シングルトンのプログラムが纏められている。  
ゲームオブジェクト用と、ゲームオブジェクト化しないシステム内部用の、シングルトンが存在する。  
シングルトンは、各種管理処理に継承される。  

+ [/Utility/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Utility)  
各種処理を便利にしたプログラムが纏められている。  
ゲームオブジェクト、フォルダ階層、シリアライズ、等の便利処理を記述している。  
ビルボード、
[CoroutineProcess](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Coroutine/CoroutineProcess.cs)
の簡易呼出、DEVELOPのみ存在のゲームオブジェクト、
[MonoBehaviourProcess](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Scripts/System/Process/Base/MonoBehaviourProcess.cs)
のテンプレートを実装している。  



## 外部ライブラリ
#### 外部ライブラリの配置フォルダ
[Assets/Plugins/ExternalAssets/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/Plugins/ExternalAssets)に、導入した外部ライブラリを配置している。  
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
当フレームワークでは、書類の読み書き（Save & Load）、クラス中身のデバッグ表示（ToDeepString）、クラスのコピー（DeepCopy）等に使用している。  


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
不具合の解決、新機能の実装予定等は、逐次[Issues](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)に記載する。  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)開発中の為、スタジオがゲーム公開の度に、次回作開発前に、毎回追加分をマージする予定である。  



## 由来
+ Submarine Mirage Framework for Unity  
Unity用の、海底の蜃気楼フレームワーク。  

+ Submarine  
  + 海底  
  [夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)ゲーム開発に使用する為、海底とした。  

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
+ [夢想海の水底より（From Seabed of Reverie）](https://seabed-of-reverie.wixsite.com/front)  

+ [神呪歌人魚](https://twitter.com/SirenSingsCurse)  

+ 共同開発者を募集中  
万人が便利に使う為のフレームワークは、作家性等とは無関係に、作業分担可能と思われる。  
[Issues](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)、
[掲示板](https://seabed-of-reverie.wixsite.com/front/notice-board)、
[お問い合わせ](https://seabed-of-reverie.wixsite.com/front/contact)等から連絡頂けると幸いである。  



## ライセンス
+ [MIT License](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE)  

+ 外部ライブラリ  
他人が開発した外部ライブラリを多数使用している為、それぞれ別々のライセンスが存在する。  
詳しくは、[LICENSE](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE)書類を確認頂きたい。  