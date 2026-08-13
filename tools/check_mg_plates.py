# -*- coding: utf-8 -*-
"""Check MG plate assort compatibility with vanilla plate carrier slots."""
import json
from collections import Counter

d = json.load(open(r'E:/Workdata/Git_repositories/SPT/SPT_Data/database/templates/items.json', encoding='utf-8'))
plates = {
    '64afd81707e2cf40e903a316': 'granit6side',
    '64afdb577bb3bfe8fe03fd1d': 'SSAPI6side',
    '654a4f8bc721968a4404ef18': 'korund5side',
    '6557458f83942d705f0c4962': 'SSAPI5side',
    '656faf0ca0dce000a2020f77': 'SAPI6fb',
    '656fafe3498d1b7e3e071da4': 'KITECO6fb',
    '656fae5f7c2d57afe200c0d7': 'SAPI5fb',
}
vest_fits = {}  # vest name -> list of plate short names fitting
for tpl, it in d.items():
    slots = it.get('_props', {}).get('Slots', [])
    plate_slots = [s for s in slots if 'plate' in s.get('_name', '').lower()]
    if not plate_slots:
        continue
    fits = []
    for s in plate_slots:
        flt = s.get('_props', {}).get('filters', [{}])[0].get('Filter', [])
        for p in plates:
            if p in flt:
                fits.append(plates[p])
    vest_fits[it.get('_name', '?')] = fits

print('vest/plate-carriers with plate slots:', len(vest_fits))
c = Counter()
for fits in vest_fits.values():
    for x in set(fits):
        c[x] += 1
print()
print('=== plate fits N vests ===')
for p in plates.values():
    print(f'  {p}: {c[p]}')
print()
ranked = sorted(vest_fits.items(), key=lambda kv: -len(set(kv[1])))[:10]
print('=== top 10 vests by plate compat ===')
for name, fits in ranked:
    print(f'  {name}: {sorted(set(fits))}')
