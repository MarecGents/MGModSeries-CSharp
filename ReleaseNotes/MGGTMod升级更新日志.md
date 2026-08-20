# 发布链接
## MG-GT-Mod v0.4.0.040100 - v0.4.0.040101
爱发电主页链接：https://ifdian.net/a/MarecGents
论坛主页链接：https://sns.oddba.cn/author/92586
Github链接：https://github.com/MarecGents/MG-GT-Mod/releases/latest
百度网盘链接: https://pan.baidu.com/s/1smGqyFzbDV832-InjMc4tQ?pwd=MGYY 提取码：MGYY 
迅雷网盘链接：https://pan.xunlei.com/s/VOz1L_Ve67UJiRWNK0xRdmYeA1?pwd=ijjz 提取码：ijjz
夸克网盘链接：https://pan.quark.cn/s/47cd989d2a57?pwd=MzAt 提取码：MzAt
123 网盘链接：https://1827650002.share.123pan.cn/123pan/txrKjv-Q6Gy3?pwd=MGYY# 提取码：MGYY

# MG-GT-Mod v0.x.x.x
## 新增

## 修复

## 变更

## 优化

## 移出

# MG-GT-Mod v0.5.0.040102
FG商人新增任务系统、护甲/弹挂甲装备与自定义弹挂布局，全面适配SPT-4.1.2。✅
## 新增
1. FG商人任务系统（三任务串行任务链，完成后解锁对应物品购买权限）：
   - 《真材实料》：收集并上交5件ANA Alpha弹挂，奖励FG Alpha胸挂（爆改款）✅
   - 《硬壳》：收集并上交5件IOTV护甲，奖励FG护甲（芳纶内衬强化）✅
   - 《弹袋钢板》：收集并上交5件RBAV弹挂甲，奖励FG弹挂甲（弹袋重排+芳纶内衬）✅
2. FG商人新装备：
   - FG Alpha胸挂（ANA Tactical爆改款）✅
   - FG护甲（IOTV强化，9部位芳纶内衬满配）✅
   - FG弹挂甲（RBAV-AF强化，5部位芳纶内衬）✅
   - 9个芳纶内衬软甲（前/后/左/右/护颈/双肩/腹股沟/后腹股沟）✅
3. 自定义RigLayout弹挂布局（bundle客户端注入）：FG_Alpha（10格5×7）、FG_RBAV（11格5×7）✅
4. 护甲/弹挂甲ItemPresets完整装配预设——任何渠道（商人/跳蚤）获取均为内置软甲满配✅
5. 新增3张任务图片（FGquest001/002/003.jpg）与任务/物品背景故事文案✅
6. 护甲/弹挂甲前后左右插板装配预设（ItemPresets 预设内置 SAPI6 前后板 + SSAPI6 侧板）与商人插板出售子项（购买护甲/弹挂甲即含板）✅
7. 《硬壳》《弹袋钢板》任务奖励的 FG 护甲/FG 弹挂甲内置前后左右插板（SAPI6 前后板 + SSAPI6 侧板）与 required 芳纶软甲满配（插板+软甲全套，与商人/预设同款）✅
8. FG 商人支持收购玩家物品（items_buy 配置：弹挂/护甲/针剂/内置插板/武器/弹药/医疗类）——护甲/弹挂甲等可出售给商人 ✅
## 修复
1. 修复任务解锁物品购买（questassort）导致的服务端启动崩溃（键值方向修正：assort物品id→任务id）✅
2. 修复护甲/弹挂甲在跳蚤市场AI挂单中无软甲、亮红不可用的问题（ItemPresets预设完整装配）✅
3. 修复客户端图鉴（HandBook）加载 NRE（NullReferenceException：Object reference not set）——ItemPresets 预设 `_parent` 字段修正为根物品 `_id`（原写成物品 tpl，导致 `ItemPresetSerializer.Deserialize` 返回 Item=null）✅
4. 修复护甲/弹挂甲插板槽空板警告（Unable to randomise armor items ... slot as it cannot be found）——清空插板槽 Plate 默认值（Filter 保留可自装）✅
5. 修复 RBAV 弹挂甲 ItemPresets 预设子项 `_id` 大量重复（前6项同为 ...201，会导致预设装配错乱）——按槽位唯一化（...201~...209）✅
6. 修复 RBAV 弹挂甲商人插板子项 `_tpl` 误用软甲（9cc23608...003 放不进插板槽）——改为 SAPI6 前后板（656faf0c...）/ SSAPI6 侧板（64afdb57...）✅
7. 护甲/弹挂甲 Slots `_parent` 由原版物品 id 修正为 FG 物品 id（9cc23608...012/013）✅
## 变更
1. 任务收集条件扩展为同系列多物品（ANA弹挂系/IOTV护甲系/RBAV弹挂甲系3选多），降低完成难度✅
2. 软甲不再单独出售（禁售，仅作为护甲/弹挂甲内置部分）✅
3. 护甲/弹挂甲允许在跳蚤市场出售（玩家挂单带软甲、AI挂单预设满配）✅
4. 三任务新增击杀完成条件（与上交物品并存）：
   - 《真材实料》：任意地点击杀 5 名 scav✅
   - 《硬壳》：灯塔击杀 10 名游荡者（Rogue/exUsec，地点约束灯塔）✅
   - 《弹袋钢板》：实验室击杀 10 名 PMC（地点约束实验室）✅
## 优化
1. FG商人物品补充背景故事描述（itemsdescription），与任务剧情呼应✅
2. 依赖升级：SPTarkov 包 4.1.1 → 4.1.2（与 MGModServer 对齐，全面适配 SPT 4.1.2）✅
3. 客户端资源目录重构：`bundles/rig/` → `bundles/resources/rig/`（对应 Unity Resources 系统，与 SPT bundle 链路隔离；客户端由 MGModClient 通用加载器按类型分发注入）✅
4. 三任务击杀完成条件文本注册进 mail.json（other 条件文本 + 任务描述补充击杀要求条目）✅
## 移出
0. x


# MG-GT-Mod v0.4.0.040101
适配SPT-4.1.1版本。✅
## 新增
0. x
## 修复
0. x
## 变更
0. x
## 优化
0. x
## 移出
0. x

# MG-GT-Mod v0.4.0.040100
大版本更新至v0.4.0，全面适配SPT-4.1.0版本全新服务端架构。✅
## 新增
0. x
## 修复
0. x
## 变更
0. x
## 优化
0. x
## 移出
0. x