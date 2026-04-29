#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
使用 wanx-v1 模型生成像素风格卡牌图片
"""

import dashscope
from dashscope import ImageSynthesis
import os
import time
import requests

# 设置 API KEY
dashscope.api_key = "sk-197dbde89db14cafa99ca5bac696fcd2"

# 卡牌数据 (前 20 张)
CARDS = [
    {"name": "Ice", "display": "冰", "type": "item"},
    {"name": "Stone", "display": "石头", "type": "item"},
    {"name": "Iron_ore", "display": "铁矿", "type": "ore"},
    {"name": "Silicon_ore", "display": "硅矿", "type": "ore"},
    {"name": "Magnesium_ore", "display": "镁矿", "type": "ore"},
    {"name": "Nickel_ore", "display": "镍矿", "type": "ore"},
    {"name": "Cobalt_ore", "display": "钴矿", "type": "ore"},
    {"name": "Silver_ore", "display": "银矿", "type": "ore"},
    {"name": "Gold_ore", "display": "金矿", "type": "ore"},
    {"name": "Platinum_ore", "display": "铂矿", "type": "ore"},
    {"name": "Uranium_ore", "display": "铀矿", "type": "ore"},
    {"name": "Iron", "display": "铁", "type": "item"},
    {"name": "Silicon", "display": "硅", "type": "item"},
    {"name": "Magnesium", "display": "镁", "type": "item"},
    {"name": "Nickel", "display": "镍", "type": "item"},
    {"name": "Cobalt", "display": "钴", "type": "item"},
    {"name": "Aluminum", "display": "铝", "type": "item"},
    {"name": "Titanium", "display": "钛", "type": "item"},
    {"name": "Copper", "display": "铜", "type": "item"},
    {"name": "Zinc", "display": "锌", "type": "item"},
]

# 类型到文件夹的映射
TYPE_FOLDER_MAP = {
    "item": "item",
    "ore": "ore",
    "site": "site",
    "npc": "npc",
    "food": "food",
    "damage": "damage",
}

# 类型到尺寸的映射
TYPE_SIZE_MAP = {
    "item": "1024*1024",
    "ore": "1024*1024",
    "site": "1024*1024",
    "npc": "1024*1024",
    "food": "1024*1024",
    "damage": "1024*1024",
}

# 矿石颜色映射
ORE_COLORS = {
    "Iron_ore": "gray iron ore crystal",
    "Silicon_ore": "dark gray silicon crystal with metallic shine",
    "Magnesium_ore": "silver white magnesium ore",
    "Nickel_ore": "silver nickel ore with metallic luster",
    "Cobalt_ore": "deep blue cobalt ore crystal",
    "Silver_ore": "shiny silver ore with bright reflection",
    "Gold_ore": "golden gold ore with yellow sparkle",
    "Platinum_ore": "platinum white precious ore",
    "Uranium_ore": "green glowing radioactive uranium ore",
}

def get_size(card_type):
    return TYPE_SIZE_MAP.get(card_type, "1024*1024")

def get_folder(card_type):
    return TYPE_FOLDER_MAP.get(card_type, "item")

def generate_prompt(card):
    """生成图片提示词"""
    card_type = card["type"]
    name = card["name"]
    
    if card_type == "item":
        prompt = f"pixel art icon of {name} game resource item, simple clean geometric shape, white border, steel blue background, 8-bit pixel style, minimalist game UI asset, square icon, retro video game style"
    elif card_type == "ore":
        color_desc = ORE_COLORS.get(name, "mineral ore crystal")
        prompt = f"pixel art icon of {color_desc}, rough crystal texture, game resource item, golden border, brown earth background, 8-bit retro pixel style, mining game asset, square icon"
    elif card_type == "site":
        prompt = f"pixel art sci-fi space station building location, detailed architecture, blue border, dark teal background, 16-bit pixel style, game map location"
    elif card_type == "npc":
        prompt = f"pixel art character portrait upper body, anime style game NPC, purple border, 16-bit pixel style, RPG game character"
    elif card_type == "food":
        prompt = f"pixel art food item, consumable game prop, orange border, 8-bit pixel style, game inventory item"
    elif card_type == "damage":
        prompt = f"pixel art weapon attack skill icon, red border, fire effect, game ability symbol, 8-bit pixel style"
    else:
        prompt = f"pixel art game icon, 8-bit style, game asset"
    
    return prompt

def download_image(url, output_path):
    """下载图片"""
    try:
        response = requests.get(url, timeout=60)
        if response.status_code == 200:
            with open(output_path, 'wb') as f:
                f.write(response.content)
            return True
        return False
    except Exception as e:
        print(f"下载错误：{e}")
        return False

def generate_card_image(card, output_path):
    """使用 wanx-v1 生成单张卡牌图片"""
    card_type = card["type"]
    size = get_size(card_type)
    prompt = generate_prompt(card)
    
    print(f"正在生成：{card['display']} ({card['name']})")
    
    try:
        # 调用 wanx-v1 模型
        rsp = ImageSynthesis.call(
            model="wanx-v1",
            prompt=prompt,
            n=1,
            size=size,
        )
        
        if rsp.status_code == 200 and rsp.output and rsp.output.results:
            img_url = rsp.output.results[0].url
            print(f"  获取到图片 URL")
            
            if download_image(img_url, output_path):
                print(f"  ✓ 已保存：{output_path}")
                return True
            else:
                print(f"  ✗ 下载失败")
                return False
        else:
            print(f"  ✗ 生成失败：{rsp.message if hasattr(rsp, 'message') else 'unknown'}")
            return False
            
    except Exception as e:
        print(f"  ✗ 错误：{str(e)}")
        return False

def main():
    base_dir = "/root/project/godot/carddemo/CardImg/AIImg"
    
    print("=" * 60)
    print(f"开始使用 wanx-v1 模型生成 {len(CARDS)} 张卡牌图片...")
    print("=" * 60)
    
    success_count = 0
    fail_count = 0
    
    for i, card in enumerate(CARDS, 1):
        folder = get_folder(card["type"])
        output_dir = os.path.join(base_dir, folder)
        os.makedirs(output_dir, exist_ok=True)
        
        filename = f"{card['name']}.png"
        output_path = os.path.join(output_dir, filename)
        
        if generate_card_image(card, output_path):
            success_count += 1
        else:
            fail_count += 1
        
        # 添加延迟避免 API 限流
        if i < len(CARDS):
            print(f"  等待 3 秒...")
            time.sleep(3)
    
    print("=" * 60)
    print(f"完成！成功：{success_count}, 失败：{fail_count}")
    print(f"输出目录：{base_dir}")

if __name__ == "__main__":
    main()
