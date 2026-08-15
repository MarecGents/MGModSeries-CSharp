# -*- coding: utf-8 -*-
"""发布前全面检查：MGMod + MGGTMod 商人数据完整性"""
import json, os, sys

BASE = r'E:/Workdata/Git_repositories/MGModSeries/MGModSeries-CSharp'
SPT = r'E:/Workdata/Git_repositories/SPT/SPT_Data/database'
fail = []

def check(cond, msg):
    print(f'  [{"OK" if cond else "FAIL"}] {msg}')
    if not cond:
        fail.append(msg)

def load(p):
    return json.load(open(os.path.join(BASE, p), encoding='utf-8-sig'))

# 原版物品库（校验 tpl 存在 + 槽 filter）
items_db = json.load(open(os.path.join(SPT, 'templates/items.json'), encoding='utf-8-sig'))

print('='*60)
print('① 任务系统 (quests.json)')
print('='*60)
for tag, qf, mf, qa in [
    ('FG', 'MGGTMod/traders/FlanrecGents/templates/quests.json', 'MGGTMod/traders/FlanrecGents/locales/mail.json', 'MGGTMod/traders/FlanrecGents/traderData/questassort.json'),
    ('MG', 'MGModServer/traders/MarecGents/templates/quests.json', 'MGModServer/traders/MarecGents/locales/mail.json', 'MGModServer/traders/MarecGents/traderData/questassort.json'),
]:
    q = load(qf); m = load(mf); qa = load(qa)
    trader_root = os.path.dirname(os.path.dirname(qf))
    print(f'\n-- {tag} --')
    allids = []
    for qid, quest in q.items():
        allids.append(qid)
        check(quest.get('location') is not None, f"{quest.get('QuestName')} location 存在: {quest.get('location')}")
        for c in quest['conditions'].get('AvailableForFinish', []):
            cid = c.get('id', '')
            allids.append(cid)
            if c.get('type') == 'Elimination':
                # Kills value 必须 = 1（1>=value 判定）
                subs = c['counter']['conditions']
                kills = [s for s in subs if s.get('conditionType') == 'Kills']
                if kills:
                    check(kills[0].get('value') == 1, f"{quest.get('QuestName')} Kills value={kills[0].get('value')} (必须=1)")
                    check(kills[0].get('target') in ('Savage','AnyPmc','Usec','Bear','Any'), f"Kills target 合法: {kills[0].get('target')}")
                # 文本注册
                check(cid in m.get(qid, {}).get('other', {}), f"Elimination 条件 {cid} 已注册 mail.json other")
                allids.append(c['counter'].get('id', ''))
                for sc in subs:
                    allids.append(sc.get('id', ''))
            elif c.get('conditionType') in ('FindItem','HandoverItem'):
                check(cid in m.get(qid, {}).get('other', {}), f"{c.get('conditionType')} 条件 {cid} 已注册 mail.json other")
        # reward 子项（插板+软甲）
        for r in quest['rewards'].get('Success', []):
            for it in r.get('items', []):
                allids.append(it.get('_id', ''))
                if 'parentId' in it:
                    check(it.get('slotId') and it.get('_tpl'), f"reward 子项 {it.get('slotId')} 有 slotId+tpl")
        # id 唯一
        for ck, cv in quest['conditions'].items():
            for c in cv:
                for vc in c.get('visibilityConditions', []):
                    allids.append(vc.get('id', ''))
    dup = {i for i in set(allids) if allids.count(i) > 1}
    check(not dup, f'{tag} 全部 id 唯一 ({len(allids)})' + (f' 重复:{dup}' if dup else ''))
    # questassort 方向 + 引用
    assort = load(os.path.join(trader_root, 'traderData/assort.json'))
    assort_ids = [i['_id'] for i in assort['items']]
    for k, v in qa.items():
        for item_k, quest_v in v.items():
            check(item_k in assort_ids, f'questassort 物品 {item_k} 在 assort 存在')
            check(quest_v in q, f'questassort 任务 {quest_v} 存在')

print()
print('='*60)
print('② 商人收购 items_buy')
print('='*60)
for tag, tf in [('FG', 'MGGTMod/traders/FlanrecGents/traderInfo.json'), ('MG', 'MGModServer/traders/MarecGents/traderInfo.json')]:
    d = load(tf)
    ib = d.get('items_buy')
    check(ib and ib.get('category'), f'{tag} items_buy.category 已配置: {len(ib.get("category", []))} 类')

print()
print('='*60)
print('③ handbook 注册（跳蚤上架）')
print('='*60)
for tag, hf, items_dir in [
    ('FG', 'MGGTMod/traders/FlanrecGents/templates/handbook.json', 'MGGTMod/traders/FlanrecGents/items'),
    ('MG', 'MGModServer/traders/MarecGents/templates/handbook.json', 'MGModServer/traders/MarecGents/items'),
]:
    hb = load(hf)
    hb_ids = {it['Id'] for it in hb}
    # 顶层出售物品（assort 顶层）才需要 handbook
    assort = load(os.path.join(os.path.dirname(items_dir), 'traderData/assort.json'))
    top_sold = {i['_tpl'] for i in assort['items'] if i['parentId'] == 'hideout'}
    item_files = [f for f in os.listdir(os.path.join(BASE, items_dir)) if f.endswith('.json')]
    for fn in item_files:
        try:
            it = json.load(open(os.path.join(BASE, items_dir, fn), encoding='utf-8-sig'))
            iid = it['item']['_id']
            if iid in top_sold and iid not in hb_ids:
                check(False, f'{tag} 顶层出售物品 {fn} ({iid}) 缺 handbook 注册')
        except Exception as e:
            check(False, f'{tag} 物品 {fn} 解析失败: {e}')

print()
print('='*60)
print('④ 物品属性完整性')
print('='*60)
for tag, items_dir in [('FG', 'MGGTMod/traders/FlanrecGents/items'), ('MG', 'MGModServer/traders/MarecGents/items')]:
    item_files = [f for f in os.listdir(os.path.join(BASE, items_dir)) if f.endswith('.json')]
    for fn in item_files:
        it = json.load(open(os.path.join(BASE, items_dir, fn), encoding='utf-8-sig'))['item']
        p = it['_props']
        check(p.get('Width') and p.get('Height'), f"{tag} {it['_name']} 有 Width/Height")
        if 'Grids' in p and p['Grids']:
            for g in p['Grids']:
                gp = g['_props']
                check(isinstance(gp['cellsH'], int) and isinstance(gp['cellsV'], int),
                      f"{tag} {it['_name']} {g['_name']} cellsH/V 为数字")

print()
print('='*60)
print('结果:', '全部通过 ✅' if not fail else f'发现问题 {len(fail)} 处 ❌')
for f in fail:
    print('  -', f)
sys.exit(0 if not fail else 1)
