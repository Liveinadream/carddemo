#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""批量缩放卡牌图片到指定尺寸"""

from PIL import Image
import os

base = "/root/project/godot/carddemo/CardImg/AIImg"

# 类型对应的目标尺寸
SIZE_MAP = {
    "item": (64, 64),
    "ore": (64, 64),
    "site": (256, 128),
    "npc": (57, 57),
    "food": (64, 64),
    "damage": (64, 64),
}

# 处理 wanx2 生成的图片
folders = ["item", "ore"]

print("=== 缩放 wanx2 生成的卡牌图片 ===\n")

total, ok, fail = 0, 0, 0

for folder in folders:
    target_size = SIZE_MAP.get(folder, (64, 64))
    src_dir = f"{base}/{folder}"
    
    if not os.path.exists(src_dir):
        print(f"跳过 {folder}: 目录不存在")
        continue
    
    # 找到所有 _wanx2.png 文件
    files = [f for f in os.listdir(src_dir) if f.endswith("_wanx2.png")]
    
    if not files:
        print(f"跳过 {folder}: 没有 _wanx2.png 文件")
        continue
    
    print(f"\n处理 {folder} 文件夹 ({len(files)} 张图片) -> 目标尺寸：{target_size[0]}x{target_size[1]}")
    
    for filename in files:
        src_path = f"{src_dir}/{filename}"
        # 输出文件名：去掉 _wanx2 后缀
        base_name = filename.replace("_wanx2.png", ".png")
        dst_path = f"{src_dir}/{base_name}"
        
        total += 1
        try:
            img = Image.open(src_path)
            # 使用 LANCZOS 高质量缩放
            resized = img.resize(target_size, Image.LANCZOS)
            resized.save(dst_path, "PNG")
            print(f"  ✓ {filename} -> {base_name} ({img.size[0]}x{img.size[1]} -> {target_size[0]}x{target_size[1]})")
            ok += 1
        except Exception as e:
            print(f"  ✗ {filename} 失败：{e}")
            fail += 1

print(f"\n=== 完成！总计：{total}, 成功：{ok}, 失败：{fail} ===")

# 列出最终结果
print("\n=== 最终文件列表 ===")
for folder in folders:
    dir_path = f"{base}/{folder}"
    if os.path.exists(dir_path):
        files = sorted([f for f in os.listdir(dir_path) if f.endswith(".png") and not f.endswith("_wanx2.png")])
        print(f"\n{folder}/ ({len(files)} 张):")
        for f in files[:10]:
            path = f"{dir_path}/{f}"
            size = os.path.getsize(path)
            print(f"  - {f} ({size} bytes)")
        if len(files) > 10:
            print(f"  ... 还有 {len(files) - 10} 张")
