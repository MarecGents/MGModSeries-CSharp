# 发布链接
见MGMod

# MGModEditor vx.x.x.x
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

# MGModEditor v1.3.2.1
## 新增
0. x
## 修复
1. 【MGModEditor】[主题]：修复自定义主题（如 TeaDark）加载失效的问题——主题字典 pack URI 残留旧程序集名「MGEditor」，改为「MGModEditor」后 36 个自定义主题恢复正常加载与切换。✅
2. 【MGModEditor】[设置]：About 页应用名与版本串由「MGEditor」统一为「MGModEditor」。✅
## 变更
1. 【MGModEditor】版本号更新至 v1.3.2.1。✅
## 优化
0. x
## 移出
0. x

# MGModEditor v1.3.2.0
## 新增
1. 【MGModEditor】全部功能页：新增"功能描述"提示——每个功能行左侧按钮旁新增圆形问号图标，悬停或点击显示功能说明（悬停 10 秒 ToolTip、点击 Popup 钉住，随语言切换实时刷新）。✅
2. 【MGModEditor】[战局系统]/[养成系统]/[经济系统]/[特色功能]：22 个分组标题新增说明图标。✅
3. 【MGModEditor】[容器扩容]：23 个容器标题新增容器说明（存放内容与扩容提示）。✅
4. 【MGModEditor】i18n：新增 159 个功能描述键 × 5 语言（功能行描述 114 + 分组标题说明 22 + 容器说明 23），术语按 SPT 官方语料对齐（地图名/T7/幸运Scav垃圾箱/安全箱/健身区等）。✅
## 修复
1. 【MGModEditor】[配置]：修复 config.json 损坏时编辑器直接崩溃的问题——反序列化增加容错，损坏/缺失先备份（config.json.corrupt-<时间戳>）再回退 defaultConfig.json。✅
2. 【MGModEditor】[配置]：修复 5 个功能页 ViewModel 构造函数误调 SaveConfig 导致每次进入页面都写盘（并刷新 saveTime）的问题。✅
## 变更
1. 【MGModEditor】版本号更新至 v1.3.2.0。✅
## 优化
1. 【MGModEditor】[配置]：config.json 原子写增加异常处理与 .tmp 残留清理，保存失败返回状态并记录日志。✅
2. 【MGModEditor】[配置]：defaultConfig.json 单一来源——构建时 Link 引用服务端文件，避免与服务端拷贝漂移。✅
3. 【MGModEditor】i18n：修正 20 处功能描述文案并同步 5 语言翻译（AI 数量/难度档位命名、独立功能说明、附魔/耐久/容器等描述）。✅
## 移出
0. x

# MGModEditor v1.3.1.1
## 新增
1. 【MGModEditor】[养成系统]-[任务系统]-[3X4任务标记]：新增 3X4任务标记 开关。✅
2. 【MGModEditor】i18n：新增 Develop.Button.Quest3X4Marker 键（5 语言包）。✅
3. 【MGModEditor】[战局系统]-[地图回保]：新增 Labyrinth（迷宫）回保开关。✅
4. 【MGModEditor】[战局系统]-[资源设置]：物资倍率新增 x0 选项（关闭刷新物资）。✅
5. 【MGModEditor】i18n：新增 Raid.Map.Labyrinth 键（5 语言包，按 SPT 服务端翻译）。✅
## 修复
0. x
## 变更
0. x
## 优化
0. x
## 移出
0. x

# MGModEditor v1.3.1.0
MGModEditor升级至v1.3.1.0。
## 新增
1. 【MGModEditor】[设置]-[关于]-[致谢]：新增致谢栏，展示 15 位作者（头像+主页链接+致谢）。✅
2. 【MGModEditor】[设置]-[关于]：致谢小作文段落缩进优化。✅
## 修复
1. 【MGModEditor】[Home]-[卡片跳转]：修复 Home 卡片跳转后大标题不随语言切换的问题。✅
2. 【MGModEditor】[设置]-[语言]：修复语言下拉框空白问题。✅
## 变更
1. 【MGModEditor】i18n 多语言扩展：新增俄语/法语/日语语言包（原 2 语言 → 5 语言）。✅
2. 【MGModEditor】[Home]-[卡片]：卡片布局改为固定宽度 + 动态高度（文本测量自动增高）。✅
3. 【MGModEditor】功能页按钮：按钮文本支持流式换行（长文本自动断行）。✅
4. 【MGModEditor】容器设置：容器标题加入 i18n 翻译（23 种容器 × 5 语言）。✅
5. 【MGModEditor】版本号更新至 v1.3.0.0。✅
## 优化
1. 【MGModEditor】Home hero 横幅：新增顶部横幅（MG 图标+渐变色+多语言文案）。✅
2. 【MGModEditor】[设置]-[关于]：版本栏/致谢栏同款样式统一。✅
## 移出
0. x
