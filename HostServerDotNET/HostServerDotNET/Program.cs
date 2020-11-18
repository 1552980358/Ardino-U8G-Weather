using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Threading;

namespace HostServerDotNET
{
    internal static class Program
    {
        private const string ApiKey = "de815d60d8077177e2f482ab4f5b4a7a";

        public static void Main()
        {
            // ReSharper disable once TooWideLocalVariableScope
            WebRequest webRequest;
            // ReSharper disable once TooWideLocalVariableScope
            Stream stream;
            // ReSharper disable once TooWideLocalVariableScope
            string msg;
            // ReSharper disable once TooWideLocalVariableScope
            SerialPort serialPort = null;

            while (true)
            {
                Console.WriteLine("Connecting to \"https://openweathermap.org/\"");
                try
                {
                    // 连接到 https://openweathermap.org/ 的API
                    webRequest = (HttpWebRequest)
                        WebRequest.Create(
                            "http://api.openweathermap.org/data/2.5/weather?q=Mongkok,hk&units=metric&APPID=" + ApiKey);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection failed: ");
                    Console.WriteLine(e);
                    Console.WriteLine("====================");
                    continue;
                }

                Console.WriteLine("Reading data from API");
                try
                {
                    stream = webRequest.GetResponse().GetResponseStream();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Reading failed: ");
                    Console.WriteLine(e);
                    Console.WriteLine("====================");
                    continue;
                }

                msg = ReadStream(stream);
                if (msg == "")
                {
                    continue;
                }

                msg = ReadData(msg);

                if (serialPort == null || !serialPort.IsOpen)
                {
                    try
                    {
                        Console.WriteLine("Connecting to board");
                        serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                        serialPort.Open();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Connection failed:");
                        Console.WriteLine(e);
                        Console.WriteLine("====================");
                        Console.WriteLine();
                        try
                        {
                            serialPort.Close();
                        }
                        catch
                        {
                            //
                        }

                        serialPort = null;
                        continue;
                    }
                }

                try
                {
                    Console.WriteLine("Writing to board");
                    serialPort.Write(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Writing failed");
                    Console.WriteLine(e);
                    Console.WriteLine("====================");
                    Console.WriteLine();
                    try
                    {
                        serialPort.Close();
                    }
                    catch
                    {
                        //
                    }

                    serialPort = null;
                    continue;
                }

                try
                {
                    serialPort.ReadTimeout = 1000;
                    if (serialPort.ReadLine() == "RECEIVED")
                    {
                        Console.WriteLine("Completed");
                        Console.WriteLine("====================");
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("No response received");
                    Console.WriteLine("====================");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error found:");
                    Console.WriteLine(e);
                    Console.WriteLine("====================");
                }

                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());
                // Console.WriteLine(serialPort.ReadLine());

                Thread.Sleep(200);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private static string ReadStream(Stream stream)
        {
            var streamContent = "";
            try
            {
                var streamReader = new StreamReader(stream);
                streamContent = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("Reading failed: ");
                Console.WriteLine(e);
                Console.WriteLine("====================");
            }

            return streamContent;
        }

        private static string ReadData(string jsonMsg)
        {
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
            return temp + ';' + tempMin + ';' + tempMax + ';' + pressure + ';' + humidity + ';' + time;
        }
    }
}