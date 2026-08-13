# -*- coding: utf-8 -*-
"""Scan vanilla quests Elimination conditions: categorize target usage."""
import json
from collections import Counter

q = json.load(open(r'E:/Workdata/Git_repositories/SPT/SPT_Data/database/templates/quests.json', encoding='utf-8'))
items = json.load(open(r'E:/Workdata/Git_repositories/SPT/SPT_Data/database/templates/items.json', encoding='utf-8'))

target_usage = Counter()   # target value -> count of conditions
sample_quest = {}          # target value -> sample quest name
for qid, quest in q.items():
    if not isinstance(quest, dict):
        continue
    for ck, cv in quest.get('conditions', {}).items():
        if ck not in ('AvailableForFinish', 'AvailableForStart'):
            continue
        if not isinstance(cv, list):
            continue
        for c in cv:
            ct = c.get('type')
            if ct not in ('Elimination', 'Kills'):
                continue
            t = c.get('target')
            vals = t if isinstance(t, list) else ([t] if t else [None])
            for v in vals:
                if v is None:
                    target_usage['<no-target>'] += 1
                    sample_quest.setdefault('<no-target>', quest.get('QuestName', '?'))
                else:
                    target_usage[v] += 1
                    sample_quest.setdefault(v, quest.get('QuestName', '?'))

print('=== Elimination/Kills 条件总数:', sum(target_usage.values()), '===')

# 分类：物品id（在 items.json 中）vs bot名（不在）
item_like = {}
name_like = {}
for t, cnt in target_usage.most_common():
    is_item = t in items
    (item_like if is_item else name_like)[t] = (cnt, sample_quest[t])

print()
print('=== 非物品 id 的 target（可能是 bot 名）===' )
for t, (cnt, sq) in name_like.items():
    print(f'  {t!r}: {cnt} 次 | 示例任务: {sq}')

print()
print('=== 物品 id 类 target 前 20（boss 识别装备）===')
for t, (cnt, sq) in list(item_like.items())[:20]:
    nm = items.get(t, {}).get('_name', '?')
    print(f'  {t}: {cnt} 次 ({nm}) | 示例: {sq}')
