using Io.Gate.GateApi.Api;
using Io.Gate.GateApi.Client;
using Io.Gate.GateApi.Model;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using TicTacTec.TA.Library;
using TANet.Core;
using TANet.Contracts.OperationResults.Indicators;
using Microsoft.AspNetCore.Http;

namespace AlSatBot.services
{
    public class RsiServices
    {
        public string RsiIndicator(string[] Coins, int rsi_buy_threshold, int rsi_sell_threshold, string hourPeriod, string apiKey, string SecretKey)
        {
            string _settle = "usdt";
            int rsi_period = 14;

            //string apiKey = "";
            //string SecretKey = "";
            string EndPoint = "https://api.gateio.ws/api/v4";
            string _timezone = "utc0";

            Configuration config = new Configuration();
            config.BasePath = EndPoint;
            config.SetGateApiV4KeyPair(apiKey, SecretKey); ;

            var apiInstance = new SpotApi(config);

            string data = "";
            foreach(string coin in Coins)
            {
                try
                {
                    List<List<string>> candles = apiInstance.ListCandlesticks(currencyPair: coin, limit: 100, interval: hourPeriod);

                    var closes = new decimal[candles.Count];
                    for (int i = 0; i < candles.Count; i++)
                    {
                        closes[i] = decimal.Parse(candles[i][2].ToString());
                    }

                    // NaN değerleri filtrele
                    var closes2 = new decimal[closes.Length];
                    int index = 0;
                    for (int i = 0; i < closes.Length; i++)
                    {
                        if (!IsDecimalNaN(closes[i]))
                        {
                            closes2[index] = closes[i];
                            index++;
                        }
                    }

                    // RSI hesapla

                    RsiResult rsi = Indicators.Rsi(closes2, rsi_period);
                    decimal[] rsiArray = rsi.Rsi;
                    var lastRsi = rsiArray[rsiArray.Length - 1];
                    
                    var ticker1 = apiInstance.ListTickers(coin, _timezone);
                    var price1 = ticker1[0];

                    data += $" {coin} icin RSI: {lastRsi} fiyat ise: {price1.Last}   #  ++";

                    // RSI alım fırsatı
                    if (lastRsi < rsi_buy_threshold)
                    {
                        // Anlık fiyatı al
                        var ticker = apiInstance.ListTickers(coin, _timezone);
                        var price = ticker[0];

                        // Buy Market order
                        data += $" {coin}{price.Last} ALIM SINYALI VERDI  # bg-success++";
                    }
                    // RSI satış fırsatı
                    else if (lastRsi > rsi_sell_threshold)
                    {
                        // Anlık fiyatı al
                        var ticker = apiInstance.ListTickers(coin, _timezone);
                        var price = ticker[0];

                        // Sell Market order
                        data += $" {coin}'i {price.Last} fiyat ile SATIM SINYALI VERDI # bg-danger++";
                    }

                }
                catch(Exception e)
                {
                    //return "Error occured while trading "+ coin + ":" + e;
                    return "Bot başlatılırken bir hata oluştu lütfen şunlara dikkat edin" +
                        " 1) coin isimlerinin düzgün yazıldığına," +
                        " 2) araya sadece virgül konulmasına, dikkat edin";
                }
            }

            
            return data;
        }
        private static bool IsDecimalNaN(decimal value)
        {
            return value == decimal.MinValue || value == decimal.MaxValue;
        }
    }
}
