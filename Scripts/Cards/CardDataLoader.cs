using Godot;
using System;
using System.Collections.Generic;

/*
 * 卡牌数据加载器类
 * 负责从CSV文件中加载卡牌数据并转换为CardData对象列表
 * 
 * 主要功能:
 * - 读取CSV格式的卡牌数据文件
 * - 解析CSV数据并创建对应的CardData对象
 * - 处理文件不存在、格式错误等异常情况
 * 
 * 使用方法:
 * 1. 创建CardDataLoader实例
 * 2. 调用LoadCardsFromCsv方法并传入CSV文件路径
 * 3. 获取返回的CardData对象列表进行使用
 */
public class CardDataLoader
{
    private const string CSV_DELIMITER = ",";
    private static readonly int CSV_LINE = 11;
    private static readonly int CARD_NAME = 0;
    private static readonly int CARD_INDEX = 1;
    private static readonly int CARD_DISPLAY = 2;
    private static readonly int CARD_TYPE = 3;
    private static readonly int CARD_DESCRIPTION = 4;
    private static readonly int CARD_PRICE = 5;
    private static readonly int CARD_WEIGHT = 6;
    private static readonly int CARD_MAXSTACK = 7;
    private static readonly int CARD_SITEAREA = 8;
    private static readonly int CARD_NPCSCHEDULE = 9;
    private static readonly int CARD_FOODHP = 10;



    public static List<CardData> LoadCardsFromCsv(string csvFilePath)
    {
        List<CardData> cardDataList = [];

        try
        {
            // 使用 Godot 的 FileAccess 检查文件是否存在
            if (FileAccess.FileExists(csvFilePath))
            {
                GD.Print($"File exists: {csvFilePath}");
                // 使用 Godot 的 FileAccess 读取文件
                using FileAccess file = FileAccess.Open(csvFilePath, FileAccess.ModeFlags.Read);
                // 跳过标题行
                string headerLine = file.GetLine();

                // 读取剩余行
                while (file.GetPosition() < file.GetLength())
                {
                    string line = file.GetLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(CSV_DELIMITER);

                    if (parts.Length >= CSV_LINE)
                    {
                        try
                        {
                            string baseCardName = parts[CARD_NAME];
                            int index = int.Parse(parts[CARD_INDEX]);
                            string baseDisplay = parts[CARD_DISPLAY];
                            string baseCardType = parts[CARD_TYPE];
                            string baseDescription = parts[CARD_DESCRIPTION];
                            string basePrice = parts[CARD_PRICE];
                            int weight = int.Parse(parts[CARD_WEIGHT]);
                            int baseMaxStack = int.Parse(parts[CARD_MAXSTACK]);
                            int siteArea = int.Parse(parts[CARD_SITEAREA]);
                            int npcSchedule = int.Parse(parts[CARD_NPCSCHEDULE]);
                            int foodHP = int.Parse(parts[CARD_FOODHP]);

                            CardData cardData = new(
                                baseCardName, index, baseDisplay, baseCardType,
                                baseDescription,weight, basePrice, baseMaxStack,
                                siteArea, npcSchedule, foodHP
                            );
                            
                            cardDataList.Add(cardData);
                        }
                        catch (Exception)
                        {
                            GD.PrintErr($"Error parsing card data: {line}");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error parsing card data: {e.Message}");
        }

        GD.Print($"Loaded {cardDataList.Count} cards from {csvFilePath} , cardInfo {cardDataList}");

        return cardDataList;
    }
}