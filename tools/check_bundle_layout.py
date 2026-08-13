# -*- coding: utf-8 -*-
"""Check rig layout bundle: root prefab RectTransform sizeDelta + LayoutElement minHeight."""
import sys
import UnityPy

def dump(path):
    env = UnityPy.load(path)
    for obj in env.objects:
        if obj.type.name == 'GameObject':
            data = obj.read()
            print(f"  GameObject: {data.m_Name}")
            # find RectTransform component of root (no parent)
        if obj.type.name == 'RectTransform':
            data = obj.read()
            f = data.m_Father
            sd = data.m_SizeDelta
            ap = data.m_AnchoredPosition
            g = data.m_GameObject
            parentless = (f.m_PathID == 0)
            print(f"  RectTransform: sizeDelta=({sd.x:.1f},{sd.y:.1f}) pos=({ap.x:.1f},{ap.y:.1f}) parentless={parentless}")
        if obj.type.name == 'MonoBehaviour':
            data = obj.read()
            try:
                t = data.m_Script.read() if data.m_Script else None
            except Exception:
                t = None
            name = getattr(t, 'm_Name', '?') if t else '?'
            if 'LayoutElement' in name:
                # parse fields
                print(f"  MonoBehaviour LayoutElement: minW={data.m_MinWidth} minH={data.m_MinHeight} (gameObject {data.m_GameObject.m_PathID})")
            elif 'GridsView' in name or 'GridsView' in name:
                print(f"  MonoBehaviour {name}: gameObject {data.m_GameObject.m_PathID}")

for p in sys.argv[1:]:
    print(f"===== {p} =====")
    dump(p)
