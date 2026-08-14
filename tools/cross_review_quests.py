# -*- coding: utf-8 -*-
"""交叉审查 MG/FG 商人任务信息：quests.json / mail.json / questassort.json 一致性"""
import json, os, sys

BASE = r'E:/Workdata/Git_repositories/MGModSeries/MGModSeries-CSharp'
fail = []

def check(cond, msg):
    status = 'OK' if cond else 'FAIL'
    if not cond:
        fail.append(msg)
    print(f'  [{status}] {msg}')

def load(p):
    return json.load(open(os.path.join(BASE, p), encoding='utf-8'))

print('='*60)
print('① FG 商人 (MGGTMod/traders/FlanrecGents)')
print('='*60)
fg_q = load('MGGTMod/traders/FlanrecGents/templates/quests.json')
fg_m = load('MGGTMod/traders/FlanrecGents/locales/mail.json')
fg_qa = load('MGGTMod/traders/FlanrecGents/traderData/questassort.json')
fg_a = load('MGGTMod/traders/FlanrecGents/traderData/assort.json')

fg_ids = set()
for qid, q in fg_q.items():
    print(f'\n-- {q.get("QuestName")} ({qid}) --')
    check(q['location'] in ('Any',) or isinstance(q['location'], str), f'location 字段存在: {q["location"]}')
    # questassort 引用
    for k, v in fg_qa.items():
        for item_k, quest_v in v.items():
            if quest_v == qid:
                check(item_k in [i['_id'] for i in fg_a['items']], f'questassort 物品 {item_k} 在 assort 中存在')
                check(qid in fg_q, f'questassort 任务 {quest_v} 在 quests 中存在')
    # 条件
    has_kill = False
    for c in q['conditions'].get('AvailableForFinish', []):
        fg_ids.add(c.get('id', ''))
        if c.get('type') == 'Elimination':
            has_kill = True
            subs = c['counter']['conditions']
            kills = [s for s in subs if s.get('conditionType') == 'Kills']
            locs = [s for s in subs if s.get('conditionType') == 'Location']
            check(len(kills) == 1, f'击杀条件含 1 个 Kills 子条件 (value={c["value"]})')
            for s in kills:
                check(s['target'], f'Kills target 非空: {s["target"]}')
                fg_ids.add(s['id'])
            for s in locs:
                check(s['target'] and len(s['target']) == 1, f'Location target 合法: {s.get("target")}')
                fg_ids.add(s['id'])
            fg_ids.add(c['counter'].get('id', ''))
        else:
            fg_ids.add(c.get('id', ''))
            for vc in c.get('visibilityConditions', []):
                fg_ids.add(vc.get('id', ''))
    check(has_kill, '包含击杀条件')
    # mail.json
    m = fg_m.get(qid, {})
    check(m.get('name'), f'mail.json name 已注册: {m.get("name")}')
    check(m.get('description'), 'mail.json description 已注册')
    others = m.get('other', {})
    for c in q['conditions'].get('AvailableForFinish', []):
        cid = c.get('id', '')
        if cid and c.get('type') != 'Elimination' and c.get('conditionType') in ('FindItem', 'HandoverItem'):
            check(cid in others, f'FindItem/HandoverItem 条件 {cid} 已在 other 注册')
        if c.get('type') == 'Elimination':
            check(cid in others, f'Elimination 条件 {cid} 已在 other 注册')
    # reward 插板
    for r in q['rewards'].get('Success', []):
        for it in r.get('items', []):
            if 'parentId' in it:
                check(it.get('slotId') in ('Front_plate','Back_plate','Left_side_plate','Right_side_plate'),
                      f'reward 插板 {it.get("slotId")} slotId 合法')
                check(it['_tpl'] in ('656faf0ca0dce000a2020f77','64afdb577bb3bfe8fe03fd1d'),
                      f'reward 插板 tpl 为 SAPI6/SSAPI6: {it["_tpl"][:8]}')
                check(it['parentId'] in [x['_id'] for x in r['items']], f'插板 parentId 指向主物品')
                fg_ids.add(it['_id'])

fg_dup = {i for i in fg_ids if list(fg_ids).count(i) > 1}
check(not fg_dup, f'FG 全部 id 唯一 (共{len(fg_ids)})' + (f' 重复:{fg_dup}' if fg_dup else ''))

print()
print('='*60)
print('② MG 商人 (MGModServer/traders/MarecGents)')
print('='*60)
mg_q = load('MGModServer/traders/MarecGents/templates/quests.json')
mg_m = load('MGModServer/traders/MarecGents/locales/mail.json')
mg_qa = load('MGModServer/traders/MarecGents/traderData/questassort.json')
mg_a = load('MGModServer/traders/MarecGents/traderData/assort.json')

for qid, q in mg_q.items():
    print(f'\n-- {q.get("QuestName")} ({qid}) --')
    check(q['location'] in ('Any',) or isinstance(q['location'], str), f'location: {q["location"]}')
    for k, v in mg_qa.items():
        for item_k, quest_v in v.items():
            if quest_v == qid:
                check(item_k in [i['_id'] for i in mg_a['items']], f'questassort 物品 {item_k} 在 assort 存在')
                check(qid in mg_q, f'questassort 任务存在')
    for c in q['conditions'].get('AvailableForFinish', []):
        if c.get('type') == 'Elimination':
            subs = c['counter']['conditions']
            kills = [s for s in subs if s.get('conditionType') == 'Kills']
            locs = [s for s in subs if s.get('conditionType') == 'Location']
            check(len(kills) == 1, f'Kills 子条件 (value={c["value"]})')
            for s in kills:
                check(s['target'], f'target: {s["target"]} savageRole: {s.get("savageRole")}')
            for s in locs:
                check(s['target'] == ['laboratory'], f'Location: {s.get("target")}')
    m = mg_m.get(qid, {})
    check(m.get('name'), f'name: {m.get("name")}')
    others = m.get('other', {})
    for c in q['conditions'].get('AvailableForFinish', []):
        cid = c.get('id', '')
        if c.get('type') == 'Elimination':
            check(cid in others, f'Elimination 条件 {cid} 已在 other 注册')

print()
print('='*60)
print('③ 地图 id 抽查')
print('='*60)
check(mg_q['8ef5b2ef4000000000000001']['location'] == '5b0fc42d86f7744a585f9105',
      'MG《初次见面》location = 实验室 5b0fc42d')
check(fg_q['9cc236084000000000000002']['conditions']['AvailableForFinish'][-1]['counter']['conditions'][-1]['target'] == ['laboratory'],
      'FGQuest3 Location = 实验室')
check(fg_q['9cc236084000000000000001']['conditions']['AvailableForFinish'][-1]['counter']['conditions'][-1]['target'] == ['Lighthouse'],
      'FGQuest2 Location = 灯塔')

print()
print('='*60)
print('结果:', '全部通过 ✅' if not fail else f'发现问题 {len(fail)} 处 ❌')
for f in fail:
    print('  -', f)
sys.exit(0 if not fail else 1)
