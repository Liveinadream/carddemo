#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""参考原有风格重新生成前 5 张卡牌"""

import dashscope
from dashscope import ImageSynthesis
import requests
import os
import time

dashscope.api_key = "sk-197dbde89db14cafa99ca5bac696fcd2"

# 前 5 张卡牌
CARDS = [
    {"name": "Ice", "type": "item", "display": "冰"},
    {"name": "Stone", "type": "item", "display": "石头"},
    {"name": "Iron_ore", "type": "ore", "display": "铁矿"},
    {"name": "Silicon_ore", "type": "ore", "display": "硅矿"},
    {"name": "Magnesium_ore", "type": "ore", "display": "镁矿"},
]

# 参考原有 Ice.png 的风格描述
# 64x64 像素，像素风格，带有边框和图标
STYLE_PROMPT = "pixel art game card icon, 64x64 pixel style, simple geometric shape, clean design, solid color background, white border, retro video game resource icon, minimalist pixel art"

ORE_COLORS = {
    "Iron_ore": "gray iron ore crystal",
    "Silicon_ore": "dark gray silicon crystal", 
    "Magnesium_ore": "silver white magnesium ore",
}

def gen_prompt(card):
    t = card["type"]
    n = card["name"]
    if t == "item":
        # 参考 Ice.png 风格：蓝色背景，白色边框，简单几何形状
        return f"pixel art icon of {n} ice cube game resource, light blue and white colors, simple geometric crystal shape, steel blue background, white border, 64x64 pixel art style, clean minimalist design, retro game item icon"
    elif t == "ore":
        c = ORE_COLORS.get(n, "mineral ore")
        # 矿石风格：棕色背景，金色边框，粗糙晶体
        return f"pixel art icon of {c}, rough crystal texture, brown earth background, golden border, 64x64 pixel art style, retro mining game resource icon, simple geometric shape"
    return f"pixel art game icon, 64x64 style"

base = "/root/project/godot/carddemo/CardImg/AIImg"
MODEL = "wanx2.1-t2i-turbo"

print(f"=== 参考原有风格重新生成前 5 张卡牌 ===")
print(f"模型：{MODEL}")
print(f"参考图片：/root/project/godot/carddemo/CardImg/Ice.png (64x64 像素风格)")
print()

ok, fail = 0, 0

for i, card in enumerate(CARDS, 1):
    folder = "item" if card["type"] == "item" else "ore"
    out_dir = f"{base}/{folder}"
    os.makedirs(out_dir, exist_ok=True)
    # 使用 v2 后缀避免覆盖
    path = f"{out_dir}/{card['name']}_v2.png"
    
    prompt = gen_prompt(card)
    print(f"[{i}/5] {card['display']} ({card['name']})...")
    print(f"    提示词：{prompt[:80]}...")
    
    try:
        rsp = ImageSynthesis.call(model=MODEL, prompt=prompt, n=1, size="1024*1024")
        
        if rsp.output and rsp.output.task_status == "SUCCEEDED" and rsp.output.results:
            url = rsp.output.results[0].url
            
            # 下载原图
            r = requests.get(url, timeout=60)
            if r.status_code == 200:
                # 保存 1024x1024 原图
                orig_path = f"{out_dir}/{card['name']}_v2_orig.png"
                with open(orig_path, 'wb') as f:
                    f.write(r.content)
                
                # 缩放到 64x64
                from PIL import Image
                img = Image.open(orig_path)
                resized = img.resize((64, 64), Image.LANCZOS)
                resized.save(path, "PNG")
                
                print(f"    ✓ 已生成：{path} (64x64)")
                print(f"    原图：{orig_path} ({len(r.content)} bytes)")
                ok += 1
            else:
                print(f"    ✗ 下载失败")
                fail += 1
        else:
            print(f"    ✗ 生成失败")
            fail += 1
    except Exception as e:
        print(f"    ✗ 错误：{e}")
        fail += 1
    
    if i < len(CARDS):
        time.sleep(2)

print(f"\n=== 完成！成功：{ok}, 失败：{fail} ===")
print(f"\n生成的图片位置：")
for card in CARDS:
    folder = "item" if card["type"] == "item" else "ore"
    print(f"  {base}/{folder}/{card['name']}_v2.png")
