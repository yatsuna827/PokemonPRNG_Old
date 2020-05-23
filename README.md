# PokemonPRNG
author [夜綱](https://twitter.com/sub_827)

## 概要
ポケモンの乱数処理の記述にあると便利な処理をまとめたライブラリです.
`ref this`を使用しているので, C# 7.2が必要です.
今後余裕があればMTや64bitLCG, AlternatibeLCG等にも対応するかもしれませんし, しないかもしれません.

## 使い方
`using PokemonPRNG.LCG32`するだけで
```
uint seed = 0xbadface;
seed.Advance(827); // 827消費する.
for(int i=0; i<827; i++){
    if(seed.GetRand(827)==0) break; // 乱数を取得し, 827で割った余りが0なら終了.
}
```
のように書くことができます.

## 構成
### `PokemonPRNG.LCG32`
| 名前 | 種類 | 概要 |
|:-|:-|:-|
| LCGType | `enum` | LCGの種類を指定する. |
| LCG32 | `static class` | 具体的な処理を記述する. |

#### `LCGType`
|  | 名前 | 概要 | 更新処理 |
|:-|:-|:-|:-|
| 0 | StandardLCG | 3,4世代で一般的に用いられるLCG. | `S_{n+1} = 0x41C64E6D * S_n + 0x6073` |
| 1 | GCLCG | コロシアム, XDで用いられるLCG. | `S_{n+1} = 0x343FD * S_n + 0x269EC3`|
| 2 | StaticLCG | IDくじ用の乱数等に用いられるLCG. | `S_{n+1} = 0x41C64E6D * S_n + 0x3039 `|

#### LCG32

##### 呼び出せるメソッド
| 名前 | 戻り値 | 引数 | 説明 |
|:-|:-|:-|:-|
| SetLCGType | `void` | `LCGType` | LCGの種類を変更.  |
| GetLCGSeed | `uint` | `uint, uint` | 初期seedと消費数からseedを取得. |

##### 拡張メソッド

(もちろん普通に呼び出すこともできます).

| 名前 | 戻り値 | 引数 | 説明 |
|:-|:-|:-|:-|
| NextSeed | `uint` | `this uint` | 1個先のseedを取得.  |
| NextSeed | `uint` | `this uint, uint` | n個先のseedを取得. |
| PrevSeed | `uint` | `this uint` | 1個前のseedを取得.  |
| PrevSeed | `uint` | `this uint, uint` | n個前のseedを取得. |
| Advance | `uint` | `ref this uint` | seedを1つ進める.  |
| Advance | `uint` | `ref this uint, uint` | seedをn進める. |
| Back | `uint` | `ref this uint` | seedを1つ戻す.  |
| Back | `uint` | `ref this uint, uint` | seedをn戻す. |
| GetRand | `uint` | `ref this uint` | seedを1つ進め, 16bitの乱数値を取得.  |
| GetRand | `uint` | `ref this uint, uint` | seedを1つ進め, 剰余で乱数値を取得. |
| GetIndex | `uint` | `this uint` | 0x0からの消費数を取得.  |
| GetIndex | `uint` | `this uint, uint` | 指定した初期seedからの消費数を取得. |
