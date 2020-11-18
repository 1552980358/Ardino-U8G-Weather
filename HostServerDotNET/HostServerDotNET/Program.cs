using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Threading;

namespace HostServerDotNET
{
    internal static class Program
    {
        private const string ApiKey = "";

        public static void Main()
        {
            // ReSharper disable once TooWideLocalVariableScope
            WebRequest webRequest;
            // ReSharper disable once TooWideLocalVariableScope
            Stream stream;
            // ReSharper disable once TooWideLocalVariableScope
            string jsonMsg;
            // ReSharper disable once TooWideLocalVariableScope
            SerialPort serialPort;

            while (true)
            {
                try
                {
                    // 连接到 https://openweathermap.org/ 的API
                    webRequest = (HttpWebRequest)
                        WebRequest.Create(
                            "http://api.openweathermap.org/data/2.5/weather?q=Mongkok,hk&units=metric&APPID=" + ApiKey);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    webRequest = null;
                }

                if (webRequest == null)
                {
                    continue;
                }

                try
                {
                    stream = webRequest.GetResponse().GetResponseStream();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    stream = null;
                }

                if (stream == null)
                {
                    continue;
                }

                jsonMsg = ReadStream(stream);
                if (jsonMsg == "")
                {
                    continue;
                }

                string tmp = jsonMsg.Substring(jsonMsg.IndexOf("\"temp\":", StringComparison.Ordinal) + 7);
                var temp = tmp.Substring(0, tmp.IndexOf(','));

                if (temp.Length > 2)
                {
                    temp = temp[3] >= '5'
                        ? (
                            temp[1] == '9'
                                ? (temp[0] + 1).ToString() + '0'
                                : temp[0].ToString() + (char) (temp[1] + 1)
                        )
                        : temp.Substring(0, 2);
                }

                tmp = jsonMsg.Substring(jsonMsg.IndexOf("\"temp_min\"", StringComparison.Ordinal) + 11);
                var tempMin = tmp.Substring(0, tmp.IndexOf(','));
                if (tempMin.Length > 2)
                {
                    tempMin = tempMin[3] >= '5'
                        ? (
                            tempMin[1] == '9'
                                ? (tempMin[0] + 1).ToString() + '0'
                                : tempMin[0].ToString() + (char) (temp[1] + 1)
                        )
                        : tempMin.Substring(0, 2);
                }

                tmp = jsonMsg.Substring(jsonMsg.IndexOf("\"temp_max\"", StringComparison.Ordinal) + 11);
                var tempMax = tmp.Substring(0, tmp.IndexOf(','));
                if (tempMax.Length > 2)
                {
                    tempMax = tempMax[3] >= '5'
                        ? (
                            tempMax[1] == '9'
                                ? (tempMax[0] + 1).ToString() + '0'
                                : tempMax[0].ToString() + (char) (temp[1] + 1)
                        )
                        : tempMax.Substring(0, 2);
                }

                tmp = jsonMsg.Substring(jsonMsg.IndexOf("\"pressure\"", StringComparison.Ordinal) + 11);
                var pressure = tmp.Substring(0, tmp.IndexOf(','));

                tmp = jsonMsg.Substring(jsonMsg.IndexOf("\"humidity\"", StringComparison.Ordinal) + 11);
                var humidity = tmp.Substring(0, tmp.IndexOf('}'));
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                serialPort.Open();
                serialPort.Write(
                    temp + ';' +
                    tempMin + ';' +
                    tempMax + ';' +
                    pressure + ';' +
                    humidity + ';' +
                    time
                );

                Console.WriteLine(
                    "temp: " + temp + '\n' +
                    "temp_min: " + tempMin + '\n' +
                    "temp_max: " + tempMax + '\n' +
                    "pressure: " + pressure + '\n' +
                    "humidity: " + humidity + '\n' +
                    "time: " + time + '\n' +
                    "===================="
                );

                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());

                serialPort.Close();

                Thread.Sleep(500);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        static string ReadStream(Stream stream)
        {
            var streamContent = "";
            try
            {
                var streamReader = new StreamReader(stream);
                streamContent = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return streamContent;
        }
    }
}