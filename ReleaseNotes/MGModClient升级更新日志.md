# 发布链接
见MGMod和MGGTMod


# MGModClient v0.2.1.040013-hotfix-1
## 新增
0. x
## 修复
1. 【功能】[客户端资源]-[弹挂布局]：修复 rig 不显示格子的根因——ModDetector 缺少 C# SPT 4.0.13 目录 SPT/user/mods（4.1 起改为 SPT_Runtime/user/mods）。改为多候选探测并按分支优先：4.0.x SPT/user/mods 优先，4.1.x SPT_Runtime/user/mods 优先。✅
2. 【功能】[客户端资源]-[弹挂布局]：注入改覆盖式写入——空缓存项覆盖注入（Pop 未命中时 null 已写入字典致 TryAdd 静默失败），非空原版资源跳过。✅
## 变更
1. 【版本】v0.2.1.040013-hotfix-1（修复 rig 路径）。✅
## 优化
0. x
## 移出
0. x

# MGModClient v0.2.1.040013
## 新增
0. x
## 修复
1. 【功能】[客户端资源]-[弹挂布局]：适配 SPT 4.0.13 游戏客户端——资源缓存类由 EFT.Utilities.ResourcesCache 更名为 CacheResourcesPopAbstractClass（静态字段 _storage→Dictionary_0），RigLayoutInjector 注入目标同步更新，修复自定义弹挂布局注入失效。✅

## 变更
1. 【版本】版本号更新至 v0.2.1.040013（适配 SPT 4.0.13 客户端）。✅
## 优化
0. x
## 移出
0. x

# MGModClient v0.2.1.040102
## 新增
0. x
## 修复
1. 【功能】[配置编辑]：补齐 MGConfig 配置模型（对齐服务端 ConfigSettingType 全部字段），修复 F12 菜单触发写回时清空 config.json 的问题。✅
2. 【功能】[配置编辑]：JsonUtils.WriteAtomic 在目标文件不存在时改用 File.Move，修复首次写回失败。✅
3. 【功能】[配置编辑]：修复 config.json 损坏/缺失时写回默认空模型覆盖原文件的问题——读取失败时跳过写回。✅
## 变更
1. 【版本】版本号更新至 v0.2.1.040102。✅
## 优化
1. 【功能】[配置编辑]：读取到损坏 config.json 时输出警告日志（不再静默回退默认）。✅
## 移出
0. x


# MGModClient v0.2.0.040102
## 新增
1. 【功能】[客户端资源]-[通用加载器]：新增 ClientResourceLoader——检索 MGMod 与 MGGTMod 的 `bundles/resources/` 下全部 .bundle（含子文件夹递归），按资源类型分发加载。✅
2. 【功能】[客户端资源]-[弹挂布局]：RigLayoutInjector 改为类型处理器（TryInjectPrefab），支持 FG_Alpha/FG_RBAV 与 MG_Velocity_Systems 自定义弹挂布局注入。✅
3. 【功能】[客户端资源]-[目录约定]：客户端注入资源统一收归 `bundles/resources/`（对应 Unity Resources 系统，与 SPT bundle 链路隔离），按类型分子目录（rig 布局、Slots/语音预留）。✅
## 修复
0. x
## 变更
1. 【版本】版本号更新至 v0.2.0.040102。✅
## 优化
0. x
## 移出
0. x

