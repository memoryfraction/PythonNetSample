import pandas as pd
import numpy as np
import json


pairTradingAnalysisResultList = [
    {
        "UnderlyingPair": {"Symbol1": "BTC/USD", "Symbol2": "ETH/USD"},
        "CorrelationValue": 0.9,
        "IsCorrelated": True,
        "IsStationary": True,
        "IsNormalDistributed": True,
        "DiffEquation": "y = mx + b",
        "Slope": 0.5,
        "Intercept": 1.2,
        "StartDt": "2024-10-31",
        "EndDt": "2025-10-31",
        "ResolutionLevel": "1h"
    },
    {
        "UnderlyingPair": {"Symbol1": "XRP/USD", "Symbol2": "LTC/USD"},
        "CorrelationValue": 0.85,
        "IsCorrelated": True,
        "IsStationary": False,
        "IsNormalDistributed": False,
        "DiffEquation": "y = mx + b",
        "Slope": 0.6,
        "Intercept": 1.1,
        "StartDt": "2024-10-31",
        "EndDt": "2025-10-31",
        "ResolutionLevel": "1h"
    }
]


def calculate_zscore(price_data_1, price_data_2, window=91):
    """
    计算 Z-Score
    :param price_data_1: 第一个资产的价格数据
    :param price_data_2: 第二个资产的价格数据
    :param window: 滚动窗口大小，默认 30
    :return: 计算出的 Z-Score
    """
    # 计算价格差异
    price_diff = price_data_1 - price_data_2

    # 计算滚动均值和标准差
    rolling_mean = price_diff.rolling(window=window, min_periods=1).mean()
    rolling_std = price_diff.rolling(window=window, min_periods=1).std()

    # 计算 Z-Score
    zscore = (price_diff - rolling_mean) / rolling_std
    return zscore


def process_pair_trading_data(pair_trading_results):
    all_zscores = []

    # 遍历每一个配对
    for result in pair_trading_results:
        symbol1 = result['UnderlyingPair']['Symbol1']
        symbol2 = result['UnderlyingPair']['Symbol2']

        # 假设我们已经加载了两个资产的价格数据
        # 在这里我们用随机生成的数据作为示例，实际使用时需要从文件或API获取真实的价格数据
        price_data_1 = pd.Series(np.random.random(100))  # 模拟的现货价格数据
        price_data_2 = pd.Series(np.random.random(100))  # 模拟的期货价格数据

        # 计算 Z-Score
        zscore = calculate_zscore(price_data_1, price_data_2)

        # 获取 Z-Score 的最大值和最小值作为 `top_zscore` 和 `bottom_zscore`
        bottom_zscore = zscore.min()
        top_zscore = zscore.max()

        # 保存每个配对的 Z-Score 最大值和最小值
        all_zscores.append({
            'Symbol1': symbol1,
            'Symbol2': symbol2,
            'bottom_zscore': bottom_zscore,
            'top_zscore': top_zscore
        })

    return all_zscores


# 计算 Z-Score 阈值
zscore_result = process_pair_trading_data(pairTradingAnalysisResultList)

# 打印最终的 Z-Score 阈值
for result in zscore_result:
    print(f"Pair: {result['Symbol1']} / {result['Symbol2']}")
    print(f"Bottom Z-Score: {result['bottom_zscore']}")
    print(f"Top Z-Score: {result['top_zscore']}")
    print("-----------------------------")
