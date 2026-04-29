#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
生成像素风格卡牌图片
- 资源卡牌 (item/ore): 64x64
- 人物卡牌 (npc): 57x57 (上半身)
- 地区卡牌 (site): 256x128
- 食物卡牌 (food): 64x64
- 伤害卡牌 (damage): 64x64
"""

from PIL import Image, ImageDraw, ImageFont
import os
import csv

# 卡牌数据 (前 20 张)
CARDS = [
    {"name": "Ice", "display": "冰", "type": "item", "desc": "降温用，临时冰淇淋替代品。"},
    {"name": "Stone", "display": "石头", "type": "item", "desc": "基础建材，随处可见。"},
    {"name": "Iron_ore", "display": "铁矿", "type": "ore", "desc": "工业基础，加工后成铁。"},
    {"name": "Silicon_ore", "display": "硅矿", "type": "ore", "desc": "芯片原料，科技核心。"},
    {"name": "Magnesium_ore", "display": "镁矿", "type": "ore", "desc": "轻质合金材料。"},
    {"name": "Nickel_ore", "display": "镍矿", "type": "ore", "desc": "增强装备韧性。"},
    {"name": "Cobalt_ore", "display": "钴矿", "type": "ore", "desc": "电池关键材料。"},
    {"name": "Silver_ore", "display": "银矿", "type": "ore", "desc": "导电性极佳。"},
    {"name": "Gold_ore", "display": "金矿", "type": "ore", "desc": "奢侈品兼货币。"},
    {"name": "Platinum_ore", "display": "铂矿", "type": "ore", "desc": "稀有贵金属。"},
    {"name": "Uranium_ore", "display": "铀矿", "type": "ore", "desc": "高危高能，谨慎使用。"},
    {"name": "Iron", "display": "铁", "type": "item", "desc": "基础金属，用途广泛。"},
    {"name": "Silicon", "display": "硅", "type": "item", "desc": "半导体核心。"},
    {"name": "Magnesium", "display": "镁", "type": "item", "desc": "轻量化首选。"},
    {"name": "Nickel", "display": "镍", "type": "item", "desc": "合金强化剂。"},
    {"name": "Cobalt", "display": "钴", "type": "item", "desc": "高性能电池。"},
    {"name": "Aluminum", "display": "铝", "type": "item", "desc": "轻质结构材料。"},
    {"name": "Titanium", "display": "钛", "type": "item", "desc": "强度重量比最优。"},
    {"name": "Copper", "display": "铜", "type": "item", "desc": "导电王者。"},
    {"name": "Zinc", "display": "锌", "type": "item", "desc": "防腐蚀涂层。"},
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
    "item": (64, 64),
    "ore": (64, 64),
    "site": (256, 128),
    "npc": (57, 57),
    "food": (64, 64),
    "damage": (64, 64),
}

# 像素风格颜色 palette
COLORS = {
    "item": {
        "bg": (70, 130, 180),      # 钢蓝色背景
        "border": (255, 255, 255),  # 白色边框
        "icon": (200, 220, 255),    # 浅色图标
        "shadow": (40, 80, 120),    # 深色阴影
    },
    "ore": {
        "bg": (139, 90, 43),        # 棕色背景
        "border": (255, 215, 0),    # 金色边框
        "icon": (205, 133, 63),     # 巧克力色图标
        "shadow": (80, 50, 20),     # 深棕色阴影
    },
    "site": {
        "bg": (47, 79, 79),         # 深青色背景
        "border": (100, 149, 237),  # 矢车菊蓝边框
        "icon": (176, 196, 222),    # 浅钢蓝色图标
        "shadow": (25, 25, 112),    # 午夜蓝阴影
    },
    "npc": {
        "bg": (148, 0, 211),        # 紫色背景
        "border": (255, 105, 180),  # 亮粉色边框
        "icon": (221, 160, 221),    # 梅红色图标
        "shadow": (75, 0, 130),     # 靛蓝色阴影
    },
    "food": {
        "bg": (255, 140, 0),        # 暗橙色背景
        "border": (255, 255, 255),  # 白色边框
        "icon": (255, 165, 0),      # 橙色图标
        "shadow": (178, 34, 34),    # 火砖红色阴影
    },
    "damage": {
        "bg": (178, 34, 34),        # 火砖红色背景
        "border": (255, 69, 0),     # 橙红色边框
        "icon": (255, 99, 71),      # 番茄红色图标
        "shadow": (139, 0, 0),      # 暗红色阴影
    },
}

def get_size(card_type):
    """获取卡牌尺寸"""
    return TYPE_SIZE_MAP.get(card_type, (64, 64))

def get_folder(card_type):
    """获取文件夹名称"""
    return TYPE_FOLDER_MAP.get(card_type, "item")

def draw_pixel_rect(draw, x, y, w, h, color):
    """绘制像素风格的矩形"""
    draw.rectangle([x, y, x + w - 1, y + h - 1], fill=color)

def draw_pixel_border(draw, width, height, border_color, thickness=2):
    """绘制像素风格边框"""
    # 上边框
    draw.rectangle([0, 0, width - 1, thickness - 1], fill=border_color)
    # 下边框
    draw.rectangle([0, height - thickness, width - 1, height - 1], fill=border_color)
    # 左边框
    draw.rectangle([0, 0, thickness - 1, height - 1], fill=border_color)
    # 右边框
    draw.rectangle([width - thickness, 0, width - 1, height - 1], fill=border_color)

def draw_resource_icon(draw, card_type, width, height, colors):
    """绘制资源类图标 (item/ore)"""
    center_x, center_y = width // 2, height // 2
    icon_size = width // 3
    
    # 绘制一个像素风格的宝石/矿石形状
    # 主菱形
    points = [
        (center_x, center_y - icon_size),  # 顶
        (center_x + icon_size, center_y),   # 右
        (center_x, center_y + icon_size),   # 底
        (center_x - icon_size, center_y),   # 左
    ]
    draw.polygon(points, fill=colors["icon"])
    
    # 添加高光
    highlight_size = icon_size // 3
    draw.rectangle([
        center_x - highlight_size, 
        center_y - icon_size + 2,
        center_x, 
        center_y - icon_size // 2
    ], fill=(255, 255, 255))

def draw_site_icon(draw, width, height, colors):
    """绘制地区图标 (site)"""
    # 绘制一个简单的建筑轮廓
    margin = 20
    building_width = width - margin * 2
    building_height = height - margin * 2 - 20
    
    # 主体建筑
    draw.rectangle([
        margin, height - margin - building_height,
        width - margin, height - margin
    ], fill=colors["icon"])
    
    # 屋顶
    roof_points = [
        (margin - 5, height - margin - building_height),
        (width // 2, height - margin - building_height - 30),
        (width - margin + 5, height - margin - building_height),
    ]
    draw.polygon(roof_points, fill=colors["border"])
    
    # 窗户
    window_size = 15
    window_spacing = 35
    for i in range(3):
        for j in range(2):
            wx = margin + 25 + i * window_spacing
            wy = height - margin - building_height + 20 + j * 30
            draw.rectangle([wx, wy, wx + window_size, wy + window_size], fill=(100, 149, 237))

def draw_npc_icon(draw, width, height, colors):
    """绘制人物图标 (npc) - 上半身"""
    center_x = width // 2
    head_radius = width // 5
    shoulder_width = width // 2
    body_height = height // 3
    
    # 头部 (圆形)
    draw.ellipse([
        center_x - head_radius,
        height // 3 - head_radius,
        center_x + head_radius,
        height // 3 + head_radius
    ], fill=colors["icon"])
    
    # 身体 (梯形)
    body_top = height // 3 + head_radius
    body_bottom = body_top + body_height
    draw.polygon([
        (center_x - shoulder_width // 2, body_top),
        (center_x + shoulder_width // 2, body_top),
        (center_x + shoulder_width, body_bottom),
        (center_x - shoulder_width, body_bottom),
    ], fill=colors["shadow"])

def draw_food_icon(draw, width, height, colors):
    """绘制食物图标"""
    center_x, center_y = width // 2, height // 2
    size = width // 3
    
    # 绘制一个碗/盘子形状
    draw.arc([
        center_x - size, center_y - size // 2,
        center_x + size, center_y + size
    ], 0, 180, fill=colors["icon"], width=4)
    draw.line([
        center_x - size, center_y - size // 2,
        center_x + size, center_y - size // 2
    ], fill=colors["icon"], width=4)
    
    # 食物内容
    draw.ellipse([
        center_x - size // 2, center_y - size // 3,
        center_x + size // 2, center_y + size // 4
    ], fill=colors["border"])

def draw_damage_icon(draw, width, height, colors):
    """绘制伤害图标"""
    center_x, center_y = width // 2, height // 2
    size = width // 3
    
    # 绘制剑/武器形状
    # 剑刃
    draw.polygon([
        (center_x, center_y - size),
        (center_x + 5, center_y - size // 2),
        (center_x + 5, center_y + size // 2),
        (center_x, center_y + size),
        (center_x - 5, center_y + size // 2),
        (center_x - 5, center_y - size // 2),
    ], fill=colors["icon"])
    
    # 剑柄
    draw.rectangle([
        center_x - 8, center_y + size // 2,
        center_x + 8, center_y + size // 2 + 5
    ], fill=colors["border"])

def generate_card_image(card, output_path):
    """生成单张卡牌图片"""
    card_type = card["type"]
    width, height = get_size(card_type)
    colors = COLORS.get(card_type, COLORS["item"])
    
    # 创建图像
    img = Image.new('RGB', (width, height), colors["bg"])
    draw = ImageDraw.Draw(img)
    
    # 绘制边框
    draw_pixel_border(draw, width, height, colors["border"], thickness=3)
    
    # 绘制阴影效果 (内阴影)
    shadow_rect = [4, 4, width - 5, height - 5]
    draw.rectangle(shadow_rect, outline=colors["shadow"])
    
    # 根据类型绘制图标
    if card_type in ["item", "ore"]:
        draw_resource_icon(draw, card_type, width, height, colors)
    elif card_type == "site":
        draw_site_icon(draw, width, height, colors)
    elif card_type == "npc":
        draw_npc_icon(draw, width, height, colors)
    elif card_type == "food":
        draw_food_icon(draw, width, height, colors)
    elif card_type == "damage":
        draw_damage_icon(draw, width, height, colors)
    
    # 保存图像
    img.save(output_path, 'PNG')
    print(f"✓ 已生成：{output_path}")

def main():
    base_dir = "/root/project/godot/carddemo/CardImg/AIImg"
    
    print(f"开始生成 {len(CARDS)} 张卡牌图片...")
    print("=" * 50)
    
    for card in CARDS:
        folder = get_folder(card["type"])
        output_dir = os.path.join(base_dir, folder)
        os.makedirs(output_dir, exist_ok=True)
        
        # 文件名使用英文名
        filename = f"{card['name']}.png"
        output_path = os.path.join(output_dir, filename)
        
        generate_card_image(card, output_path)
    
    print("=" * 50)
    print(f"完成！共生成 {len(CARDS)} 张卡牌图片")
    print(f"输出目录：{base_dir}")

if __name__ == "__main__":
    main()
