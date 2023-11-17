import vectorbt as vbt
import yfinance as yf

# 获取历史股价数据
symbol = 'AAPL'
ohlcv = yf.download(symbol, start='2022-01-01', end='2023-01-01')

# 定义交易信号
entries = ohlcv['Close'] > ohlcv['Close'].rolling(50).mean()
exits = ohlcv['Close'] < ohlcv['Close'].rolling(50).mean()

# 创建VectorBT Portfolio对象
pf = vbt.Portfolio.from_signals(
    close=ohlcv['Close'],
    entries=entries,
    exits=exits,
    init_cash=10000
)

# 获取回测统计信息
stats = pf.stats()

# 输出回测结果
print(stats)


