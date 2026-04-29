#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""使用 wanx2.1-t2i-turbo 模型批量生成卡牌图片"""

import dashscope
from dashscope import ImageSynthesis
import requests
import os
import time

dashscope.api_key = "sk-197dbde89db14cafa99ca5bac696fcd2"

CARDS = [
    {"name": "Ice", "type": "item"},
    {"name": "Stone", "type": "item"},
    {"name": "Iron_ore", "type": "ore"},
    {"name": "Silicon_ore", "type": "ore"},
    {"name": "Magnesium_ore", "type": "ore"},
    {"name": "Nickel_ore", "type": "ore"},
    {"name": "Cobalt_ore", "type": "ore"},
    {"name": "Silver_ore", "type": "ore"},
    {"name": "Gold_ore", "type": "ore"},
    {"name": "Platinum_ore", "type": "ore"},
    {"name": "Uranium_ore", "type": "ore"},
    {"name": "Iron", "type": "item"},
    {"name": "Silicon", "type": "item"},
    {"name": "Magnesium", "type": "item"},
    {"name": "Nickel", "type": "item"},
    {"name": "Cobalt", "type": "item"},
    {"name": "Aluminum", "type": "item"},
    {"name": "Titanium", "type": "item"},
    {"name": "Copper", "type": "item"},
    {"name": "Zinc", "type": "item"},
]

ORE_COLORS = {
    "Iron_ore": "gray iron ore crystal",
    "Silicon_ore": "dark gray silicon crystal",
    "Magnesium_ore": "silver white magnesium ore",
    "Nickel_ore": "silver nickel ore",
    "Cobalt_ore": "deep blue cobalt ore",
    "Silver_ore": "shiny silver ore",
    "Gold_ore": "golden gold ore",
    "Platinum_ore": "platinum white ore",
    "Uranium_ore": "green glowing uranium ore",
}

FOLDER_MAP = {"item": "item", "ore": "ore"}

def gen_prompt(card):
    t = card["type"]
    n = card["name"]
    if t == "item":
        return f"pixel art icon of {n} game resource item, simple clean geometric shape, white border, steel blue background, 8-bit pixel style, minimalist game UI asset, square icon, retro video game style"
    elif t == "ore":
        c = ORE_COLORS.get(n, "mineral ore")
        return f"pixel art icon of {c}, rough crystal texture, game resource item, golden border, brown background, 8-bit retro pixel style, mining game asset, square icon"
    return f"pixel art game icon, 8-bit style"

base = "/root/project/godot/carddemo/CardImg/AIImg"
MODEL = "wanx2.1-t2i-turbo"

print(f"=== 使用 {MODEL} 模型生成 {len(CARDS)} 张卡牌 ===\n")

ok, fail = 0, 0

for i, card in enumerate(CARDS, 1):
    folder = FOLDER_MAP.get(card["type"], "item")
    out_dir = f"{base}/{folder}"
    os.makedirs(out_dir, exist_ok=True)
    path = f"{out_dir}/{card['name']}_wanx2.png"  # 新文件名避免覆盖
    
    prompt = gen_prompt(card)
    print(f"[{i}/20] {card['name']}...", flush=True)
    
    try:
        rsp = ImageSynthesis.call(model=MODEL, prompt=prompt, n=1, size="1024*1024")
        
        if rsp.output and rsp.output.task_status == "SUCCEEDED" and rsp.output.results:
            url = rsp.output.results[0].url
            r = requests.get(url, timeout=60)
            if r.status_code == 200:
                with open(path, 'wb') as f:
                    f.write(r.content)
                print(f"  ✓ OK -> {path} ({len(r.content)} bytes)")
                ok += 1
            else:
                print(f"  ✗ FAIL download (status {r.status_code})")
                fail += 1
        else:
            print(f"  ✗ FAIL task_status={getattr(rsp.output, 'task_status', 'N/A')}")
            fail += 1
    except Exception as e:
        print(f"  ✗ ERR {e}")
        fail += 1
    
    if i < len(CARDS):
        time.sleep(2)

print(f"\n=== Done! OK={ok}, Fail={fail} ===")
